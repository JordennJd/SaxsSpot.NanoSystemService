using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Contracts.Services;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

/// <summary>
/// Run single nanosystem generation
/// </summary>
/// <param name="nanoSystemService"></param>
public class RunMassGenerationHandler(IServiceScopeFactory scopeFactory, ILogger<RunMassGenerationHandler> logger) : IRequestHandler<RunMassGenerationCommand, Guid>
{
    public async Task<Guid> Handle(RunMassGenerationCommand request, CancellationToken cancellationToken)
    {
        var operationGuid = Guid.NewGuid();
        
        try
        {
            logger.Log(LogLevel.Information, $"Run generation started with operation id: {operationGuid}");
            
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            _ = Task.Run(() =>
            {
                var scope = scopeFactory.CreateScope();
                var nanoSystemService = scope.ServiceProvider.GetService<INanoSystemService>();

                return nanoSystemService.RunSeriesGeneration(request.Parameters,
                    cancellationToken: cancellationTokenSource.Token);
            }, cancellationTokenSource.Token);
            
            return operationGuid;
        }
        catch (OperationCanceledException e)
        {
            logger.Log(LogLevel.Information, $"Run generation canceled with operationId: {operationGuid}");
            throw;
        }
    }
}