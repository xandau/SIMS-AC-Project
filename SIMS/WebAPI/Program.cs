using Amazon.SecretsManager;
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
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

namespace WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                // Json-Error für Cycle-Object - so wird Fehler ignoriert
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
            JsonDocument secretData = await GetSecret();
            string? baseConnectionString = config["ConnectionString-SQL"]; // Retrieve base connection string from environment variables
            string? secretKey = config["JWTSettings-Secret"];
            string? adminPassword = config["Admin-Password"];
#endif
            // Register DbContext
#if DEBUG
            builder.Services.AddDbContext<SIMSContext>(options => options.UseSqlServer(secretData));
#else
            // Build the complete connection string
            string? username = secretData.RootElement.GetProperty("username").GetString();
            string? password = secretData.RootElement.GetProperty("password").GetString();
            string connectionString = $"{baseConnectionString}User ID={username};Password={password};";

            builder.Services.AddDbContext<SIMSContext>(options => options.UseSqlServer(connectionString));
#endif

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

#if DEBUG
            MigrateDatabase(app, secretData, adminPassword);
#else
            // Build the complete connection string
            //string usernameAdmin = secretData.RootElement.GetProperty("username").GetString();
            //string passwordAdmin = secretData.RootElement.GetProperty("password").GetString();
            //string connectionStringAdmin = $"{baseConnectionString};User ID={usernameAdmin};Password={passwordAdmin};";
            MigrateDatabase(app, connectionString, adminPassword);
#endif

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

        static async Task<JsonDocument> GetSecret()
        {
            string secretName = "rds!db-1c56e640-dff0-4bce-aa20-2cff1209ee86";
            string region = "eu-central-1";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified.
            };

            GetSecretValueResponse response;

            try
            {
                response = await client.GetSecretValueAsync(request);
            }
            catch (Exception e)
            {
                // For a list of the exceptions thrown, see
                // https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_GetSecretValue.html
                throw e;
            }

            string secret = response.SecretString;

            JsonDocument jsonDocument = JsonDocument.Parse(secret);
            return jsonDocument;

            // Your code goes here
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
