namespace api_app.Services
{
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using api_app.Models;
    using Newtonsoft.Json;
    using System.Text;

    public class QueueService : IQueueService
    {
        private IQueueClient queueClient;
        
        public QueueService(string ServiceBusConnectionString, string QueueName)
        {
            this.queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
        }

        public async Task SendMessage(TaskModel message)
        {
            string messageBody = JsonConvert.SerializeObject(message);
            await this.queueClient.SendAsync(new Message(Encoding.UTF8.GetBytes(messageBody)));
        }
    }
}