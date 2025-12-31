using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.CancelOperation;

public record CancelOperationCommand(Guid OperationId, string? OperationType = null) : IRequest<Result>;
