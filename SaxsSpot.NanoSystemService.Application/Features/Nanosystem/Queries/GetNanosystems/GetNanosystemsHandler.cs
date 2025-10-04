using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystems;

public class GetNanosystemsHandler(INanoSystemStorage storage, IMapper mapper) : IRequestHandler<GetNanosystemsQuery, Result<IEnumerable<NanosystemDto>>>
{
    public async Task<Result<IEnumerable<NanosystemDto>>> Handle(GetNanosystemsQuery request, CancellationToken cancellationToken)
    {
        return FluentResults.Result.Ok(mapper.Map<IEnumerable<NanosystemDto>>(await storage.WhereByGridifyStringAsync(request.Query.Filter)));
    }
}