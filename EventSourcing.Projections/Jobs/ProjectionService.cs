﻿using EventSourcing.Projections.Database;
using EventSourcing.Projections.Database.Repositories;
using EventSourcing.Projections.Projections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventSourcing.Projections.Jobs
{
    public class ProjectionService<TProjection>(IServiceProvider serviceProvider, EventStoreRepository eventsRepo) : BackgroundService where TProjection : class, IProjection
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var checkpoint = GetCheckpoint();

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();

                var connection = scope.ServiceProvider
                    .GetRequiredService<ReadStoreConnection>();
                using var transaction = connection.GetTransaction();

                var projection = scope.ServiceProvider
                    .GetRequiredService<TProjection>();

                var events = eventsRepo.GetEvents(
                    projection.RelevantEventTypes,
                    checkpoint,
                    projection.BatchSize
                );

                if (events.Count == 0)
                {
                    await Task.Delay(projection.WaitTime, stoppingToken);
                }
                else
                {
                    projection.Project(events);

                    checkpoint = events.Last().RowVersion;
                    var checkpointRepo = scope.ServiceProvider
                        .GetRequiredService<CheckpointRepository>();

                    checkpointRepo.SetCheckpoint(
                        typeof(TProjection).Name,
                        checkpoint);
                }
                transaction.Commit();
            }
        }

        private byte[] GetCheckpoint()
        {
            using var scope = serviceProvider.CreateScope();
            var checkpointService = scope.ServiceProvider
                .GetRequiredService<CheckpointRepository>();

            var checkpoint = checkpointService.GetCheckpoint(
                typeof(TProjection).Name);

            return checkpoint;
        }
    }
}
