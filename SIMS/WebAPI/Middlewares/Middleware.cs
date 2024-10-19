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
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            secretProvider.TryGet("ConnectionStrings:SQL", out var secretData);
            secretProvider.TryGet("ConnectionStrings:REDIS", out var secretDataRedis);

            var redisOptions = ConfigurationOptions.Parse(secretDataRedis);
            redisOptions.ConnectTimeout = 5000;

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

            if (context.Request.Path.ToString().StartsWith("/user"))
            {
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
