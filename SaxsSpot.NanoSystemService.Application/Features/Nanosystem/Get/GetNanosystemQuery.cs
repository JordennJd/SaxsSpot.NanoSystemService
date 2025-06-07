using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Get;

public record GetNanosystemQuery(Guid Id) : IRequest<Result<NanosystemDto>>;