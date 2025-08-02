using AutoMapper;
using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;

/// <summary>
/// Get nanosystem parameters by id
/// </summary>
/// <param name="nanoSystemStorage"></param>
/// <param name="mapper"></param>
public class GetNanosystemHandler(INanoSystemStorage nanoSystemStorage, IMapper mapper)
    : IRequestHandler<GetNanosystemQuery, IResult<NanosystemDto>>
{
    public async Task<IResult<NanosystemDto>> Handle(GetNanosystemQuery request, CancellationToken cancellationToken)
    {
        var nanosystem = await nanoSystemStorage.FirstOrDefaultAsync(x => x.Id == request.Id);
        
        if (nanosystem is null)
        {
            throw new KeyNotFoundException("nanosystem not found");
        }

        return FluentResults.Result.Ok(mapper.Map<NanosystemDto>(nanosystem));
    }
}