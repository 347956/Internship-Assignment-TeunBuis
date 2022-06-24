using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Internship_Assignment_Teunbuis.Services
{
    public class MessageQueue : IMessageQueue
    {
        private readonly IConfiguration config;

        public MessageQueue(IConfiguration config)
        {
            this.config = config;
        }
        public async Task SendMessageAsync<T>(T ServiceBusMessage, string newmessagequeue)
        {
            var queueClient = new QueueClient(config.GetConnectionString("AzureServiceBus"), newmessagequeue);
            string messageBody = JsonSerializer.Serialize(ServiceBusMessage);
            var message = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(message);
        }
    }
}
