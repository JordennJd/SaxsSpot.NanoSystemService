using AutoMapper;
using FluentValidation;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Validation;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

public class RunGenerationValidator : AbstractValidator<RunGenerationCommand>
{
    public RunGenerationValidator(IMapper mapper)
    {
        RuleFor(x => x).Custom((parameters, context) =>
        {
            switch (parameters.Parameters.GetParticleKind())
            {
                case ParticleKind.Parallelepiped:
                    var parametersValidator = new ParallelepipedGenerationParametersValidator();
                    var parallelepipedResult = parametersValidator.Validate(mapper.Map<ParallelepipedGenerationParameters>(parameters.Parameters));
                    foreach (var error in parallelepipedResult.Errors)
                    {
                        context.AddFailure(error);
                    }
                    break;
                
                case ParticleKind.Sphere:
                    var sphereGenerationParametersValidator = new SphereGenerationParametersValidator();
                    var result = sphereGenerationParametersValidator.Validate(mapper.Map<SphereGenerationParameters>(parameters.Parameters));
                    foreach (var error in result.Errors)
                    {
                        context.AddFailure(error);
                    }

                    break;
                default:
                    context.AddFailure("invalid parameters");
                    break;
            }
        });
    }
}