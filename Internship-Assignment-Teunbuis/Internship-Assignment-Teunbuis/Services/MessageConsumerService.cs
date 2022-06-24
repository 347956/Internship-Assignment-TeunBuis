using DAL.Models;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace Internship_Assignment_Teunbuis.Services
{
    public class MessageConsumerService : BackgroundService
    {
        private readonly ISubscriptionClient subscriptionClient;

        public MessageConsumerService(ISubscriptionClient subscriptionClient)
        {
            this.subscriptionClient = subscriptionClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            subscriptionClient.RegisterMessageHandler((message, token) =>
            {
                var messageCreated = JsonConvert.DeserializeObject<MessageModel>(Encoding.UTF8.GetString(message.Body));
                Console.WriteLine($"New Message with userName{messageCreated.UserName} at:{messageCreated.Date}");
                return subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            }, new MessageHandlerOptions(args => Task.CompletedTask)
            {
                AutoComplete = false,
                MaxConcurrentCalls = 1
            });
            return Task.CompletedTask;
        }
    }
}
