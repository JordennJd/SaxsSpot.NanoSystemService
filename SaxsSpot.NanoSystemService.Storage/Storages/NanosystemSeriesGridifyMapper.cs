using Gridify;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Storage;

public sealed class NanosystemSeriesGridifyMapper : GridifyMapper<NanosystemSeries>
{
    public NanosystemSeriesGridifyMapper()
    {
        AddMap("Id", x => x.Id);
        AddMap("ParticleKind", x => x.ParticleKind);
        AddMap("ParticleCountFrom", x => x.ParticleCountFrom);
        AddMap("ParticleCountTo", x => x.ParticleCountTo);
        AddMap("GlobalSizeFrom", x => x.GlobalSizeFrom);
        AddMap("GlobalSizeTo", x => x.GlobalSizeTo);
        AddMap("NumericalConcentrationFrom", x => x.NumericalConcentrationFrom);
        AddMap("NumericalConcentrationTo", x => x.NumericalConcentrationTo);
        AddMap("ExcessFrom", x => x.ExcessFrom);
        AddMap("ExcessTo", x => x.ExcessTo);
        AddMap("MaxParticleSizeFrom", x => x.MaxParticleSizeFrom);
        AddMap("MaxParticleSizeTo", x => x.MaxParticleSizeTo);
        AddMap("MinParticleSizeFrom", x => x.MinParticleSizeFrom);
        AddMap("MinParticleSizeTo", x => x.MinParticleSizeTo);
        AddMap("KFrom", x => x.KFrom);
        AddMap("KTo", x => x.KTo);
        AddMap("ThetaFrom", x => x.ThetaFrom);
        AddMap("ThetaTo", x => x.ThetaTo);
        AddMap("Comment", x => x.Comment);
        AddMap("comment", x => x.Comment);
        AddMap("series_comment", x => x.Comment);
        AddMap("CreatedAt", x => x.CreatedAt);
        AddMap("createdAt", x => x.CreatedAt);
        AddMap("created_at", x => x.CreatedAt);
    }
}
