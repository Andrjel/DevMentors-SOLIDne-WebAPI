using System.Net;
using Shouldly;

namespace MySpot.Tests.Integration.Endpoints;

public class HomeApiTests : EndpointsTests
{
    public HomeApiTests(OptionsProvider optionsProvider)
        : base(optionsProvider) { }

    [Fact]
    public async Task get_base_endpoint_should_return_200_ok_status_code_and_api_name()
    {
        // Arrange

        // Act
        var response = await Client.GetAsync("/");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldBe("\"Welcome to MySpot API [test]!\"");
    }
}
