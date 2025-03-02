using FluentAssertions;

namespace EventSourcing.Domain.Tests
{
    /// <summary>
    /// Test base class for CommandHandler tests.
    /// </summary>
    /// <typeparam name="TCommand">The command type for the handler.</typeparam>
    public abstract class CommandHandlerTest<TCommand>
    {
        /// <summary>
        /// If no explicit aggregateId is provided, this one will be used behind the scenes.
        /// </summary>
        protected readonly Guid _aggregateId = Guid.NewGuid();

        /// <summary>
        /// The command handler, to be provided in the Test class.
        /// This to account for additional injections
        /// </summary>
        protected abstract CommandHandler<TCommand> Handler { get; }

        /// <summary>
        /// A fake, in-memory event store.
        /// </summary>
        protected TestStore eventStore = new();

        /// <summary>
        /// Sets a list of previous events for the default aggregate ID.
        /// </summary>
        protected void Given(params object[] events)
        {
            Given(_aggregateId, events);
        }

        /// <summary>
        /// Sets a list of previous events for a specified aggregate ID.
        /// </summary>
        protected void Given(Guid aggregateId, params object[] events)
        {
            eventStore.PreviousEvents.AddRange(events.Select((x, y) => new StoredEvent(aggregateId, y, DateTime.Now, x)));
        }

        /// <summary>
        /// Triggers the handling of a command against the configured events.
        /// </summary>

        protected void When(TCommand command)
        {
            Handler.Handle(command);
        }

        protected void Then(params object[] events)
        {
            Then(_aggregateId, events);
        }

        protected void Then(Guid aggregateId, params object[] events)
        {
            var actualEvents = eventStore.NewEvents
                    .Where(x => x.AggregateId == aggregateId)
                    .OrderBy(x => x.SequenceNumber)
                    .Select(x => x.EventData)
                    .ToArray();

            actualEvents.Length.Should().Be(events.Length);

            for (var i = 0; i < actualEvents.Length; i++)
            {
                actualEvents[i].Should().BeOfType(events[i].GetType());
                try
                {
                    actualEvents[i].Should().BeEquivalentTo(events[i]);
                }
                catch (InvalidOperationException e)
                {
                    // Empty event with matching type is OK. This means that the event class
                    // has no properties. If the types match in this situation, the correct
                    // event has been appended. So we should ignore this exception.
                    if (!e.Message.StartsWith("No members were found for comparison."))
                        throw;
                }
            }
        }
    }
}
