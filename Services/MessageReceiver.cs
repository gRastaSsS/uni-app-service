namespace api_app.Services
{
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.Cosmos;
    using api_app.Models;
    using Newtonsoft.Json;
    using System.Text;
    using System.Threading;
    using System.IO;
    using System;

    public class MessageReceiver 
    {
        private IQueueClient queueClient;
        private readonly Container container;

        public MessageReceiver(Container container, string ServiceBusConnectionString, string QueueName)
        {
            this.container = container;
            this.queueClient = new QueueClient("Endpoint=sb://uni-service-bus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ycjhIxyZcWke94o8XU+xgw6kHTDk4ZHda/N3jVIifGk=", "uni-result-queue");
            this.RegisterOnMessageHandlerAndReceiveMessages();
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            string s = Encoding.UTF8.GetString(message.Body);
            Result result = JsonConvert.DeserializeObject<Result>(s.Substring(63)); //Магия десериализации..
            Item item = this.container.ReadItemAsync<Item>(result.Id, PartitionKey.None).GetAwaiter().GetResult();
            item.Completed = true;
            item.Output = result.Output;
            container.UpsertItemAsync<Item>(item).Wait();
            
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }
    }
}