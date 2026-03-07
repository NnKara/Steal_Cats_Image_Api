using Microsoft.Extensions.Logging;
using StealCatsImage.Application.DTOs;
using StealCatsImage.Application.Helpers;
using StealCatsImage.Application.Interfaces.ClientInterfaces;
using StealCatsImage.Application.Interfaces.Repositories;
using StealCatsImage.Application.Interfaces.ServiceInterfaces;
using StealCatsImage.Domain.Entities;

namespace StealCatsImage.Application.Services;

public sealed class CatService : ICatService
{
    private readonly ICatRepository _cats;
    private readonly ITagRepository _tags;
    private readonly ILogger<CatService> _logger;
    private readonly ICatApiClient _catApi;

    public CatService(ICatRepository cats,ITagRepository tags,ICatApiClient catApi,ILogger<CatService> logger)
    {
        _cats = cats;
        _tags = tags;
        _catApi = catApi;
        _logger = logger;
    }

    public async Task<int> FetchCatsAsync(int limit = 25, CancellationToken ct = default)
    {
        try
        {
            var apiCats = await _catApi.GetCatsAsync(limit, ct);

            var filteredCats = apiCats.Where(c => !string.IsNullOrEmpty(c.CatId)).ToList();
            var existingIds = await _cats.GetExistingCatIdsAsync(filteredCats.Select(c => c.CatId).ToList(), ct);

            var newCats = new List<CatEntity>();
            var tagCache = new Dictionary<string, TagEntity>(StringComparer.OrdinalIgnoreCase);

            foreach (var cat in filteredCats)
            {
                if (existingIds.Contains(cat.CatId))
                    continue;

                var entity = new CatEntity
                {
                    CatId = cat.CatId,
                    Width = cat.Width,
                    Height = cat.Height,
                    ImageUrl = cat.ImageUrl,
                    Created = DateTime.UtcNow
                };

                if (!string.IsNullOrWhiteSpace(cat.Temperament))
                {
                    foreach (var raw in cat.Temperament.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var tagName = raw.Trim();
                        if (string.IsNullOrWhiteSpace(tagName))
                            continue;

                        if (!tagCache.TryGetValue(tagName, out var tag))
                        {
                            tag = await _tags.GetByNameAsync(tagName, ct);

                            if (tag == null)
                            {
                                tag = new TagEntity
                                {
                                    Name = tagName,
                                    Created = DateTime.UtcNow
                                };

                                await _tags.AddAsync(tag, ct);
                            }

                            tagCache[tagName] = tag;
                        }

                        entity.Tags.Add(tag);
                    }
                }

                newCats.Add(entity);
            }

            if (newCats.Count > 0)
                await _cats.AddRangeAsync(newCats, ct);

            _logger.LogInformation("Fetched cats from TheCatAPI. Added {Count} new cats", newCats.Count);
            return newCats.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching cats from TheCatAPI");
            throw;
        }
    }

    public async Task<CatDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        try
        {
            var entity = await _cats.GetByIdAsync(id, ct);
            return entity is null ? null : ModelMapperDto.Map(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting cat by id {Id}", id);
            throw;
        }
    }

    public async Task<(List<CatDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
    {
        try
        {
            var result = await _cats.GetPagedAsync(page, pageSize, ct);

            var items = result.Items;
            var total = result.TotalCount;

            var mappedItems = items
            .Select(ModelMapperDto.Map)
            .ToList();

            return (mappedItems, total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting paged cats (page={Page}, pageSize={PageSize})", page, pageSize);
            throw;
        }
    }

    public async Task<(List<CatDto> Items, int TotalCount)> GetPagedByTagAsync(string tag, int page, int pageSize, CancellationToken ct = default)
    {
        try
        {
            var (items, total) = await _cats.GetPagedByTagAsync(tag, page, pageSize, ct);
            return (items.Select(ModelMapperDto.Map).ToList(), total);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting cats by tag {Tag} (page={Page}, pageSize={PageSize})", tag, page, pageSize);
            throw;
        }
    }
}