using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Storage.DbContexts;

public class RadialAnalysisLayerDbContext(IConfiguration configuration)
    : GenericDbContext<RadialAnalysisLayer>(configuration);
