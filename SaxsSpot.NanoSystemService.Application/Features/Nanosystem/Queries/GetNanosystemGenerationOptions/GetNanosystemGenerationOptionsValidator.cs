using FluentValidation;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;

public class GetNanosystemGenerationOptionsValidator : AbstractValidator<GetNanosystemGenerationOptionsQuery>
{
    public GetNanosystemGenerationOptionsValidator()
    {
        RuleFor(x => x.EpsilonFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.EpsilonTo)
            .When(x => x.EpsilonFrom.HasValue && x.EpsilonTo.HasValue)
            .WithMessage("EpsilonFrom must be less than or equal to EpsilonTo");

        RuleFor(x => x.ParticleCountFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.ParticleCountTo)
            .WithMessage("ParticleCountFrom must be less than or equal to ParticleCountTo");

        RuleFor(x => x.GlobalSizeFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.GlobalSizeTo)
            .When(x => x.GlobalSizeFrom.HasValue && x.GlobalSizeTo.HasValue)
            .WithMessage("GlobalSizeFrom must be less than or equal to GlobalSizeTo");

        RuleFor(x => x.NumericalConcentrationFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.NumericalConcentrationTo)
            .When(x => x.NumericalConcentrationFrom.HasValue && x.NumericalConcentrationTo.HasValue)
            .WithMessage("NumericalConcentrationFrom must be less than or equal to NumericalConcentrationTo");

        RuleFor(x => x.ExcessFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.ExcessTo)
            .When(x => x.ExcessFrom.HasValue && x.ExcessTo.HasValue)
            .WithMessage("ExcessFrom must be less than or equal to ExcessTo");

        RuleFor(x => x.MaxParticleSizeFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.MaxParticleSizeTo)
            .WithMessage("MaxParticleSizeFrom must be less than or equal to MaxParticleSizeTo");

        RuleFor(x => x.MinParticleSizeFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.MinParticleSizeTo)
            .WithMessage("MinParticleSizeFrom must be less than or equal to MinParticleSizeTo");

        RuleFor(x => x.KFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.KTo)
            .WithMessage("KFrom must be less than or equal to KTo");
        
        RuleFor(x => x.KTo)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.KFrom)
            .WithMessage("KFrom must be less than or equal to KTo");

        RuleFor(x => x.ThetaFrom)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.ThetaTo)
            .WithMessage("ThetaFrom must be less than or equal to ThetaTo");
        
        RuleFor(x => x.ThetaTo)
            .NotNull().WithMessage("Field required.")
            .LessThanOrEqualTo(x => x.ThetaFrom)
            .WithMessage("ThetaFrom must be less than or equal to ThetaTo");

        RuleFor(x => x)
            .NotNull().WithMessage("Field required.")
            .Must(x =>
                (x.GlobalSizeFrom.HasValue && x.GlobalSizeTo.HasValue) ||
                (x.NumericalConcentrationFrom.HasValue && x.NumericalConcentrationTo.HasValue))
            .WithMessage("Either GlobalSizeFrom/To or NumericalConcentrationFrom/To must be provided");

        RuleFor(x => x.EpsilonFrom)
            .NotNull().WithMessage("Field required.")
            .When(x => x.ParticleKind == ParticleKind.Parallelepiped)
            .WithMessage("EpsilonFrom is required for Parallelepiped kind");

        RuleFor(x => x.EpsilonTo)
            .NotNull().WithMessage("Field required.")
            .When(x => x.ParticleKind == ParticleKind.Parallelepiped)
            .WithMessage("EpsilonTo is required for Parallelepiped kind");
    }
}