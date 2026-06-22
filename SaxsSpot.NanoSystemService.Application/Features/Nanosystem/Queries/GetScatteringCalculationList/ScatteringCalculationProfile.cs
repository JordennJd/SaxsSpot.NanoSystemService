using AutoMapper;
using Gridify;
using SaxsSpot.NanoSystemService.Contracts.Enums;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetScatteringCalculationList;

public class ScatteringCalculationProfile : Profile
{
    public ScatteringCalculationProfile()
    {
        CreateMap<ScatteringCalculation, ScatteringCalculationDto>()
            .ForMember(dest => dest.CalculationKind, opt => opt.MapFrom(src => (ScatteringCalculationKind)src.CalculationKind));
        CreateMap<Paging<ScatteringCalculation>, Paging<ScatteringCalculationDto>>()
            .ForMember(dest => dest.Data, cfg => cfg.MapFrom(x => x.Data));
    }
}
