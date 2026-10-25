using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Moq;
using System.Security.Claims;
using TaskManagerAPI.Controllers;
using TaskManagerAPI.Dtos;
using TaskManagerAPI.Models;
using TaskManagerAPI.Repository.Interface;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace TaskManagerAPI.Tests
{
    public class TaskItemControllerTests
    {
        private static TaskItemController CreateController(ITaskItemRepository repo, int? userId)
        {
            var controller = new TaskItemController(repo);
            var claims = new List<Claim>();
            if (userId.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString()));
            }
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestTaskItemController"))
                }
            };
            return controller;
        }
        //Get all task test
        [Fact]
        public async Task GetAllTasks_WhenUserIsAuthenticated_ReturnsOkWithTasks()
        {
            var repoMock = new Mock<ITaskItemRepository>();
            var tasks = new List<TaskItem>
            {
            new TaskItem { Id = 1, Title = "Task 1", Description = "A", IsDone = false, UserId = 1 },
            new TaskItem { Id = 2, Title = "Task 2", Description = "B", IsDone = true, UserId = 1 }
            };
            repoMock
                .Setup(r => r.GetAllByUserIdAsync(1))
                .ReturnsAsync(tasks);
            var controller = CreateController(repoMock.Object, 1);
            var result = await controller.GetAllTasks();
            var ok = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<TaskItemRepositoryDto>>(ok.Value);
            Assert.Equal(2, dtos.Count());
            repoMock.Verify(r => r.GetAllByUserIdAsync(1), Times.Once);

        }
        [Fact]
        public async Task GetAllTasks_WhenUserIsNotAuthenticated_ReturnsUnauthorized()
        {
            var repoMock = new Mock<ITaskItemRepository>();
            var controller = CreateController(repoMock.Object, null);
            var result = await controller.GetAllTasks();
            Assert.IsType<UnauthorizedResult>(result);
            repoMock.Verify(r => r.GetAllByUserIdAsync(It.IsAny<int>()), Times.Never());

        }
        //Get task test
        [Fact]
        public async Task GetTask_WhenTaskNotFound_ReturnsNotFound()
        {
            var repoMock = new Mock<ITaskItemRepository>();
            repoMock
                 .Setup(r => r.GetByIdAsync(5, 1))
                 .ReturnsAsync((TaskItem?)null);

            var controller = CreateController(repoMock.Object, 1);
            var result = await controller.GetTask(5);
            Assert.IsType<NotFoundResult>(result);
            repoMock.Verify(r => r.GetByIdAsync(5, 1), Times.Once);
        }
        [Fact]
        public async Task GetTask_WhenTaskExists_ReturnsOk()
        {
            var repoMock = new Mock<ITaskItemRepository>();
            var task = new TaskItem
            {
                Id = 3,
                Title = "Task 1",
                Description = "A",
                IsDone = false,
                UserId = 1
            };

            repoMock
                .Setup(r => r.GetByIdAsync(3, 1))
                .ReturnsAsync(task);
            var controller = CreateController(repoMock.Object, 1);
            var result = await controller.GetTask(3);
            var ok = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TaskItemRepositoryDto>(ok.Value);
            Assert.Equal(3, dto.Id);
            Assert.Equal("Task 1", dto.Title);
            repoMock.Verify(r => r.GetByIdAsync(3, 1), Times.Once);

        }
        //Create task test
        [Fact]
        public async Task CreateTask_WhenValid_ReturnsCreatedAtAction()
        {
            var repoMask = new Mock<ITaskItemRepository>();
            var dto = new CreateTaskDto
            {
                Title = "Task 1",
                Description = "A",
                IsDone = false
            };
            var controller = CreateController(repoMask.Object, 1);
            var result = await controller.CreateTask(dto);
            var created = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<TaskItemRepositoryDto>(created.Value);
            Assert.Equal("Task 1", response.Title);
            Assert.Equal(nameof(TaskItemController.GetTask), created.ActionName);
            repoMask.Verify(r => r.CreateAsync(It.Is<TaskItem>(t =>
                                                t.UserId == 1 && t.Title == "Task 1")), Times.Once);

        }
        //Update task test
        [Fact]
        public async Task UpdateTask_WhenTaskExists_ReturnsNoContent()
        {
            var repoMock = new Mock<ITaskItemRepository>();

            var task = new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                Description = "A",
                IsDone = false
            };
            var dto = new UpdateTaskDto
            {
                Title = "Task 1 updated",
                Description = "B",
                IsDone = true
            };
            repoMock
               .Setup(r => r.GetByIdAsync(1, 1))
               .ReturnsAsync(task);
            var controller = CreateController(repoMock.Object, 1);
            var result = await controller.UpdateTask(1, dto);
            Assert.IsType<NoContentResult>(result);
            Assert.Equal("Task 1 updated", task.Title);
            Assert.Equal("B", task.Description);
            Assert.True(task.IsDone);
            repoMock.Verify(r => r.UpdateAsync(task), Times.Once);
        }
        [Fact]
        public async Task UpdateTask_WhenTaskNotExists_ReturnsNotFound()
        {
            var repoMock = new Mock<ITaskItemRepository>();
            var dto = new UpdateTaskDto
            {
                Title = "Task 1 updated",
                Description = "B",
                IsDone = true
            };
            repoMock
                .Setup(r => r.GetByIdAsync(10, 1))
                .ReturnsAsync((TaskItem?)null);
            var controller = CreateController(repoMock.Object, 1);
            var result = await controller.UpdateTask(10, dto);
            Assert.IsType<NotFoundResult>(result);
            repoMock.Verify(r => r.GetByIdAsync(10, 1), Times.Once);
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<TaskItem>()), Times.Never);

        }
        //Delete task test
        [Fact]
        public async Task DeleteTask_WhenTaskExists_ReturnsNoContent()
        {
            var repoMock = new Mock<ITaskItemRepository>();
            var task = new TaskItem
            {
                Id = 1,
                Title = "Task 1",
                Description = "A",
                IsDone = false
            };
            repoMock
                .Setup(r => r.GetByIdAsync(1, 2))
                .ReturnsAsync(task);
            var controller = CreateController(repoMock.Object, 2);
            var result = await controller.DeleteTask(1);
            Assert.IsType<NoContentResult>(result);
            repoMock.Verify(r => r.GetByIdAsync(1, 2), Times.Once);
            repoMock.Verify(r => r.RemoveAsync(task), Times.Once);

           
        }
        [Fact]
        public async Task DeleteTask_WhenTaskNotFound_ReturnsNotFound()
        {
            var repoMock = new Mock<ITaskItemRepository>();
            repoMock
                .Setup(r => r.GetByIdAsync(2, 1))
                .ReturnsAsync((TaskItem?)null);
            var controller = CreateController(repoMock.Object, 1);
            var result = await controller.DeleteTask(2);
            Assert.IsType<NotFoundResult>(result);
            repoMock.Verify(r => r.GetByIdAsync(2, 1), Times.Once);
            repoMock.Verify(r => r.RemoveAsync(It.IsAny<TaskItem>()), Times.Never);
        }



    }
}

