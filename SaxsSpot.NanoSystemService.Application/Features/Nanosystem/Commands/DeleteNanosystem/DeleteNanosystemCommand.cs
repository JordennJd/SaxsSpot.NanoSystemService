using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.DeleteNanosystem;

public record DeleteNanosystemCommand(Guid NanosystemId, string Password) : IRequest<Result<Unit>>;
