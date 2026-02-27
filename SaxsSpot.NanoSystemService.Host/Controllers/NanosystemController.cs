using FluentResults;
using Gridify;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.CancelOperation;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.DeleteNanosystem;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.DeleteSeries;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadNanosystem;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemCalculationParameters;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemList;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystems;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetParticles;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSeriesList;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetSphereScatteringCalculationParameters;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetGenerationMetrics;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Host.Controllers;

[Route("api/nanosystem")]
public class NanosystemController(IMediator mediator) : Controller
{
    [HttpPost("run-mass-generation")]
    public async Task<IActionResult> RunMassGeneration([FromBody] MassGenerateNanoSystemOptions dto)
    {
        var result = await mediator.Send(new RunMassGenerationCommand(dto));
        return Ok(result.ToResultDto());
    }
    
    [HttpPost("cancel-operation")]
    public async Task<IActionResult> CancelOperation([FromBody] CancelOperationRequest request)
    {
        var result = await mediator.Send(new CancelOperationCommand(request.OperationId, request.OperationType));
        if (result.IsFailed)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }
    
    public record CancelOperationRequest(Guid OperationId, string? OperationType = null);


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

    /// <summary>
    /// Returns a slice of particles for 3D viewer. particleKind: 0 = Sphere, 1 = Parallelepiped (optional).
    /// </summary>
    [HttpGet("get-particles")]
    public async Task<IActionResult> GetParticles(
        [FromQuery] Guid nanosystemId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10000,
        [FromQuery] int? particleKind = null)
    {
        var filterKind = particleKind switch { 0 => ParticleKind.Sphere, 1 => ParticleKind.Parallelepiped, _ => (ParticleKind?)null };
        var result = await mediator.Send(new GetParticlesQuery(nanosystemId, skip, take, filterKind));
        if (result.IsFailed)
            return BadRequest(result.ToResultDto());
        return Ok(result.Value);
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
    
    [HttpPost("get-generation-metrics")]
    public async Task<IActionResult> GetGenerationMetrics([FromBody] GetGenerationMetricsQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result.ToResultDto());
    }

    [HttpDelete("delete-nanosystem")]
    public async Task<IActionResult> DeleteNanosystem([FromBody] DeleteNanosystemRequest request)
    {
        var result = await mediator.Send(new DeleteNanosystemCommand(request.NanosystemId, request.Password));
        if (result.IsFailed)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    [HttpDelete("delete-series")]
    public async Task<IActionResult> DeleteSeries([FromBody] DeleteSeriesRequest request)
    {
        var result = await mediator.Send(new DeleteSeriesCommand(request.SeriesId));
        if (result.IsFailed)
        {
            return BadRequest(result.ToResultDto());
        }
        return Ok(result.ToResultDto());
    }

    public record DeleteNanosystemRequest(Guid NanosystemId, string Password);
    public record DeleteSeriesRequest(Guid SeriesId);
}