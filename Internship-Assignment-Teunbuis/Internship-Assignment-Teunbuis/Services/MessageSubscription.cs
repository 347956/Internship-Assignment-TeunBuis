using DAL.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data.Entity.Validation;

namespace Internship_Assignment_Teunbuis.Services
{
    public class MessageSubscription : IMessageSubscription
    {
        private readonly IConfiguration config;

        public MessageSubscription( IConfiguration configuration)
        {
            this.config = configuration;
        }
        public async Task RecieveMessageAsync(string newmessagequeue)
        {
            //sets the client
            var queueClient = new QueueClient(config.GetConnectionString("AzureServiceBus"), newmessagequeue, ReceiveMode.PeekLock, RetryPolicy.Default);

            //registers to the servicebus
            queueClient.RegisterMessageHandler((message, cancelToken) =>
            {
                var bodyButes = message.Body;
                var ourMessage = System.Text.Encoding.UTF8.GetString(bodyButes);

                Console.WriteLine($"Message recieved:[ {ourMessage} ]");
                return Task.CompletedTask;

            },
                (exceptionArg) =>
                {
                    Console.WriteLine($"Exception: [ {exceptionArg.Exception.ToString} ]");
                    return Task.CompletedTask;
                });
        }
    }
}
