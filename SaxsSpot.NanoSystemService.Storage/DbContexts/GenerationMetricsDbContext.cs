using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Storage.DbContexts;

public class GenerationMetricsDbContext(IConfiguration configuration) : GenericDbContext<GenerationMetrics>(configuration);
