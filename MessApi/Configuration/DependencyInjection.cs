using MessApi.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MessApi.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAllRepository(this IServiceCollection services)
        {
            //services.AddScoped<IUserRepository,UserRepositoy>();
            services.AddHttpContextAccessor();
            services.AddScoped<JwtService>();
            return services;
        }
        public static IServiceCollection AddAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);
            var issuers = jwtSettings.GetSection("Issuers").Get<string[]>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                    ValidateIssuer = true,
                    ValidIssuers = issuers,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // No tolerance for expiration
                };
            });
            services.AddAuthorization(options =>
            {
                // Admin policy → only Admin role
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                // Example: Manager-only policy
                options.AddPolicy("ManagerOnly", policy =>
                    policy.RequireRole("Manager"));

                // Example: Admin OR Manager
                options.AddPolicy("AdminOrManager", policy =>
                    policy.RequireRole("Admin", "Manager"));
            });

            return services;
        }
    }
}
