using System.Reflection;
using CodeMate.Application.Common.Interfaces.Services;
using CodeMate.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMate.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddAutoMapper(cfg => { }, assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}