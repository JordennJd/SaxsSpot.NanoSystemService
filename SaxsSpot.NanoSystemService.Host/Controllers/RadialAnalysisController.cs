using Gridify;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunRadialAnalysis;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadRadialAnalysis;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetRadialAnalysisList;
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
    
    [HttpGet("get-radial-analysis-list")]
    public async Task<IActionResult> GetRadialAnalysisList([FromQuery] GridifyQuery query)
    {
        var result = await mediator.Send(new GetRadialAnalysisListQuery(query));
        return Ok(result.ToResultDto());
    }
    
    [HttpGet("download-radial-analysis")]
    public async Task<IActionResult> DownloadRadialAnalysis([FromQuery] DownloadRadialAnalysisQuery query)
    {
        var result = await mediator.Send(query);
        return File(result.ValueOrDefault, "application/octet-stream", $"{query.Id}");
    }
}