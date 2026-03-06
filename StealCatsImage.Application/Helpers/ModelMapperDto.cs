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
}