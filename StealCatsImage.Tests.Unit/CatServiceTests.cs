using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using StealCatsImage.Application.DTOs;
using StealCatsImage.Application.Interfaces.ClientInterfaces;
using StealCatsImage.Application.Interfaces.Repositories;
using StealCatsImage.Application.Services;
using StealCatsImage.Domain.Entities;

namespace StealCatsImage.Tests.Unit;

public class CatServiceTests
{
    [Fact]
    public async Task GetByIdAsync_WhenCatExists_ReturnsCatDto()
    {
        var cat = new CatEntity
        {
            Id = 1,
            CatId = "abc123",
            Width = 100,
            Height = 200,
            ImageUrl = "https://example.com/cat.jpg",
            Created = DateTime.UtcNow
        };

        var mockRepo = new MockCatRepository { CatToReturn = cat };
        var service = CreateService(catRepo: mockRepo);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("abc123", result.CatId);
        Assert.Equal(100, result.Width);
        Assert.Equal(200, result.Height);
        Assert.Equal("https://example.com/cat.jpg", result.ImageUrl);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCatDoesNotExist_ReturnsNull()
    {
        var fakeRepo = new MockCatRepository { CatToReturn = null };
        var service = CreateService(catRepo: fakeRepo);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsMappedItemsAndTotalCount()
    {
        var cats = new List<CatEntity>
        {
            new() { Id = 1, CatId = "cat1", Width = 100, Height = 100, ImageUrl = "url1", Created = DateTime.UtcNow },
            new() { Id = 2, CatId = "cat2", Width = 200, Height = 200, ImageUrl = "url2", Created = DateTime.UtcNow }
        };

        var fakeRepo = new MockCatRepository
        {
            PagedResult = (cats, 2)
        };
        var service = CreateService(catRepo: fakeRepo);

        var (items, total) = await service.GetPagedAsync(page: 1, pageSize: 10);

        Assert.Equal(2, total);
        Assert.Equal(2, items.Count);
        Assert.Equal("cat1", items[0].CatId);
        Assert.Equal("cat2", items[1].CatId);
    }

    [Fact]
    public async Task GetPagedByTagAsync_ReturnsMappedItemsAndTotalCount()
    {
        var cats = new List<CatEntity>
        {
            new() { Id = 1, CatId = "cat1", Width = 100, Height = 100, ImageUrl = "url1", Created = DateTime.UtcNow }
        };

        var fakeRepo = new MockCatRepository
        {
            PagedByTagResult = (cats, 1)
        };
        var service = CreateService(catRepo: fakeRepo);

        var (items, total) = await service.GetPagedByTagAsync("friendly", page: 1, pageSize: 10);

        Assert.Equal(1, total);
        Assert.Single(items);
        Assert.Equal("cat1", items[0].CatId);
    }

    [Fact]
    public async Task FetchCatsAsync_WhenNoExistingCats_AddsNewCats()
    {
        var apiCats = new List<CatApiImageDto>
        {
            new() { CatId = "new1", ImageUrl = "url1", Width = 100, Height = 100, Temperament = "Friendly" }
        };

        var fakeApi = new MockCatApiClient { CatsToReturn = apiCats };
        var fakeCatRepo = new MockCatRepository { ExistingIds = new HashSet<string>() };
        var fakeTagRepo = new MockTagRepository();

        var service = CreateService(catRepo: fakeCatRepo, tagRepo: fakeTagRepo, catApi: fakeApi);

        var count = await service.FetchCatsAsync(limit: 1);

        Assert.Equal(1, count);
        Assert.Single(fakeCatRepo.AddedCats);
        Assert.Equal("new1", fakeCatRepo.AddedCats[0].CatId);
    }

    [Fact]
    public async Task FetchCatsAsync_WhenCatsAlreadyExist_SkipsThem()
    {
        var apiCats = new List<CatApiImageDto>
        {
            new() { CatId = "existing1", ImageUrl = "url1", Width = 100, Height = 100 }
        };

        var fakeApi = new MockCatApiClient { CatsToReturn = apiCats };
        var fakeCatRepo = new MockCatRepository { ExistingIds = new HashSet<string> { "existing1" } };

        var service = CreateService(catRepo: fakeCatRepo, catApi: fakeApi);

        var count = await service.FetchCatsAsync(limit: 1);

        Assert.Equal(0, count);
        Assert.Empty(fakeCatRepo.AddedCats);
    }

    private static CatService CreateService(
        ICatRepository? catRepo = null,
        ITagRepository? tagRepo = null,
        ICatApiClient? catApi = null)
    {
        return new CatService(
            catRepo ?? new MockCatRepository(),
            tagRepo ?? new MockTagRepository(),
            catApi ?? new MockCatApiClient(),
            NullLogger<CatService>.Instance);
    }
}

public class MockCatRepository : ICatRepository
{
    public CatEntity? CatToReturn { get; set; }
    public (List<CatEntity> Items, int TotalCount) PagedResult { get; set; } = ([], 0);
    public (List<CatEntity> Items, int TotalCount) PagedByTagResult { get; set; } = ([], 0);
    public HashSet<string> ExistingIds { get; set; } = [];
    public List<CatEntity> AddedCats { get; } = [];

    public async Task AddRangeAsync(List<CatEntity> cats, CancellationToken ct = default)
    {
        AddedCats.AddRange(cats);
    }

    public async Task<CatEntity?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return CatToReturn;
    }

    public async Task<bool> ExistsByCatIdAsync(string catId, CancellationToken ct = default)
    {
        return false;
    }

    public async Task<HashSet<string>> GetExistingCatIdsAsync(List<string> catIds, CancellationToken ct = default)
    {
        return ExistingIds;
    }

    public async Task<(List<CatEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        return PagedResult;
    }

    public async Task<(List<CatEntity> Items, int TotalCount)> GetPagedByTagAsync(string tag, int page, int pageSize, CancellationToken ct = default)
    {
        return PagedByTagResult;
    }
}

public class MockTagRepository : ITagRepository
{
    public TagEntity? TagToReturn { get; set; }

    public async Task<TagEntity?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return TagToReturn;
    }

    public async Task AddAsync(TagEntity tag, CancellationToken ct = default)
    {
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
    }
}

public class MockCatApiClient : ICatApiClient
{
    public List<CatApiImageDto> CatsToReturn { get; set; } = [];

    public async Task<List<CatApiImageDto>> GetCatsAsync(int limit, CancellationToken ct = default)
    {
        return CatsToReturn;
    }
}
