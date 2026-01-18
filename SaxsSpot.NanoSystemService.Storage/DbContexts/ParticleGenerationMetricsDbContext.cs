using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Storage.DbContexts;

public class ParticleGenerationMetricsDbContext(IConfiguration configuration) : GenericDbContext<ParticleGenerationMetrics>(configuration);
