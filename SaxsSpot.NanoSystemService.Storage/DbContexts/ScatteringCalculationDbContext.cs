using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Storage.DbContexts;

public class ScatteringCalculationDbContext(IConfiguration configuration)
    : GenericDbContext<ScatteringCalculation>(configuration);
