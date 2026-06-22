using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetScatteringCalculationList;

public class GetScatteringCalculationListHandler(IScatteringCalculationStorage storage, IMapper mapper)
    : IRequestHandler<GetScatteringCalculationListQuery, Result<Paging<ScatteringCalculationDto>>>
{
    public async Task<Result<Paging<ScatteringCalculationDto>>> Handle(
        GetScatteringCalculationListQuery request,
        CancellationToken cancellationToken)
    {
        return FluentResults.Result.Ok(mapper.Map<Paging<ScatteringCalculationDto>>(await storage.Gridify(request.Query)));
    }
}
