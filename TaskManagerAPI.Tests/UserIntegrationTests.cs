using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace TaskManagerAPI.Tests
{
    public class UserIntegrationTests :IntegrationTestBase, IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public UserIntegrationTests(CustomWebApplicationFactory factory) : base(factory) 
        {
            _client = factory.CreateClient();
        }
        private static object CreateRegisterBody()
        {
            return new
            {
                userName = $"user_{Guid.NewGuid()}",
                password = "Test@12345"
            };
        }

        //Register test
        [Fact]
        public async Task Register_WithValidData_ReturnsSuccess()
        {
            var user = CreateRegisterBody();
            var response = await _client.PostAsJsonAsync(
                "/api/user/register",user, TestContext.Current.CancellationToken);

            Assert.True(response.IsSuccessStatusCode);
        }
        [Fact]
        public async Task Register_WithDuplicateUser_ReturnsError()
        {
            var user = CreateRegisterBody();
            var firstResponse = await _client.PostAsJsonAsync(
                "/api/user/register", user, TestContext.Current.CancellationToken);
            var secondRespose = await _client.PostAsJsonAsync(
                "/api/user/register", user, TestContext.Current.CancellationToken);

            Assert.True(firstResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, secondRespose.StatusCode);
        }
        //Login test
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var user = CreateRegisterBody();
            var registerResponse=  await _client.PostAsJsonAsync(
                "/api/user/register", user, TestContext.Current.CancellationToken);

            Assert.True(registerResponse.IsSuccessStatusCode);

            var loginResponse = await _client.PostAsJsonAsync(
                "/api/user/login", user, TestContext.Current.CancellationToken);

            Assert.True(loginResponse.IsSuccessStatusCode);

            var content = await loginResponse.Content.ReadAsStringAsync(
                TestContext.Current.CancellationToken);

            Assert.False(string.IsNullOrWhiteSpace(content));
        }
        [Fact]
        public async Task Login_WithWrongPassword_ReturnsUnauthorized()
        {
            var userName = $"_User{Guid.NewGuid()}";
            var registerResponse = await _client.PostAsJsonAsync(
                "/api/user/register", 
                new {userName,password="Test123456"},
                TestContext.Current.CancellationToken);

            Assert.True(registerResponse.IsSuccessStatusCode);
         
            var loginResponse = await _client.PostAsJsonAsync(
              "/api/user/login",
              new {userName,password="987456"},
              TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized,loginResponse.StatusCode);

        }
        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
        {
           
            var loginResponse = await _client.PostAsJsonAsync(
              "/api/user/login", 
              new { userName = $"nobody_{Guid.NewGuid()}", password = "Test123456" },
              TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);

        }
       
       
        //Change password test
        [Fact]
        public async Task ChangePassword_WithValidData_ReturnsSuccess()
        {
            var userName = $"_user{Guid.NewGuid()}";
            var token = await RegisterAndLoginAsync(userName , "Current123456");
            _client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
                "/api/user/me", new { CurrentPassword = "Current123456", NewPassword = "New123456" },
                TestContext.Current.CancellationToken);

            Assert.True(response.IsSuccessStatusCode);
           
            _client.DefaultRequestHeaders.Authorization = null;
            var loginResponse = await _client.PostAsJsonAsync(
                "/api/user/login",
                new { userName, password = "New123456" },
                TestContext.Current.CancellationToken);

            Assert.True(loginResponse.IsSuccessStatusCode);

        }
        [Fact]
        public async Task ChangePassword_WithWrongCurrentPassword_ReturnsError()
        {
            var userName = $"_user{Guid.NewGuid()}";
            var token = await RegisterAndLoginAsync(userName, "Current123456");
            _client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PutAsJsonAsync(
               "/api/user/me", new { CurrentPassword = "WrongPass123456", NewPassword = "New123456" },
               TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
           
        }
        [Fact]
        public async Task ChangePassword_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.PutAsJsonAsync(
              "/api/user/me", new { CurrentPassword = "Test123456", NewPassword = "New123456" },
              TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        //Delete user test
        [Fact]
        public async Task DeleteUser_WithValidToken_ReturnsSuccess()
        {
            var userName = $"_user{Guid.NewGuid()}";
            var password = "Test123456";
            var token = await RegisterAndLoginAsync(userName, password);
            _client.DefaultRequestHeaders.Authorization =
                 new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync(
                "/api/user/me", TestContext.Current.CancellationToken);

            Assert.True(response.IsSuccessStatusCode);

            _client.DefaultRequestHeaders.Authorization = null;
            var loginResponse = await _client.PostAsJsonAsync("/api/user/login",
                new { userName, password },
                TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
        }
        [Fact]
        public async Task DeleteUser_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync(
              "/api/user/me", TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

    }
}
