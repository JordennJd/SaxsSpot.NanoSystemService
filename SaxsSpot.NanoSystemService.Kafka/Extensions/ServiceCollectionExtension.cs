using Confluent.Kafka;
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
        var consumerEnabled = configuration.GetValue("kafka:consumerEnabled", true);
        if (!consumerEnabled)
        {
            services.AddMassTransit(x => x.UsingInMemory((_, _) => { }));
            return services;
        }

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
                    Console.WriteLine($"brokers: {string.Join(',', brokers)}");
                    var group = configuration["kafka:group"] ?? "run-generation-consumer-group";
                    var topic = configuration["kafka:topic"] ?? "run-generation-queue";
                    
                    k.Host(brokers);
                    
                    k.TopicEndpoint<RunGenerationRequest>(
                        topic,
                        group,
                        e =>
                        {
                            e.AutoOffsetReset = AutoOffsetReset.Earliest;
                            // When true (default), offset is stored when message is delivered, so on shutdown it gets checkpointed even if we threw (e.g. cancel). Set false so offset is only committed after successful processing; on cancel/exception the message will be redelivered.
                            e.EnableAutoOffsetStore = false;
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
