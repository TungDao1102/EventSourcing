namespace EventSourcing.Domain
{
    public class CommandRouter(IEventStore eventStore, IServiceProvider serviceProvider)
    {
        public void HandleCommand(object command)
        {
            var commandType = command.GetType();
            var handlerType = typeof(CommandHandler<>).MakeGenericType(commandType);

            var handler = serviceProvider.GetService(handlerType);
            var method = handlerType.GetMethod("Handle");
            method?.Invoke(handler, [command]);

            eventStore.SaveChanges();
        }
    }
}
