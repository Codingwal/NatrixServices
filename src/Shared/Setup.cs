using System.Reflection;
using NatrixServices.Shared.Application;


namespace NatrixServices.Shared;

public static class SetupExtensions
{
    public static void AddApplicationLayerServices(this IServiceCollection services, Assembly assembly)
    {
        AddCommandHandlers(services, assembly);
        AddEventHandlers(services, assembly);
        AddBackgroundTasks(services, assembly);

        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        services.AddSingleton<IEventManager, EventManager>();
        services.AddHostedService<BackgroundTaskUpdater>();
    }

    private static void AddCommandHandlers(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType
                    && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)
                    || i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                );

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }
    }

    private static void AddEventHandlers(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)
                );

            foreach (var handlerInterface in handlerInterfaces)
            {
                services.AddScoped(handlerInterface, type);
            }
        }
    }

    private static void AddBackgroundTasks(IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => typeof(IBackgroundTask).IsAssignableFrom(t));

        foreach (var type in types)
        {
            services.AddTransient(type); // Register as concrete type so they can be obtained using GetRequiredService()
            services.AddTransient(typeof(IBackgroundTask), type); // Register as interface so they are found on startup
        }
    }
}