using SaxsSpot.NanoSystemService.Application.Extensions;
using SaxsSpot.NanoSystemService.Host.Middlewares;
using SaxsSpot.NanoSystemService.Host.Settings;
using SaxsSpot.NanoSystemService.Kafka.Extensions;
using SaxsSpot.NanoSystemService.Storage.Extensions;
using SaxsSpot.Shared.Authenticator.Extensions;
using SaxsSpot.Shared.ProgressTrackerClient.Extensions;


var builder = WebApplication.CreateBuilder(args);
Console.WriteLine($"APP_ENV: {Environment.GetEnvironmentVariable("APP_ENV")}");
builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("APP_ENV") ?? "Development"}.json",
        true, true)
    .AddEnvironmentVariables();

var corsSettings = builder.Configuration.GetSection("Cors").Get<CorsSettings>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", policy =>
    {
        policy.WithOrigins(corsSettings.AllowedOrigins)
            .WithMethods(corsSettings.AllowedMethods)
            .WithHeaders(corsSettings.AllowedHeaders);
        
        if (corsSettings.AllowCredentials)
        {
            policy.AllowCredentials();
        }
        else
        {
            policy.DisallowCredentials();
        }
    });
});

builder.Services
    .AddApplication(builder.Configuration)
    .AddNanoSystemServiceStorage()
    .AddKafkaConsumer(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAuthenticator(builder.Configuration);
builder.Services.AddJobServiceClient(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseCors("MyCorsPolicy");

app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();