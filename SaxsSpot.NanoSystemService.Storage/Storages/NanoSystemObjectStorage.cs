using System.Globalization;
using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.CommonObjectStorage.Engine;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemService.Application.Interfaces;

namespace SaxsSpot.NanoSystemService.Storage.Storages;

public class NanoSystemObjectStorage(IConfiguration configuration)
    : CommonObjectStorage<Particle>(configuration), INanoSystemObjectStorage
{
    protected override Stream GetStream(IEnumerable<Particle> data)
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);

        foreach (var particle in data)
        {
            streamWriter.WriteLine(particle.ToString());
        }

        streamWriter.Flush();
        return stream;
    }

    protected override IEnumerable<Particle> FromStream(Stream data)
    {
        using var streamReader = new StreamReader(data, leaveOpen: true);
        
        string? str;
        while ((str = streamReader.ReadLine()) != null)
        {
            string CultureName = Thread.CurrentThread.CurrentCulture.Name;
            CultureInfo ci = new CultureInfo(CultureName);
            //TODO
            var splitted = str.Split(" ");
            
            switch (splitted.Length)
            {
                case 4: //Sphere
                    yield return new Sphere(float.Parse(splitted[0], CultureInfo.InvariantCulture),
                        float.Parse(splitted[1], CultureInfo.InvariantCulture)
                        , float.Parse(splitted[2], CultureInfo.InvariantCulture),
                        float.Parse(splitted[3], CultureInfo.InvariantCulture));
                    break;
                case 8: //Parallelepiped
                    yield return new Parallelepiped(float.Parse(splitted[0], CultureInfo.InvariantCulture),
                        float.Parse(splitted[1], CultureInfo.InvariantCulture),
                        float.Parse(splitted[2], CultureInfo.InvariantCulture),
                        float.Parse(splitted[3], CultureInfo.InvariantCulture),
                        float.Parse(splitted[4], CultureInfo.InvariantCulture),
                        float.Parse(splitted[5], CultureInfo.InvariantCulture),
                        float.Parse(splitted[6], CultureInfo.InvariantCulture)
                        , float.Parse(splitted[7], CultureInfo.InvariantCulture));
                    break;
            }
        }
    }
}