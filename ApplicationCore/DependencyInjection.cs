using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ApplicationCore
{
    public static class DependencyInjection
    {
        public static void AddApplicationCore(this IServiceCollection services, IConfiguration configuration)
        {
            var multiplexer = ConnectionMultiplexer.Connect("localhost:6379");
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddSingleton(new JobQueue(multiplexer));
        }
    }
}
