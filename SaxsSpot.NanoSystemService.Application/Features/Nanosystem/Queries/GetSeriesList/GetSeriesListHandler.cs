using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSeriesList;

public class GetSeriesListHandler(INanoSystemSeriesStorage storage, IMapper mapper) : IRequestHandler<GetSeriesListQuery, Result<Paging<NanosystemSeriesDto>>>
{
    public async Task<Result<Paging<NanosystemSeriesDto>>> Handle(GetSeriesListQuery request, CancellationToken cancellationToken)
    {
        var query = request.Query;
        if (string.IsNullOrWhiteSpace(query.OrderBy))
            query.OrderBy = "CreatedAt desc";

        return FluentResults.Result.Ok(mapper.Map<Paging<NanosystemSeriesDto>>(await storage.Gridify(query)));
    }
}