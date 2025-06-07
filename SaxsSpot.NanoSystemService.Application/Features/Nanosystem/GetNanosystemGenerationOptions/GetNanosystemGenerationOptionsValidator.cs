using FluentValidation;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.GetNanosystemGenerationOptions;

public class GetNanosystemGenerationOptionsValidator : AbstractValidator<GetNanosystemGenerationOptionsQuery>
{
    public GetNanosystemGenerationOptionsValidator()
    {
        RuleFor(x => x.EpsilonFrom)
            .LessThanOrEqualTo(x => x.EpsilonTo)
            .When(x => x.EpsilonFrom.HasValue && x.EpsilonTo.HasValue)
            .WithMessage("EpsilonFrom must be less than or equal to EpsilonTo");

        RuleFor(x => x.ParticleCountFrom)
            .LessThanOrEqualTo(x => x.ParticleCountTo)
            .WithMessage("ParticleCountFrom must be less than or equal to ParticleCountTo");

        RuleFor(x => x.GlobalSizeFrom)
            .LessThanOrEqualTo(x => x.GlobalSizeTo)
            .When(x => x.GlobalSizeFrom.HasValue && x.GlobalSizeTo.HasValue)
            .WithMessage("GlobalSizeFrom must be less than or equal to GlobalSizeTo");

        RuleFor(x => x.NumericalConcentrationFrom)
            .LessThanOrEqualTo(x => x.NumericalConcentrationTo)
            .When(x => x.NumericalConcentrationFrom.HasValue && x.NumericalConcentrationTo.HasValue)
            .WithMessage("NumericalConcentrationFrom must be less than or equal to NumericalConcentrationTo");

        RuleFor(x => x.ExcessFrom)
            .LessThanOrEqualTo(x => x.ExcessTo)
            .When(x => x.ExcessFrom.HasValue && x.ExcessTo.HasValue)
            .WithMessage("ExcessFrom must be less than or equal to ExcessTo");

        RuleFor(x => x.MaxParticleSizeFrom)
            .LessThanOrEqualTo(x => x.MaxParticleSizeTo)
            .WithMessage("MaxParticleSizeFrom must be less than or equal to MaxParticleSizeTo");

        RuleFor(x => x.MinParticleSizeFrom)
            .LessThanOrEqualTo(x => x.MinParticleSizeTo)
            .WithMessage("MinParticleSizeFrom must be less than or equal to MinParticleSizeTo");

        RuleFor(x => x.KFrom)
            .LessThanOrEqualTo(x => x.KTo)
            .WithMessage("KFrom must be less than or equal to KTo");

        RuleFor(x => x.ThetaFrom)
            .LessThanOrEqualTo(x => x.ThetaTo)
            .WithMessage("ThetaFrom must be less than or equal to ThetaTo");

        RuleFor(x => x)
            .Must(x =>
                (x.GlobalSizeFrom.HasValue && x.GlobalSizeTo.HasValue) ||
                (x.NumericalConcentrationFrom.HasValue && x.NumericalConcentrationTo.HasValue))
            .WithMessage("Either GlobalSizeFrom/To or NumericalConcentrationFrom/To must be provided");

        RuleFor(x => x.EpsilonFrom)
            .NotNull()
            .When(x => x.ParticleKind == ParticleKind.Parallelepiped)
            .WithMessage("EpsilonFrom is required for Parallelepiped kind");

        RuleFor(x => x.EpsilonTo)
            .NotNull()
            .When(x => x.ParticleKind == ParticleKind.Parallelepiped)
            .WithMessage("EpsilonTo is required for Parallelepiped kind");
    }
}