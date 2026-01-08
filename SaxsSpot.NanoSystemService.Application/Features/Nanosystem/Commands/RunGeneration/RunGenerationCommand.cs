using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

public record RunGenerationCommand(CommonParticleGenerationParameters Parameters, Guid OperationId) : IRequest<Result<Guid>>;