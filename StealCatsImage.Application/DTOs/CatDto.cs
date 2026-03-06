namespace StealCatsImage.Application.DTOs
{
    public class CatDto
    {
        public string CatId { get; init; } = null!;
        public int Width { get; init; }
        public int Height { get; init; }
        public string ImageUrl { get; init; } = null!;
        public DateTime Created { get; init; }

        public List<TagDto> Tags { get; init; } = new();
    }
}
