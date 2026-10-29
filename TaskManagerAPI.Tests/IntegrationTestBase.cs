using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace TaskManagerAPI.Tests
{
    public class IntegrationTestBase
    {
        protected readonly HttpClient _client;

        public IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }
      
        protected async Task<string> RegisterAndLoginAsync(string userName, string password)
        {
            var registerResponse = await _client.PostAsJsonAsync(
               "/api/user/register",
               new { userName, password },
               TestContext.Current.CancellationToken);

            Assert.True(registerResponse.IsSuccessStatusCode);

            var loginResponse = await _client.PostAsJsonAsync(
              "/api/user/login",
              new { userName, password },
              TestContext.Current.CancellationToken);


            var result = await loginResponse.Content.ReadFromJsonAsync<LoginResult>(
                TestContext.Current.CancellationToken);
            return result!.Token;

        }
        protected async Task AuthenticateAsync(string userName)
        {
            var token = await RegisterAndLoginAsync(userName, "Test123456");
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        private record LoginResult(string Token);
    }
}
