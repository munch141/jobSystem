using Serilog;
using StackExchange.Redis;
using WebApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var multiplexer = ConnectionMultiplexer.Connect("redis:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(multiplexer);
builder.Services.AddSingleton<JobQueue>(new JobQueue(multiplexer));

var concurrentWorkersLimit = builder.Configuration.GetValue<int>("ConcurrentWorkersLimit");

for (int i = 0; i < concurrentWorkersLimit; i++) {
    builder.Services.AddSingleton<IHostedService, Worker>();
}

builder.Host.UseSerilog((_, loggerConfiguration) => loggerConfiguration.WriteTo.Console());

var app = builder.Build();

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
app.MapFallbackToPage("/_Host");

app.Run();