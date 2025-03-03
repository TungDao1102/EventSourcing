using EventSourcing.Projections.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace EventSourcing.Projections.Projections
{
    public static class ProjectionExtensions
    {
        public static void RegisterProjections(this IServiceCollection services)
        {
            services.AddTransient<OpenBoxProjection>();
            services.AddHostedService<ProjectionService<OpenBoxProjection>>();

            services.AddTransient<UnsentBoxProjection>();
            services.AddHostedService<ProjectionService<UnsentBoxProjection>>();
        }
    }
}
