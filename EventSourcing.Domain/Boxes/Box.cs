namespace EventSourcing.Domain.Boxes
{
    public class Box : AggregateRoot
    {
        public List<BeerBottle> BeerBottles { get; } = [];
        public BoxCapacity? Capacity { get; private set; }
        public ShippingLabel? ShippingLabel { get; private set; }
        public bool IsClosed { get; private set; }
        public bool IsSent { get; private set; }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case BoxCreated boxCreated:
                    Apply(boxCreated);
                    break;
                case BeerBottleAdded beerBottleAdded:
                    Apply(beerBottleAdded);
                    break;
                case ShippingLabelAdded shippingLabelAdded:
                    Apply(shippingLabelAdded);
                    break;
                case BoxClosed boxClosed:
                    Apply(boxClosed);
                    break;
                case BoxSent boxSent:
                    Apply(boxSent);
                    break;
                default:
                    throw new InvalidOperationException($"Event type {@event.GetType().Name} is not supported");
            }
        }

        public void Apply(BoxCreated @event)
        {
            Capacity = @event.Capacity;
        }

        public void Apply(BeerBottleAdded @event)
        {
            BeerBottles.Add(@event.Bottle);
        }

        public void Apply(ShippingLabelAdded @event)
        {
            ShippingLabel = @event.Label;
        }

        public void Apply(BoxClosed @event)
        {
            IsClosed = true;
        }

        public void Apply(BoxSent @event)
        {
            IsSent = true;
        }

        public bool IsFull => BeerBottles.Count >= Capacity?.NumberOfSpots;
    }
}
