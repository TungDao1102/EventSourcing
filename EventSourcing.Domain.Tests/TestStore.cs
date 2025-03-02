
namespace EventSourcing.Domain.Tests
{
    public class TestStore : IEventStore
    {
        /// <summary>
        /// Add any events that have happened before to this collection.
        /// </summary>
        public readonly List<StoredEvent> PreviousEvents = [];

        /// <summary>
        /// Use this collection to verify which events have been raised.
        /// </summary>
        public readonly List<StoredEvent> NewEvents = [];

        public void AppendEvent(StoredEvent @event)
        {
            NewEvents.Add(@event);
        }

        public IEnumerable<StoredEvent> GetEvents(Guid aggregateId)
        {
            return [.. PreviousEvents.Where(e => e.AggregateId == aggregateId)];
        }

        public IEnumerable<StoredEvent> GetEventsUntilSequence(Guid aggregateId, int sequence)
        {
            return [.. PreviousEvents.Where(x => x.AggregateId == aggregateId && x.SequenceNumber <= sequence)];
        }

        /// <summary>
        /// Not used in command handle tests
        /// </summary>
        public void SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
