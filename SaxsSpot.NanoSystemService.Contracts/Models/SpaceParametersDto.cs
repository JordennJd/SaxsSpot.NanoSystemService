using System.Text.Json.Serialization;
using SaxsSpot.NanoSystemService.Contracts.Enums;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record SpaceParametersDto(
    [property: JsonPropertyName("spaceMethod")] SpaceMethod SpaceMethod,
    [property: JsonPropertyName("scaleMethod")] ScaleMethod ScaleMethod,
    [property: JsonPropertyName("spaceParameter")] double SpaceParameter,
    [property: JsonPropertyName("start")] double Start,
    [property: JsonPropertyName("end")] double End);
