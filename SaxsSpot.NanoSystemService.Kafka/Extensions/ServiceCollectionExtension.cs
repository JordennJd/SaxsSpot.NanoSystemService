using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemService.Contracts.Messages;
using SaxsSpot.NanoSystemService.Kafka.Consumers;

namespace SaxsSpot.NanoSystemService.Kafka.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddKafkaConsumer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<RunGenerationConsumer>();

            x.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });

            x.AddRider(rider =>
            {
                rider.AddConsumer<RunGenerationConsumer>();

                rider.UsingKafka((context, k) =>
                {
                    var brokers = configuration.GetSection("kafka:brokers").Get<string[]>() 
                        ?? new[] { "localhost:29092" };
                    var group = configuration["kafka:group"] ?? "run-generation-consumer-group";
                    var topic = configuration["kafka:topic"] ?? "run-generation-queue";
                    
                    k.Host(brokers);
                    
                    k.TopicEndpoint<RunGenerationRequest>(
                        topic,
                        group,
                        e =>
                        {
                            // Create topic if it doesn't exist with default settings
                            e.CreateIfMissing(t =>
                            {
                                t.NumPartitions = 1;
                                t.ReplicationFactor = 1;
                            });
                            
                            e.ConfigureConsumer<RunGenerationConsumer>(context, c =>
                            {
                                c.ConcurrentMessageLimit = 1;
                            });
                            
                            e.ConcurrentConsumerLimit = 1;
                        });
                });
            });
        });

        return services;
    }
}
