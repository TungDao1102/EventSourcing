using EventSourcing.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.EventStore
{
    public static class EventStoreExtensions
    {
        public static void RegisterEventStore(this IServiceCollection services)
        {
            services.AddSingleton<EventStoreConnectionFactory>();
            services.AddScoped<IEventStore, EventStore>();
        }
    }
}
