using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Models;
using TaskManagerAPI.Dtos;
using TaskManagerAPI.Data;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskManagerAPI.Repository.Interface;


namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskItemController : ControllerBase
    {
        private readonly ITaskItemRepository _taskItemRepository;

        public TaskItemController(ITaskItemRepository taskItemRepository)
        {
            _taskItemRepository = taskItemRepository;

        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var userId = GetUserId();
            var tasks = await _taskItemRepository.GetAllByUserIdAsync(userId);
            return Ok(tasks);
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto taskItem)
        {
            var task = new TaskItem
            {
                Title = taskItem.Title,
                Description = taskItem.Description,
                IsDone = taskItem.IsDone,
                UserId = GetUserId()
            };
            await _taskItemRepository.CreateAsync(task);
            return Ok(task);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto dto   )
        {
            var taskItem = await _taskItemRepository.GetByIdAsync(id, GetUserId());
            if (taskItem == null)
            {
                return NotFound();

            }
            taskItem.Title = dto.Title;
            taskItem.Description = dto.Description;
            taskItem.IsDone = dto.IsDone;

            await _taskItemRepository.UpdateAsync(taskItem);
            return Ok(taskItem);

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _taskItemRepository.GetByIdAsync(id, GetUserId());
            if (task == null)
            {
                return NotFound();
            }
            await _taskItemRepository.RemoveAsync(task);
            return Ok(task);
        }
    }
}
