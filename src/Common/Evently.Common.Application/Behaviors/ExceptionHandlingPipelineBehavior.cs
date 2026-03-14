using Evently.Common.Application.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Evently.Common.Application.Behaviors;


internal sealed class ExceptionHandlingPipelineBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingPipelineBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            // Handle current request and catch any unhandled exceptions that may occur in the pipeline or handlers
            return await next();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception for {RequestName}", typeof(TRequest).Name);

            throw new EventlyException(typeof(TRequest).Name, innerException: exception);
        }
    }
}
