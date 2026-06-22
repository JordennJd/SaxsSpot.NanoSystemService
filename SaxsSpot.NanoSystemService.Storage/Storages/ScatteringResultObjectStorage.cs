using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using SaxsSpot.Core.CommonObjectStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Storage.Storages;

public class ScatteringResultObjectStorage(IConfiguration configuration)
    : CommonObjectStorage<IntensityResult>(configuration), IScatteringResultObjectStorage
{
    private readonly IMinioClient _minioClient = CreateMinioClient(configuration);
    private readonly string? _bucketName = configuration.GetSection("minio")["bucketName"];

    private static IMinioClient CreateMinioClient(IConfiguration configuration)
    {
        var minioConfig = configuration.GetSection("minio");
        return new MinioClient()
            .WithEndpoint(minioConfig["endpoint"])
            .WithCredentials(minioConfig["accessKey"], minioConfig["secretKey"])
            .WithSSL(minioConfig.GetValue("useSsl", false))
            .Build();
    }

    public async Task Delete(Guid objectId)
    {
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject($"{objectId}"));
    }
    protected override Stream GetStream(IEnumerable<IntensityResult> data)
    {
        var stream = new MemoryStream();
        using var streamWriter = new StreamWriter(stream, leaveOpen: true);

        foreach (var point in data)
        {
            streamWriter.WriteLine(point.ToString());
        }

        streamWriter.Flush();
        stream.Position = 0;
        return stream;
    }

    protected override async IAsyncEnumerable<IntensityResult> FromStreamAsync(Stream data)
    {
        using var streamReader = new StreamReader(data, leaveOpen: true);

        string? line;
        while ((line = await streamReader.ReadLineAsync()) != null)
        {
            var parts = line.Split(' ');
            yield return new IntensityResult(double.Parse(parts[0]), double.Parse(parts[1]));
        }
    }
}
