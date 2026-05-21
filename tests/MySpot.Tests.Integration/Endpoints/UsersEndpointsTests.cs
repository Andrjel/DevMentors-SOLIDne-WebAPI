using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MySpot.App.Commands;
using MySpot.App.DTO;
using MySpot.Core.Entities;
using MySpot.Core.Repositories;
using MySpot.Core.ValueObjects;
using MySpot.Infrastructure.Security;
using MySpot.Infrastructure.Services;
using Shouldly;

namespace MySpot.Tests.Integration.Endpoints;

public sealed class UsersEndpointsTests : EndpointsTests, IAsyncLifetime
{
    [Fact]
    public async Task post_users_should_return_201_created_status_code()
    {
        // Arrange
        var command = new SignUp(
            Guid.Empty,
            "john.doe@example.com",
            "test-user-1",
            "secret",
            "John Doe",
            Role.User
        );

        // Act
        var response = await Client.PostAsJsonAsync("users/signup", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
    }

    [Fact]
    public async Task post_signin_should_return_200_ok_status_code_and_jwt()
    {
        // Arrange
        var user = await CreateUserAsync();
        var command = new SignIn("john.doe@example.com", Password);

        // Act
        var response = await Client.PostAsJsonAsync("users/signin", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var jwt = await response.Content.ReadFromJsonAsync<JwtDto>();
        jwt.ShouldNotBeNull();
        jwt.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task get_me_should_return_200_ok_status_code_and_user_info()
    {
        // Arrange
        var user = await CreateUserAsync();

        Authorize(user.Id, user.Role);

        // Act
        var userDto = await Client.GetFromJsonAsync<UserDto>("users/me");

        // Assert
        userDto.ShouldNotBeNull();
        userDto.Id.ShouldBe(user.Id.Value);
    }

    private async Task<User> CreateUserAsync()
    {
        var passwordManager = new PasswordManager(new PasswordHasher<User>());
        var clock = new Clock();
        var user = new User(
            Guid.NewGuid(),
            "john.doe@example.com",
            "test-user-1",
            passwordManager.Secure(Password),
            "John Doe",
            Role.User,
            clock.Current.DateTime
        );
        await _userRepository.AddAsync(user);
        // await _testDatabase.DbContext.Users.AddAsync(user);
        // await _testDatabase.DbContext.SaveChangesAsync();
        return user;
    }

    private readonly TestDatabase _testDatabase;
    private const string Password = "secret";
    private IUserRepository _userRepository;

    public UsersEndpointsTests(OptionsProvider optionsProvider)
        : base(optionsProvider)
    {
        _testDatabase = new TestDatabase();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        await _testDatabase.DisposeAsync();
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        _userRepository = new TestUserRepository();
        services.AddSingleton(_userRepository);
    }
}
