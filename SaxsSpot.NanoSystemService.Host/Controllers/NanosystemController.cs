using Gridify;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSeriesList;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Host.Controllers;

[Route("api/nanosystem")]
public class NanosystemController(IMediator mediator) : Controller
{
    [HttpPost("run-generation")]
    public async Task<IActionResult> RunParallelepipedGeneration([FromBody] CommonParticleGenerationParameters dto)
    {
        _ = await mediator.Send(new RunGenerationCommand(dto));
        return Ok();
    }
    
    [HttpPost("run-mass-generation")]
    public async Task<IActionResult> RunSphereGeneration([FromBody] MassGenerateNanoSystemOptions dto)
    {
        _ = await mediator.Send(new RunMassGenerationCommand(dto));
        return Ok();
    }


    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] GetNanosystemQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result.ValueOrDefault);
    }
    
    [HttpGet("get-nanosystem-series-list")]
    public async Task<IActionResult> GetNanosystemSeriesList([FromQuery] GridifyQuery query)
    {
        var result = await mediator.Send(new GetSeriesListQuery(query));
        return Ok(result.ValueOrDefault);
    }
    
    [HttpGet("get-nanosystem-mass-generation-parameters")]
    public async Task<IActionResult> GetNanosystemMassGenerationParameters([FromQuery] GetNanosystemGenerationOptionsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result.ValueOrDefault);
    }
}