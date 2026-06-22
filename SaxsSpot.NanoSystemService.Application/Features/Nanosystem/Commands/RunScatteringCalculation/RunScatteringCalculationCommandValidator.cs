using FluentValidation;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunScatteringCalculation;

public class RunScatteringCalculationCommandValidator : AbstractValidator<RunScatteringCalculationCommand>
{
    public RunScatteringCalculationCommandValidator()
    {
        RuleFor(x => x.NanosystemId).NotEmpty();
        RuleFor(x => x.QSpaceParameters).NotNull();
        RuleFor(x => x.QSpaceParameters.End).GreaterThan(x => x.QSpaceParameters.Start);
        RuleFor(x => x.QSpaceParameters.SpaceParameter).GreaterThan(0);
        RuleFor(x => x.Excess)
            .InclusiveBetween(0, 1)
            .When(x => x.Excess.HasValue);
    }
}
