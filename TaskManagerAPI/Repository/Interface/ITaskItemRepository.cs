using TaskManagerAPI.Dtos;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Repository.Interface
{
    public interface ITaskItemRepository
    {
        public Task<List<TaskItem>> GetAllByUserIdAsync(int userId);
        public Task<TaskItem?> GetByIdAsync(int taskId, int userId);
        Task CreateAsync(TaskItem taskItem);
        Task RemoveAsync(TaskItem taskItem);
        Task UpdateAsync(TaskItem taskItem);

    }
}
