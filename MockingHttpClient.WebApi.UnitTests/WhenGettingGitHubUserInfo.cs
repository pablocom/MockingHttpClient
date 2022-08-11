using FluentAssertions;
using MockingHttpClient.WebApi.Services;
using Moq.Protected;
using System.Net;

namespace MockingHttpClient.WebApi.UnitTests;

public class WhenGettingGitHubUserInfo
{
    private readonly GitHubService githubService;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<HttpMessageHandler> _handlerMock = new();

    public WhenGettingGitHubUserInfo()
    {
        githubService = new GitHubService(_httpClientFactoryMock.Object);
    }

    [Fact]
    public async Task ExceptionIsThrownOnNotSuccessfulStatusCode()
    {
        const string username = "pablocom";

        var response = new HttpResponseMessage
        {
            Content = new StringContent(
                $@"{{
                    ""message"": ""something""
                }}"),
            StatusCode = HttpStatusCode.Forbidden,
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri == new Uri($"https://api.github.com/users/{username}")),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        _httpClientFactoryMock.Setup(x => x.CreateClient("GitHub"))
            .Returns(new HttpClient(_handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.github.com")
            });

        var resultAction = () => githubService.GetUserInfoAsync(username);

        var exception = (await resultAction.Should().ThrowAsync<HttpRequestException>()).And.Message.Should().Be("something");
    }
}