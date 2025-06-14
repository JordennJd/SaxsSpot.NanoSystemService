using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
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

    [HttpPost("run-parallelepiped-generation")]
    public async Task<IActionResult> RunParallelepipedGeneration(ParallelepipedGenerationParameters dto)
    {
        _ = await _mediator.Send(new RunGenerationCommand(dto));
        return Ok();
    }
    
    [HttpPost("run-sphere-generation")]
    public async Task<IActionResult> RunSphereGeneration(SphereGenerationParameters dto)
    {
        _ = await _mediator.Send(new RunGenerationCommand(dto));
        return Ok();
    }

    [HttpGet("get")]
    public async Task<IActionResult> Get([FromQuery] GetNanosystemQuery query)
    {
        var nanosystem = await _mediator.Send(query);
        return Ok(nanosystem);
    }
}