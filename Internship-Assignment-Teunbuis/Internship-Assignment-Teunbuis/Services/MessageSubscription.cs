using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Internship_Assignment_Teunbuis.Services
{
    public class MessageSubscription
    {
        private readonly IConfiguration config;

        public MessageSubscription(IConfiguration configuration)
        {
            this.config = configuration;
        }
        public async Task RecieveMessageAsync(string newmessagequeue)
        {
            var queueClient = new QueueClient(config.GetConnectionString("AzureServiceBus"), newmessagequeue, ReceiveMode.PeekLock, RetryPolicy.Default);
        }
    }
}
