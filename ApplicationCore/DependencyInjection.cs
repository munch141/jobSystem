using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ApplicationCore
{
    public static class DependencyInjection
    {
        public static void AddApplicationCore(this IServiceCollection services)
        {
            var multiplexer = ConnectionMultiplexer.Connect("redis:6379");
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddSingleton(new JobQueue(multiplexer));
        }
    }
}
