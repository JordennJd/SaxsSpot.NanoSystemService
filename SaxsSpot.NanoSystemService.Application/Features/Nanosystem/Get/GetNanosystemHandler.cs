using AutoMapper;
using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Storage.Contracts;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Get;

public class GetNanosystemHandler(INanoSystemStorage nanoSystemStorage, IMapper mapper)
    : IRequestHandler<GetNanosystemQuery, Result<NanosystemDto>>
{
    public async Task<Result<NanosystemDto>> Handle(GetNanosystemQuery request, CancellationToken cancellationToken)
    {
        var nanosystem = await nanoSystemStorage.FirstOrDefaultAsync(x => x.Id == request.Id);
        
        if (nanosystem is null)
        {
            throw new KeyNotFoundException("nanosystem not found");
        }
        
        return Result.Ok(mapper.Map<NanosystemDto>(nanosystem));
    }
}