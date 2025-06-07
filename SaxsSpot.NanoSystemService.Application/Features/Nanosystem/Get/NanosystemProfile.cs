using AutoMapper;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Get;

public class NanosystemProfile : Profile
{
    public NanosystemProfile()
    {
        CreateMap<Domain.Nanosystem, NanosystemDto>()
            .ReverseMap();
    }
}