using Gridify;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadNanosystem;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemCalculationParameters;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemList;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSeriesList;
using SaxsSpot.NanoSystemService.Contracts.Models;
using FluentResults.Extensions.AspNetCore;
using SaxsSpot.NanoSystemService.Host.Result;

namespace SaxsSpot.NanoSystemService.Host.Controllers;

[Route("api/nanosystem")]
public class NanosystemController(IMediator mediator) : Controller
{
    [HttpPost("run-generation")]
    public async Task<IActionResult> RunGeneration([FromBody] CommonParticleGenerationParameters dto)
    {
        var result = await mediator.Send(new RunGenerationCommand(dto));
        return Ok(result.ValueOrDefault);
    }
    
    [HttpPost("run-mass-generation")]
    public async Task<IActionResult> RunMassGeneration([FromBody] MassGenerateNanoSystemOptions dto)
    {
        var result = await mediator.Send(new RunMassGenerationCommand(dto));
        return Ok(result.ValueOrDefault);
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
    
    [HttpGet("get-nanosystem-list")]
    public async Task<IActionResult> GetNanosystemList([FromQuery] GridifyQuery query)
    {
        var result = await mediator.Send(new GetNanosystemListQuery(query));
        return Ok(result.ValueOrDefault);
    }
    
    [HttpGet("get-nanosystem-mass-generation-parameters")]
    public async Task<IActionResult> GetNanosystemMassGenerationParameters([FromQuery] GetNanosystemGenerationOptionsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result.ValueOrDefault);
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
}