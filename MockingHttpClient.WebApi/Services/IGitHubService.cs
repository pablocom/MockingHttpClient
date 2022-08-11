using MockingHttpClient.WebApi.Models;

namespace MockingHttpClient.WebApi.Services
{
    public interface IGitHubService
    {
        Task<GitHubUserInfo> GetUserInfoAsync(string username);
    }
}
