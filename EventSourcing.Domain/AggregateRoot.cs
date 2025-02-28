namespace EventSourcing.Domain
{
    public abstract class AggregateRoot
    {
        public void ApplyEvent(object @event)
        {
            When(@event);
        }

        protected abstract void When(object @event);
    }
}
