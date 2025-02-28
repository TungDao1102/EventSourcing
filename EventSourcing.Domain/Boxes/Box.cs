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
                    ApplyBeerBottleAdded(beerBottleAdded);
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

    // command
    public record CreateBox(Guid BoxId, int DesiredNumberOfSpots);


    //event
    public record BoxCreated(int ActualNumberOfSpots);
    public record ShippingLabelAdded(ShippingLabel Label);
    public record ShippingLabelFailedToAdd(ShippingLabelFailedToAdd.FailReason Reason)
    {
        public enum FailReason
        {
            TrackingCodeInvalid
        }
    }

    public enum Carrier
    {
        UPS,
        FedEx,
        BPost
    }

    public record ShippingLabel(Carrier Carrier, string TrackingCode)
    {
        public bool IsValid()
        {
            return Carrier switch
            {
                Carrier.UPS => TrackingCode.StartsWith("ABC"),
                Carrier.FedEx => TrackingCode.StartsWith("DEF"),
                Carrier.BPost => TrackingCode.StartsWith("GHI"),
                _ => throw new ArgumentOutOfRangeException(nameof(Carrier), Carrier, null)
            };
        }
    }

    public record BoxCapacity(int NumberOfSpots)
    {
        public static BoxCapacity Create(int desiredNumberOfSpots)
        {
            return desiredNumberOfSpots switch
            {
                <= 6 => new BoxCapacity(6),
                <= 12 => new BoxCapacity(12),
                _ => new BoxCapacity(24),
            };
        }
    }
}
