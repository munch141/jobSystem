using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using StackExchange.Redis;
using WebApp;
using WebApp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

var multiplexer = ConnectionMultiplexer.Connect("redis:6379");
var jobQueue = new JobQueue(multiplexer);
await jobQueue.InitializeQueueAsync();

builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton<JobQueue>(jobQueue);

var concurrentWorkersLimit = builder.Configuration.GetValue<int>("ConcurrentWorkersLimit");

for (int i = 0; i < concurrentWorkersLimit; i++) {
    builder.Services.AddSingleton<IHostedService, Worker>();
}

builder.Host.UseSerilog((_, loggerConfiguration) => loggerConfiguration.WriteTo.Console());

var app = builder.Build();

app.UseResponseCompression();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<JobStatusHub>("/jobStatusHub");
app.MapFallbackToPage("/_Host");

app.Run();