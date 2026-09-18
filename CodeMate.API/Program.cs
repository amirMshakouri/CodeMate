using CodeMate.API.Extensions;
using CodeMate.API.Middlewares;
using CodeMate.Application.DependencyInjection;
using CodeMate.Infrastructure.DependencyInjection;
using CodeMate.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CodeMate.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<CodeMate.API.Filters.ValidationFilter>();
            });
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerWithJwt();

            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddCorsPolicy(builder.Configuration);

            var app = builder.Build();

            // اجرای خودکار Migrationها موقع بالا اومدن برنامه
            // (نیازی به اجرای دستور Update-Database یا dotnet ef از بیرون نیست)
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();

                var passwordHasher = scope.ServiceProvider
                    .GetRequiredService<CodeMate.Application.Common.Interfaces.Security.IPasswordHasher>();

                await CodeMate.Infrastructure.Persistence.Seed.SeedData
                    .SeedAdminAsync(db, passwordHasher, builder.Configuration);
            }

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("FrontendPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }
    }
}