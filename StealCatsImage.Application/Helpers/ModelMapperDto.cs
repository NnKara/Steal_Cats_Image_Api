using StealCatsImage.Application.DTOs;
using StealCatsImage.Domain.Entities;

namespace StealCatsImage.Application.Helpers;

public static class ModelMapperDto
{
    public static CatDto Map(CatEntity cat)
    {
        return new CatDto
        {
            CatId = cat.CatId,
            Width = cat.Width,
            Height = cat.Height,
            ImageUrl = cat.ImageUrl,
            Created = cat.Created,
            Tags = cat.Tags.Select(t => new TagDto{
                Name = t.Name
            }).ToList()
        };
    }

    public static PageCatResponseDto<CatDto> ToPageResponse(List<CatEntity> items, int totalCount, int page, int pageSize, string? tag = null)
    {
        return new PageCatResponseDto<CatDto>
        {
            Tag = tag,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items.ConvertAll(Map)
        };
    }
}