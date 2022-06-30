using Serilog;
using StackExchange.Redis;
using WebApi;
using WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

// add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:8080")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

// add redis and job queue
var multiplexer = ConnectionMultiplexer.Connect(builder.Configuration["RedisUrl"]);
var jobQueue = new JobQueue(multiplexer);
await jobQueue.InitializeQueueAsync();

builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton(jobQueue);

// add workers
var concurrentWorkersLimit = builder.Configuration.GetValue<int>("ConcurrentWorkersLimit");

for (int i = 0; i < concurrentWorkersLimit; i++)
{
    builder.Services.AddSingleton<IHostedService, Worker>();
}

builder.Host.UseSerilog((_, loggerConfiguration) => loggerConfiguration.WriteTo.Console());

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<JobStatusHub>("/jobStatusHub");

if (app.Environment.IsDevelopment())
{
    app.UseCors();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.Run();