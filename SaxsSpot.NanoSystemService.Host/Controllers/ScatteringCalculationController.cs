using Gridify;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunScatteringCalculation;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadScatteringCalculation;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetScatteringCalculationList;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Host.Controllers;

[Route("api/scattering-calculation")]
public class ScatteringCalculationController(IMediator mediator) : Controller
{
    [HttpPost("run-scattering-calculation")]
    public async Task<IActionResult> RunScatteringCalculation([FromBody] RunScatteringCalculationCommand request)
    {
        var result = await mediator.Send(request);
        return Ok(result.ToResultDto());
    }

    [HttpGet("get-scattering-calculation-list")]
    public async Task<IActionResult> GetScatteringCalculationList([FromQuery] GridifyQuery query)
    {
        var result = await mediator.Send(new GetScatteringCalculationListQuery(query));
        return Ok(result.ToResultDto());
    }

    [HttpGet("download-scattering-calculation")]
    public async Task<IActionResult> DownloadScatteringCalculation([FromQuery] DownloadScatteringCalculationQuery query)
    {
        var result = await mediator.Send(query);
        return File(result.ValueOrDefault, "application/octet-stream", $"{query.Id}");
    }
}
