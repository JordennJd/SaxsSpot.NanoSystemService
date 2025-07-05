using AutoMapper;
using FluentValidation;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Validation;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

public class RunMassGenerationValidator : AbstractValidator<RunMassGenerationCommand>
{
    public RunMassGenerationValidator(IMapper mapper)
    {
        RuleFor(x => x).Custom((parameters, context) =>
        {
            if (parameters?.Parameters is null)
            {
                context.AddFailure("Parameters is invalid");
            }
            
            foreach (var option in parameters.Parameters.Options)
            {
                switch (option.GetParticleKind())
                {
                    case ParticleKind.Parallelepiped:
                        var parametersValidator = new ParallelepipedGenerationParametersValidator();
                        var parallelepipedResult = parametersValidator.Validate(mapper.Map<ParallelepipedGenerationParameters>(option));
                        foreach (var error in parallelepipedResult.Errors)
                        {
                            context.AddFailure(error);
                        }
                        break;
                
                    case ParticleKind.Sphere:
                        var sphereGenerationParametersValidator = new SphereGenerationParametersValidator();
                        var result = sphereGenerationParametersValidator.Validate(mapper.Map<SphereGenerationParameters>(option));
                        foreach (var error in result.Errors)
                        {
                            context.AddFailure(error);
                        }

                        break;
                    default:
                        context.AddFailure("invalid parameters");
                        break;
                }            
            }
        });
    }
}