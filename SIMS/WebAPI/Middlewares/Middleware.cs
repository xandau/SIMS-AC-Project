using Azure.Core;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
using System.Text.Json;
using WebAPI.AuthServices;
using WebAPI.Enums;
using WebAPI.Repository;

namespace WebAPI.Middlewares
{
    public class Middleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly JwtService _jwtService;

        public Middleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
            _jwtService = new JwtService();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Bypass middleware logic for health check endpoint
            if (context.Request.Path.StartsWithSegments("/health"))
            {
                await _requestDelegate(context);
                return;
            }

#if DEBUG
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string? secretData = config["ConnectionStrings-SQL"];
            string? secretDataRedis = config["ConnectionStrings-REDIS"];
#else
            IConfigurationRoot envConfig = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            string? baseConnectionString = envConfig["ConnectionStrings-SQL"];
            string? secretDataRedis = envConfig["ConnectionStrings-REDIS"];

            string? fullSqlConnectionStringForMiddleware = baseConnectionString; // Default
            try
            {
                IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
                JsonDocument secretJson = JsonDocument.Parse(config["SQL-User"]);
                //JsonDocument secretJson = await Program.GetSecret(); // CALL GetSecret()
                string? username = secretJson.RootElement.GetProperty("username").GetString();
                string? password = secretJson.RootElement.GetProperty("password").GetString();

                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    if (!string.IsNullOrEmpty(baseConnectionString) && !baseConnectionString.Trim().EndsWith(";"))
                    {
                        baseConnectionString += ";";
                    }
                    fullSqlConnectionStringForMiddleware = $"{baseConnectionString}User Id={username};Password={password};";
                }
                // ... (error logging) ...
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Middleware Error: Failed to fetch/parse SQL secrets or build connection string: {ex.Message}");
            }
#endif

            //var redisOptions = ConfigurationOptions.Parse(secretDataRedis);
            //redisOptions.ConnectTimeout = 5000;

            var redisOptions = new ConfigurationOptions
            {
                EndPoints = { secretDataRedis },
                /*
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                */
                AllowAdmin = true,
                Ssl = true
            };

            /*

            if (context.Request.Method == HttpMethods.Options)
            {
                // If it's an OPTIONS request, simply return
                context.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }
            */

            try
            {
                // Use the potentially more complete connection string
                if (!string.IsNullOrEmpty(fullSqlConnectionStringForMiddleware))
                {
                    using (SqlConnection connection = new SqlConnection(fullSqlConnectionStringForMiddleware)) // USE THE ASSEMBLED STRING
                    {
                        await connection.OpenAsync();
                    }
                } else { /* log error */ }
            }
            catch (Exception ex)
            {
                // Print error in Console
                Console.WriteLine(ex.Message);

                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    await context.Response.WriteAsJsonAsync(new { Type = "error", Message = "An error occurred. Please try again later." });
                    return; 
                }
            }

            if(context.Request.Path.ToString().StartsWith("/auth/login") || context.Request.Path.ToString().StartsWith("/auth/refresh"))
            {
                // Bypass Redis check if health check somehow matched this (though unlikely with above check)
                if (context.Request.Path.StartsWithSegments("/health"))
                {
                    await _requestDelegate(context);
                    return;
                }
                try
                {
                    using (ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(redisOptions))
                    {
                        var database = redis.GetDatabase();
                    }
                }
                catch (Exception ex)
                {
                    // Print error in Console
                    Console.WriteLine(ex.Message);

                    if (!context.Response.HasStarted)
                    {
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        await context.Response.WriteAsJsonAsync(new { Type = "error", Message = "An error occurred. Please try again later." });
                        return;
                    }
                }
            }



            if (context.Request.Path.ToString().StartsWith("/user") && !(context.Request.Method.ToUpper()=="GET" && context.Request.Path.ToString().StartsWith("/user/")))
            {
                // Bypass auth check if health check somehow matched this (though unlikely with above check)
                if (context.Request.Path.StartsWithSegments("/health"))
                {
                    await _requestDelegate(context);
                    return;
                }
                var authHeader = context.Request.Headers["Authorization"].ToString();

                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    string token = authHeader.Substring("Bearer ".Length).Trim();

                    long id = _jwtService.GetClaimsFromToken(token);

                    using (SqlConnection connection = new SqlConnection(fullSqlConnectionStringForMiddleware))
                    {
                        connection.Open();

                        SqlCommand command = new SqlCommand("select ID from USERS where ID = @userid AND ROLE = 2", connection);
                        command.Parameters.AddWithValue("@userid", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                await _requestDelegate(context);
                            }
                            else
                            {
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                await context.Response.WriteAsJsonAsync(new { Type = "error", Message = "You are not allowed to do this operation." });
                            }
                        }
                    }
                }
            }
            else
                await _requestDelegate(context);
        }
    }
}
