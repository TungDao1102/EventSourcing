using System.Reflection;
using System.Text.Json;
using EventSourcing.Domain;

namespace EventSourcing.EventStore
{
    public record DatabaseEvent
    {
        private static readonly Assembly DomainAssembly = typeof(CommandRouter).Assembly;
        public Guid AggregateId { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime Timestamp { get; set; }
        public string? EventTypeName { get; set; }
        public string? EventBody { get; set; }
        public byte[]? RowVersion { get; set; }

        public static DatabaseEvent FromStoredEvent(StoredEvent storedEvent)
        {
            string typeName = storedEvent.EventData.GetType().FullName ?? throw new Exception("Could not get type name from EventData");

            return new DatabaseEvent
            {
                AggregateId = storedEvent.AggregateId,
                SequenceNumber = storedEvent.SequenceNumber,
                Timestamp = storedEvent.Timestamp,
                EventTypeName = typeName,
                EventBody = JsonSerializer.Serialize(storedEvent.EventData)
            };
        }

        public StoredEvent ToStoredEvent()
        {
            if (EventTypeName == null)
                throw new Exception("EventTypeName should not be null");
            if (EventBody == null)
                throw new Exception("EventBody should not be null");

            Type eventType = DomainAssembly.GetType(EventTypeName) ?? throw new Exception($"Type not Found: {EventTypeName}");

            object eventData = JsonSerializer.Deserialize(EventBody, eventType) ?? throw new Exception($"Could not deserialize EventBody as {EventTypeName}");

            return new StoredEvent(
           AggregateId,
           SequenceNumber,
           Timestamp,
           eventData);
        }
    }
}
