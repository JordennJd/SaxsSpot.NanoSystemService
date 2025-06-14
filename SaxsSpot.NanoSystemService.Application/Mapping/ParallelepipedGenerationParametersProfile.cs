using AutoMapper;
using MathNet.Numerics.LinearAlgebra;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Mapping;

public class ParallelepipedGenerationParametersProfile : Profile
{
    public ParallelepipedGenerationParametersProfile()
    {
        CreateMap<CommonParticleGenerationParameters, ParallelepipedGenerationParameters>()
            .ReverseMap();
        
        CreateMap<CommonParticleGenerationParameters, SphereGenerationParameters>()
            .ReverseMap();
    }
}