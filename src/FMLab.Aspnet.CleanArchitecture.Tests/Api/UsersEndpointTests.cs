// API - Clean architecture boilerplate
// Copyright (c) 2026 Fagner Marinho
// Licensed under the MIT License. See LICENSE file in the project root for details.

using FMLab.Aspnet.CleanArchitecture.Application.Handlers.CreateUser;
using FMLab.Aspnet.CleanArchitecture.Application.Handlers.UpdateUser;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace FMLab.Aspnet.CleanArchitecture.Tests.Api;

public class UsersEndpointTests
{
    private static async Task<int> CreateUserAsync(HttpClient client, string name, string? email = null)
    {
        var response = await client.PostAsJsonAsync("/users", new { Name = name, Email = email });
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<CreateUserOutputDTO>();
        return dto!.Id;
    }

    [Fact]
    public async Task POST_Users_WithValidData_Returns201Created()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/users", new { Name = "Fagner", Email = "fagner@example.com" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var dto = await response.Content.ReadFromJsonAsync<CreateUserOutputDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Fagner", dto.Name);
        Assert.Equal("fagner@example.com", dto.Email);
        Assert.Equal("Active", dto.Status);
        Assert.True(dto.Id > 0);
    }

    [Fact]
    public async Task POST_Users_WithNullEmail_Returns201Created()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/users", new { Name = "Fagner", Email = (string?)null });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<CreateUserOutputDTO>();
        Assert.Null(dto!.Email);
    }

    [Fact]
    public async Task POST_Users_WithEmptyName_Returns422UnprocessableEntity()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/users", new { Name = "", Email = "fagner@example.com" });

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact (Skip = "In-Memory database doesn't enforce uniqueness")]
    public async Task POST_Users_WithDuplicateName_Returns409Conflict()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        await CreateUserAsync(client, "Fagner", "fagner@example.com");

        // Attempt to create a user with the same name
        var response = await client.PostAsJsonAsync("/users", new { Name = "Fagner", Email = "other@example.com" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GET_Users_WhenEmpty_Returns200WithEmptyList()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(0, json.GetProperty("totalItems").GetInt32());
        Assert.Equal(0, json.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public async Task GET_Users_AfterCreation_ReturnsUsers()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        await CreateUserAsync(client, "Fagner", "fagner@example.com");

        var response = await client.GetAsync("/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(1, json.GetProperty("totalItems").GetInt32());
        Assert.Equal(1, json.GetProperty("items").GetArrayLength());
    }

    [Fact]
    public async Task GET_UsersById_WhenExists_Returns200WithUser()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();
        var id = await CreateUserAsync(client, "Fagner", "fagner@example.com");

        var response = await client.GetAsync($"/users/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(id, json.GetProperty("id").GetInt32());
        Assert.Equal("Fagner", json.GetProperty("name").GetString());
        Assert.Equal("fagner@example.com", json.GetProperty("email").GetString());
    }

    [Fact]
    public async Task GET_UsersById_WhenNotFound_Returns404()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/users/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Deactivate_WhenExists_Returns204()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();
        var id = await CreateUserAsync(client, "Fagner", "fagner@example.com");

        var response = await client.PostAsync($"/users/{id}/deactivate", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task POST_Deactivate_WhenNotFound_Returns404()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsync("/users/999/deactivate", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task POST_Deactivate_WhenAlreadyDeactivated_Returns422()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();
        var id = await CreateUserAsync(client, "Fagner", "fagner@example.com");

        await client.PostAsync($"/users/{id}/deactivate", null);
        var response = await client.PostAsync($"/users/{id}/deactivate", null);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
    }

    [Fact]
    public async Task PUT_Users_WhenExists_Returns200()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();
        var id = await CreateUserAsync(client, "Fagner", "fagner@example.com");

        var response = await client.PutAsJsonAsync($"/users/{id}", new { Name = "Fagner Updated", Email = "updated@example.com" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PUT_Users_WhenNotFound_Returns404()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync("/users/999", new { Name = "Fagner", Email = "fagner@example.com" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PATCH_Users_WhenExists_Returns200WithUpdatedData()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();
        var id = await CreateUserAsync(client, "Fagner", "fagner@example.com");

        var request = new HttpRequestMessage(HttpMethod.Patch, $"/users/{id}")
        {
            Content = JsonContent.Create(new { Name = "Fagner Patched", Email = (string?)null })
        };
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<UpdateUserOutputDTO>();
        Assert.NotNull(dto);
        Assert.Equal("Fagner Patched", dto.Name);
    }

    [Fact]
    public async Task PATCH_Users_WhenNotFound_Returns404()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var request = new HttpRequestMessage(HttpMethod.Patch, "/users/999")
        {
            Content = JsonContent.Create(new { Name = "Fagner", Email = (string?)null })
        };
        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Users_WhenExists_Returns204()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();
        var id = await CreateUserAsync(client, "Fagner", "fagner@example.com");

        var response = await client.DeleteAsync($"/users/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DELETE_Users_WhenNotFound_Returns404()
    {
        using var factory = new ApiTestFactory();
        var client = factory.CreateClient();

        var response = await client.DeleteAsync("/users/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
