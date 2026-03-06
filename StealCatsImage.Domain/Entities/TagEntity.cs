namespace StealCatsImage.Domain.Entities
{
    public class TagEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public List<CatEntity> Cats { get; set; } = new();
    }
}
