using System.Reflection;
using Evently.Common.Application.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
namespace Evently.Common.Application;

public static class ApplicationConfiguration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        Assembly[] moduleAssemblies)
    {
        services.AddMediatR(config =>
        {
            /*
             * Scan and register all MediatR handlers
             */
            config.RegisterServicesFromAssemblies(moduleAssemblies);
            
            /**
             * Validation pipeline for all MediatR requests
             */
            config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
            
            /**
             * Mediatr pipeline exceptions handling
             */
            config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
            
            /**
             * Pipeline behavior for all MediatR requests
             */
            config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
        });

        /*
         * We can pass from the arguments
         */
        services.AddValidatorsFromAssemblies(moduleAssemblies, includeInternalTypes: true);

        return services;
    }
}
