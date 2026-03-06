namespace StealCatsImage.Application.DTOs
{
    public class CatApiImageDto
    {
        public string CatId { get; init; } = null!;
        public string ImageUrl { get; init; } = null!;
        public int Width { get; init; }
        public int Height { get; init; }
        public string? Temperament { get; init; }
    }
}
