using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repository.Interface;

namespace TaskManagerAPI.Repository
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly AppDBContext _dbContext;

        public TaskItemRepository(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateAsync(TaskItem taskItem)
        {
            _dbContext.TaskItems.Add(taskItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<TaskItem>> GetAllByUserIdAsync(int userId)
        {
           var tasks=await _dbContext.TaskItems
                .AsNoTracking()
                .Where(t => t.UserId == userId).ToListAsync();
            return tasks;
        }

        
        public async Task<TaskItem?> GetByIdAsync(int id, int userId)
        {
          return await _dbContext.TaskItems
                .FirstOrDefaultAsync(t =>  t.UserId == userId && t.Id == id);
          
        }

        public async Task RemoveAsync(TaskItem taskItem)
        {

            _dbContext.TaskItems.Remove(taskItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaskItem taskItem)
        {
           _dbContext.TaskItems.Update(taskItem);
           
           await _dbContext.SaveChangesAsync();
        }
    }
}
