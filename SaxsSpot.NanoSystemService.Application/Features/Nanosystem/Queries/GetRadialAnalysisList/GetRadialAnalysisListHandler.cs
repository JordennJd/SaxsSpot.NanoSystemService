using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetRadialAnalysisList;

public class GetRadialAnalysisListHandler(IRadialAnalysisStorage storage, IMapper mapper) 
    : IRequestHandler<GetRadialAnalysisListQuery, Result<Paging<RadialAnalysisDto>>>
{
    public async Task<Result<Paging<RadialAnalysisDto>>> Handle(GetRadialAnalysisListQuery request, CancellationToken cancellationToken)
    {
        return FluentResults.Result.Ok(mapper.Map<Paging<RadialAnalysisDto>>(await storage.Gridify(request.Query)));
    }
}
