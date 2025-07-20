using System.Globalization;
using System.Text.Json;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Services;

public static class NanosystemWriter
{
    public static async Task<MemoryStream> Write(IAsyncEnumerable<Particle> system, Nanosystem nanosystem)
    {
        var memoryStream = new MemoryStream();
        bool isFirstLine = true;
        
        var json = JsonSerializer.Serialize(nanosystem);
        await using (var writer = new StreamWriter(memoryStream, leaveOpen: true))
        {
            await writer.WriteLineAsync(json);
            await foreach (var p in system)
            {
                if (!isFirstLine)
                {
                    await writer.WriteLineAsync();
                }
                else
                {
                    isFirstLine = false;
                }

                if (p is Parallelepiped parallelepiped)
                {
                    await writer.WriteAsync(string.Format(CultureInfo.InvariantCulture,
                        $"{parallelepiped.A:G7} {parallelepiped.E:G7} {parallelepiped.X:G7} {parallelepiped.Y:G7} " +
                        $"{parallelepiped.Z:G7} {parallelepiped.Phi:G7} {parallelepiped.Theta:G7} {parallelepiped.Zenit:G7}").Replace(',', '.'));
                }
                else if (p is Sphere sphere)
                {
                    await writer.WriteAsync(string.Format(CultureInfo.InvariantCulture,
                        $"{sphere.X:G7} {sphere.Y:G7} {sphere.Z:G7} {sphere.Radius:G7}").Replace(',', '.'));
                }
            }

            await writer.FlushAsync();
        }

        memoryStream.Position = 0;

        return memoryStream;
    }
}