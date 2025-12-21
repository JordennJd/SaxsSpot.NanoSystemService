using AutoMapper;
using Gridify;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetRadialAnalysisList;

public class RadialAnalysisProfile : Profile
{
    public RadialAnalysisProfile()
    {
        CreateMap<RadialAnalysis, RadialAnalysisDto>()
            .ReverseMap();

        CreateMap<Paging<RadialAnalysis>, Paging<RadialAnalysisDto>>()
            .ForMember(dest => dest.Data, cfg =>
                cfg.MapFrom(x => x.Data));
    }
}
