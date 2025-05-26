using Azure.Core;
using Microsoft.Data.SqlClient;
using StackExchange.Redis;
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
            IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            string? secretData = config["ConnectionStrings-SQL"];
            string? secretDataRedis = config["ConnectionStrings-REDIS"];
#endif

            var redisOptions = ConfigurationOptions.Parse(secretDataRedis);
            redisOptions.ConnectTimeout = 5000;

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
                using (SqlConnection connection = new SqlConnection(secretData))
                {
                    await connection.OpenAsync();
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

                    using (SqlConnection connection = new SqlConnection(secretData))
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
