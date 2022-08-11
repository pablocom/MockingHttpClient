using MockingHttpClient.WebApi.Endpoints.Internal;
using MockingHttpClient.WebApi.Models;
using System.Text.Json.Nodes;

namespace MockingHttpClient.WebApi.Endpoints
{
    public class RemoteCallEndpoints : IEndpoints
    {
        public static void DefineEndpoints(IEndpointRouteBuilder app)
        {
            app.MapPost("{username}", GetGitHubUserInfo)
                .WithName(nameof(GetGitHubUserInfo))
                .Produces<GitHubUserInfo>();
        }

        private static async Task<IResult> GetGitHubUserInfo(IHttpClientFactory httpClientFactory, string username)
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
            return Results.Ok(githubUserInfo);
        }
    }
}
