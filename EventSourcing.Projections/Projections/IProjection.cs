using EventSourcing.Projections.Database.Repositories;

namespace EventSourcing.Projections.Projections
{
    public interface IProjection
    {
        List<Type> RelevantEventTypes { get; }
        int BatchSize { get; }
        int WaitTime { get; }
        void Project(IEnumerable<StoredEventWithVersion> events);
    }
}
