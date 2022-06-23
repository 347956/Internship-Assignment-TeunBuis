using Microsoft.Azure.ServiceBus;

namespace Internship_Assignment_Teunbuis.Services
{
    public class MessageQueue
    {
        private readonly IConfiguration config;

        public MessageQueue(IConfiguration config)
        {
            this.config = config;
        }
        public async Task SendMessageAsync<T>(T ServiceBusMessage, string queueName)
        {
            var queueClient = new QueueClient(config.GetConnectionString("AzureServiceBus"), queueName);
        }
    }
}
