namespace StealCatsImage.Domain.Entities
{
    public class CatEntity
    {
        public int Id { get; set; }
        public string CatId { get; set; } = null!;
        public int Width { get; set; }
        public int Height { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public List<TagEntity> Tags { get; set; } = new();
    }
}
