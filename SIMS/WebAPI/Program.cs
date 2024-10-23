using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using WebAPI.AuthServices;
using WebAPI.Enums;
using WebAPI.Middlewares;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Json-Error f�r Cycle-Object - so wird Fehler ignoriert
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Load secrets for database connection
#if DEBUG
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            string? secretData = config["ConnectionStrings-SQL"];
            string? secretKey = config["JWTSettings-Secret"];
            string? adminPassword = config["Admin-Password"];


#else
            IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            string? secretData = config["ConnectionStrings-SQL"];
            string? secretKey = config["JWTSettings-Secret"];
            string? adminPassword = config["Admin-Password"];
#endif
            // Register DbContext
            builder.Services.AddDbContext<SIMSContext>(options => options.UseSqlServer(secretData));

            // JWT and Authentication setup

            builder.Services.AddScoped<JwtService>();
            builder.Services.AddScoped<RedisTokenStore>();

            builder.Services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });



            // Register repositories
            builder.Services.AddScoped<IRepository<User>, UserRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<IRepository<LogEntry>, LogEntryRepository>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<AuthRepository>();

            var app = builder.Build();

            MigrateDatabase(app, secretData, adminPassword);

            // Middleware
            app.UseMiddleware<Middleware>();
   
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                Console.WriteLine("SWAGGER START");
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            // Apply CORS policy
            // app.UseCors("AllowSpecificOrigin");

            // Authentication and Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }

        private static void MigrateDatabase(WebApplication app, string? secretData, string? password)
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SIMSContext>().Database;
                scope.ServiceProvider.GetRequiredService<SIMSContext>().Database.Migrate();
            }

            using (SqlConnection connection = new SqlConnection(secretData))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM USERS WHERE ID = @Id";

                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Id", 1); 
                    int userCount = (int)checkCommand.ExecuteScalar(); 

                    if (userCount > 0)
                    {
                        Console.WriteLine("User with ID '1' already exists. Skipping creation.");
                        return; 
                    }
                }

                User user = new User()
                {
                    UserUUID = Guid.NewGuid(),
                    Email = "admin@admin.at",
                    UserName = "Administrator",
                    FirstName = "Admin",
                    LastName = "Admin",
                    Blocked = false,
                    Role = Enums.ERoles.ADMIN,
                };

                user.SetPassword(password);

                string query = "INSERT INTO USERS (User_UUID, USERNAME, FIRSTNAME, LASTNAME, PASSWORD_HASH, PASSWORD_SALT, EMAIL, ROLE, BLOCKED) " +
                               "VALUES (@UserUUID, @Username, @Firstname, @Lastname, @PasswordHash, @PasswordSalt, @Email, @Role, @Blocked)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserUUID", user.UserUUID);
                    command.Parameters.AddWithValue("@Username", user.UserName);
                    command.Parameters.AddWithValue("@Firstname", user.FirstName);
                    command.Parameters.AddWithValue("@Lastname", user.LastName);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Role", user.Role);
                    command.Parameters.AddWithValue("@Blocked", user.Blocked);

                    command.ExecuteNonQuery();
                }
            }

        }
    }
}
