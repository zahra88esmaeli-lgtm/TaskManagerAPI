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
        private bool TryGetUserId(out int userId) =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

        private static TaskItemRepositoryDto ToResponseDto(TaskItem task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsDone = task.IsDone
        };

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var tasks = await _taskItemRepository.GetAllByUserIdAsync(userId);
            return Ok(tasks.Select(ToResponseDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var task = await _taskItemRepository.GetByIdAsync(id, userId);
            if (task == null) return NotFound();

            return Ok(ToResponseDto(task));
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask(CreateTaskDto taskItem)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();
            var task = new TaskItem
            {
                Title = taskItem.Title,
                Description = taskItem.Description,
                IsDone = taskItem.IsDone,
                UserId = userId
            };
            await _taskItemRepository.CreateAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, ToResponseDto(task));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, UpdateTaskDto dto)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var taskItem = await _taskItemRepository.GetByIdAsync(id, userId);
            if (taskItem == null)
            {
                return NotFound();

            }
            taskItem.Title = dto.Title;
            taskItem.Description = dto.Description;
            taskItem.IsDone = dto.IsDone;

            await _taskItemRepository.UpdateAsync(taskItem);
            return NoContent();

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (!TryGetUserId(out var userId)) return Unauthorized();

            var task = await _taskItemRepository.GetByIdAsync(id, userId);
            if (task == null)
            {
                return NotFound();
            }
            await _taskItemRepository.RemoveAsync(task);
            return NoContent();
        }
    }
}
