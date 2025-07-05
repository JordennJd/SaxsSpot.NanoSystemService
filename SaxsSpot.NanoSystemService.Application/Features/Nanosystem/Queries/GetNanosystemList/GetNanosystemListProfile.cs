using AutoMapper;
using Gridify;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;

public class GetNanosystemListProfile : Profile
{
    public GetNanosystemListProfile()
    {
        CreateMap<Domain.Nanosystem, NanosystemDto>()
            .ReverseMap();

        CreateMap<Paging<Domain.Nanosystem>, Paging<NanosystemDto>>()
            .ForMember(dest => dest.Data, cfg =>
                cfg.MapFrom(x =>x.Data));
    }
}