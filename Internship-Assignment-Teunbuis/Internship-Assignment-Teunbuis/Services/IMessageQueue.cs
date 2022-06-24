namespace Internship_Assignment_Teunbuis.Services
{
    public interface IMessageQueue
    {
        Task SendMessageAsync<T>(T ServiceBusMessage, string queueName);
    }
}