using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using TaskManagerAPI.Dtos;

namespace TaskManagerAPI.Tests
{
    public class TaskItemIntegrationTests(CustomWebApplicationFactory factory) : IntegrationTestBase(factory),
        IClassFixture<CustomWebApplicationFactory>
    {
      
        [Fact]
        public async Task CreateTask_WithValidToken_ReturnsCreated()
        {
            await AuthenticateAsync($"_user{Guid.NewGuid()}");

            var dto = new CreateTaskDto
            {
                Title = "Task1",
                Description = "A",
                IsDone = false
            };
            var respnose = await _client.PostAsJsonAsync("/api/TaskItem",dto 
            ,
                TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, respnose.StatusCode);
               
        }
        [Fact]
        public async Task GetTasks_WithoutToken_ReturnsUnauthorized()
        {
           
            var response = await _client.GetAsync("/api/TaskItem",
                TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        }
        [Fact]
        public async Task CreateTask_ThenGetTasks_ReturnsTheCreatedTask()
        {
            await AuthenticateAsync($"_user{Guid.NewGuid()}");

          
            var dto = new CreateTaskDto
            {
                Title = "Task1",
                Description = "A",
                IsDone = false
            };
            var createResponse = await _client.PostAsJsonAsync("/api/TaskItem", dto,
                TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

           
            var getResponse = await _client.GetAsync("/api/TaskItem", TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskItemRepositoryDto>>(
                TestContext.Current.CancellationToken);

            Assert.NotNull(tasks);
            Assert.Contains(tasks, t => t.Title == "Task1");
        }
        [Fact]
        public async Task GetTasks_UserB_CannotSeeUserATasks()
        {
           await AuthenticateAsync($"_userA{Guid.NewGuid()}");
            var dto = new CreateTaskDto
            {
                Title = "UserA Task",
                Description = "A",
                IsDone = false
            };
            var createResponse = await _client.PostAsJsonAsync("/api/TaskItem", dto,
                TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            await AuthenticateAsync($"userB{Guid.NewGuid()}");
            await AuthenticateAsync($"userB{Guid.NewGuid()}");

            var getResponse = await _client.GetAsync("/api/TaskItem",
                TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskItemRepositoryDto>>(
                TestContext.Current.CancellationToken);

            Assert.NotNull(tasks);
            Assert.DoesNotContain(tasks, t => t.Title == "UserA Task");

        }

    }
}
