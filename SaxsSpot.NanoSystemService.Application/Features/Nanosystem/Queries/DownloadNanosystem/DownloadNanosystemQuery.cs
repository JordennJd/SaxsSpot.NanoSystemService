using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadNanosystem;

public record DownloadNanosystemQuery(Guid Id) : IRequest<IResult<Stream>>;