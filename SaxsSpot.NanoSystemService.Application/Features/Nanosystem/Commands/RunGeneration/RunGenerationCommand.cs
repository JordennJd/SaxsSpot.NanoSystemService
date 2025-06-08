using MediatR;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

public record RunGenerationCommand(ParticleGenerationParameters Parameters) : IRequest<Guid>;