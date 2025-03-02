namespace EventSourcing.Domain.Boxes.Commands
{
    public record CreateBox(Guid BoxId, int DesiredNumberOfSpots);

    public class CreateBoxHandler
    {
    }
}
