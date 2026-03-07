using System.Net;
using System.Net.Http.Json;
using StealCatsImage.Domain.Entities;

namespace StealCatsImage.Tests.Integration;

public sealed class CatsEndpointTests : IClassFixture<CatImageApiTestFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CatImageApiTestFactory<Program> _factory;

    public CatsEndpointTests(CatImageApiTestFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCats_ReturnsOk_WithPagedResult()
    {
        var response = await _client.GetAsync("/api/cats?page=1&pageSize=10");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<GetCatsResponse>();
        Assert.NotNull(json);
        Assert.Equal(1, json!.Page);
        Assert.Equal(10, json.PageSize);
        Assert.NotNull(json.Items);
    }

    [Fact]
    public async Task GetCats_InvalidPage_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/cats?page=0&pageSize=10");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCats_InvalidPageSize_ReturnsBadRequest()
    {
        var response = await _client.GetAsync("/api/cats?page=1&pageSize=0");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_WhenCatExists_ReturnsOk()
    {
        var id = await SeedCatAsync();

        var response = await _client.GetAsync($"/api/cats/{id}");
        response.EnsureSuccessStatusCode();

        var cat = await response.Content.ReadFromJsonAsync<CatResponse>();
        Assert.NotNull(cat);
        Assert.Equal("seed-cat-1", cat!.CatId);
        Assert.Equal("https://example.com/seed.jpg", cat.ImageUrl);
    }

    [Fact]
    public async Task GetById_WhenCatNotFound_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/cats/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Fetch_LimitOutOfRange_ReturnsBadRequest()
    {
        var response = await _client.PostAsync("/api/cats/fetch?limit=10", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<int> SeedCatAsync()
    {
        var cat = new CatEntity
        {
            CatId = "seed-cat-1",
            Width = 100,
            Height = 200,
            ImageUrl = "https://example.com/seed.jpg",
            Created = DateTime.UtcNow
        };
        return await _factory.SeedCatAsync(cat);
    }
}



public class GetCatsResponse
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public List<CatResponse> Items { get; set; } = [];
}

public class CatResponse
{
    public string CatId { get; set; } = string.Empty;
    public int Width { get; set; }
    public int Height { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public List<TagResponse> Tags { get; set; } = [];
}

public class TagResponse
{
    public string Name { get; init; } = string.Empty;
}
