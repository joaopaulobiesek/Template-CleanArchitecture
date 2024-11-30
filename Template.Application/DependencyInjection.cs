using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Template.Application.Common.Behaviours;

namespace Template.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Registra todos os handlers e serviços
        assembly
            .ExportedTypes
            .Where(x => x.IsClass && !x.IsAbstract)
            .SelectMany(x => x.GetInterfaces(), (c, i) => new { Class = c, Interface = i })
            .ToList()
            .ForEach(tipo => services.AddScoped(tipo.Interface, tipo.Class));

        // Registra todos os validadores do FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped(typeof(HandlerDependencies<,>));

        return services;
    }
}