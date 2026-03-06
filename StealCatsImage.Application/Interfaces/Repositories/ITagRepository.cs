using StealCatsImage.Domain.Entities;

namespace StealCatsImage.Application.Interfaces.Repositories
{
    public interface ITagRepository
    {
        Task<TagEntity?> GetByNameAsync(string name, CancellationToken ct = default);
        Task AddAsync(TagEntity tag, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
