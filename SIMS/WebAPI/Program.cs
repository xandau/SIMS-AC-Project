using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
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
                // Json-Error für Cycle-Object - so wird Fehler ignoriert
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Load secrets for database connection
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            secretProvider.TryGet("ConnectionStrings:SQL", out var secretData);

            // Register DbContext
            builder.Services.AddDbContext<SIMSContext>(options => options.UseSqlServer(secretData));

            // Register repositories
            builder.Services.AddScoped<IRepository<User>, UserRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();
            builder.Services.AddScoped<IRepository<LogEntry>, LogEntryRepository>();
            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<AuthRepository>();

            // JWT and Authentication setup
            var jwtSettings = config.GetSection("JWTSettings");
            var secretKey = jwtSettings["Secret"];

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
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Add and configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    policy => policy.WithOrigins("http://localhost:4200")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod());
            });

            var app = builder.Build();

            // Middleware - differ between ADMIN and USER in /user
            app.UseMiddleware<RoleMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Apply CORS policy
            app.UseCors("AllowSpecificOrigin");

            // Authentication and Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
