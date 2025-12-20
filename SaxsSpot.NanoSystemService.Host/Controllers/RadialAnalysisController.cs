using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunRadialAnalysis;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Host.Controllers;

[Route("api/radial-analysis")]
public class RadialAnalysisController(IMediator mediator) : Controller
{
    [HttpPost("run-radial-analysis")]
    public async Task<IActionResult> RunGeneration([FromBody] RunRadialAnalysisCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result.ToResultDto());
    }
}