using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Host.Controllers;

[Route("api/nanosystem")]
public class NanosystemController : Controller
{
    private readonly IMediator _mediator;
    
    public NanosystemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("run-generation")]
    public async Task<IActionResult> RunParallelepipedGeneration([FromBody] CommonParticleGenerationParameters dto)
    {
        _ = await _mediator.Send(new RunGenerationCommand(dto));
        return Ok();
    }
    
    [HttpPost("run-mass-generation")]
    public async Task<IActionResult> RunSphereGeneration([FromBody] MassGenerateNanoSystemOptions dto)
    {
        _ = await _mediator.Send(new RunMassGenerationCommand(dto));
        return Ok();
    }


    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] GetNanosystemQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.ValueOrDefault);
    }
    
    [HttpGet("get-nanosystem-mass-generation-parameters")]
    public async Task<IActionResult> GetNanosystemMassGenerationParameters([FromQuery] GetNanosystemGenerationOptionsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result.ValueOrDefault);
    }
}