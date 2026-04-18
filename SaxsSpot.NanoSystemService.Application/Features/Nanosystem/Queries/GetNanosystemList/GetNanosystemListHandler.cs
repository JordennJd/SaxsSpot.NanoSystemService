using AutoMapper;
using FluentResults;
using Gridify;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemList;

public class GetNanosystemListHandler(INanoSystemStorage storage, IMapper mapper) : IRequestHandler<GetNanosystemListQuery, Result<Paging<NanosystemDto>>>
{
    public async Task<Result<Paging<NanosystemDto>>> Handle(GetNanosystemListQuery request, CancellationToken cancellationToken)
    {
        var query = request.Query;
        if (string.IsNullOrWhiteSpace(query.OrderBy))
            query.OrderBy = "-InputDate";

        return FluentResults.Result.Ok(mapper.Map<Paging<NanosystemDto>>(await storage.Gridify(query)));
    }
}