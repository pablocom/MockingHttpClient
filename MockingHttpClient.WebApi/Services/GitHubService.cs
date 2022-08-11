using MockingHttpClient.WebApi.Models;
using System.Text.Json.Nodes;

namespace MockingHttpClient.WebApi.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public GitHubService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<GitHubUserInfo> GetUserInfoAsync(string username)
        {
            HttpClient client = httpClientFactory.CreateClient("GitHub");
            var response = await client.GetAsync($"/users/{username}");
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadFromJsonAsync<JsonObject>();
                var message = responseBody!["message"]!.ToString();
                throw new HttpRequestException(message);
            }

            var githubUserInfo = await response.Content.ReadFromJsonAsync<GitHubUserInfo>();
            return githubUserInfo!;
        }
    }
}
