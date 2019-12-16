namespace api_app.Services
{
    using System.Threading.Tasks;
    
    public interface IQueueService
    {
        Task SendMessage(api_app.Models.TaskModel message);
    }
}