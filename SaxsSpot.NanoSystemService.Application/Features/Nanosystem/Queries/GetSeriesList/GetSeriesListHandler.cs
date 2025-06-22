using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Storage.Contracts;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSeriesList;

public class GetSeriesListHandler(INanoSystemSeriesStorage storage, IMapper mapper) : IRequestHandler<GetSeriesListQuery, Result<Paging<NanosystemSeriesDto>>>
{
    public async Task<Result<Paging<NanosystemSeriesDto>>> Handle(GetSeriesListQuery request, CancellationToken cancellationToken)
    {
        return Result.Ok(mapper.Map<Paging<NanosystemSeriesDto>>(await storage.Gridify(request.Query)));
    }
}