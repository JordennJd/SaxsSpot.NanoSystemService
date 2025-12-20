using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage.Storages;

public class RadialAnalysisStorage(RadialAnalysisDbContext dbContext)
    : GenericStorage<RadialAnalysis>(dbContext), IRadialAnalysisStorage;