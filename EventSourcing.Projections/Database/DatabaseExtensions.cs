﻿using EventSourcing.Projections.Database.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Projections.Database
{
    public static class DatabaseExtensions
    {
        public static void RegisterDataConnections(this IServiceCollection services)
        {
            services.AddSingleton<EventStoreConnectionFactory>();
            services.AddTransient<EventStoreRepository>();

            services.AddScoped<ReadStoreConnection>();

            services.AddScoped<CheckpointRepository>();
            services.AddScoped<OpenBoxRepository>();
            services.AddScoped<UnsentBoxRepository>();
        }
    }
}
