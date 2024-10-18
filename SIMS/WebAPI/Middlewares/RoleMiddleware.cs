using Microsoft.Data.SqlClient;
using WebAPI.AuthServices;
using WebAPI.Enums;
using WebAPI.Repository;

namespace WebAPI.Middlewares
{
    public class RoleMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly JwtService _jwtService;

        public RoleMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
            _jwtService = new JwtService();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.ToString().StartsWith("/user"))
            {
                IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
                IConfigurationProvider secretProvider = config.Providers.First();
                secretProvider.TryGet("ConnectionStrings:SQL", out var secretData);

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
                                await context.Response.WriteAsync("Zugriff verweigert: Bedingung nicht erfüllt.");
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
