using AutoMapper;
using Gridify;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;

public class NanosystemSeriesProfile : Profile
{
    public NanosystemSeriesProfile()
    {
        CreateMap<Domain.NanosystemSeries, NanosystemSeriesDto>()
            .ReverseMap();

        CreateMap<Paging<Domain.NanosystemSeries>, Paging<NanosystemSeriesDto>>()
            .ForMember(dest => dest.Data, cfg =>
                cfg.MapFrom(x =>x.Data));
    }
}