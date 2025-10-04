using Gridify;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadNanosystem;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemCalculationParameters;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemList;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystems;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSeriesList;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSphereScatteringCalculationParameters;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Host.Controllers;

[Route("api/nanosystem")]
public class NanosystemController(IMediator mediator) : Controller
{
    [HttpPost("run-generation")]
    public async Task<IActionResult> RunGeneration([FromBody] CommonParticleGenerationParameters dto)
    {
        var result = await mediator.Send(new RunGenerationCommand(dto));
        return Ok(result.ToResultDto());
    }
    
    [HttpPost("run-mass-generation")]
    public async Task<IActionResult> RunMassGeneration([FromBody] MassGenerateNanoSystemOptions dto)
    {
        var result = await mediator.Send(new RunMassGenerationCommand(dto));
        return Ok(result.ToResultDto());
    }


    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] GetNanosystemQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("get-nanosystem-series-list")]
    public async Task<IActionResult> GetNanosystemSeriesList([FromQuery] GridifyQuery query)
    {
        var result = await mediator.Send(new GetSeriesListQuery(query));
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("get-nanosystem-list")]
    public async Task<IActionResult> GetNanosystemList([FromQuery] GridifyQuery query)
    {
        var result = await mediator.Send(new GetNanosystemListQuery(query));
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("get-nanosystem-mass-generation-parameters")]
    public async Task<IActionResult> GetNanosystemMassGenerationParameters([FromQuery] GetNanosystemGenerationOptionsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("download-nanosystem")]
    public async Task<IActionResult> DownloadNanosystem([FromQuery] DownloadNanosystemQuery query)
    {
        var result = await mediator.Send(query);
        return File(result.ValueOrDefault, "application/octet-stream", $"{query.Id}");
    }
    
    [HttpGet("nanosystem-calculation-parameters")]
    public async Task<IActionResult> GetNanosystemCalculationParameters([FromQuery] GetNanosystemCalculationParametersQuery query)
    {
        var result = await mediator.Send(query);
        if (result.IsFailed)
        {
            return BadRequest(result.ToResultDto());
        }
        
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("sphere-scattering-calculation-parameters")]
    public async Task<IActionResult> GetSphereScatteringCalculationParameters([FromQuery] GetSphereScatteringCalculationParametersQuery query)
    {
        var result = await mediator.Send(query);
        if (result.IsFailed)
        {
            return BadRequest(result.ToResultDto());
        }
        
        return Ok(result.ToResultDto());
    }
    
    [HttpGet]
    public async Task<IActionResult> GetNanosystem([FromQuery] ApiQuery query)
    {
        var result = await mediator.Send(new GetNanosystemsQuery(query));
        return Ok(result.ToResultDto());
    }
}