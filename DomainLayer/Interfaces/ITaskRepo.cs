using DomainLayer.Entities;


namespace DomainLayer.Interfaces
{
    public interface ITaskRepo
    {
        Task<ResponseFormatter<TaskDetails>> GetTaskByIdAsync(int id);
        Task<ResponseFormatter<List<TaskDetails>>> GetAllTasksAsync();
        Task<ResponseFormatter<List<TaskDetails>>> GetTasksForUserAsync(int userId); 
       
        Task<ResponseFormatter<string>> CreateTaskAsync(TaskDetails task);
        Task<ResponseFormatter<string>> UpdateTaskAsync(TaskDetails task);
        
    }
}
