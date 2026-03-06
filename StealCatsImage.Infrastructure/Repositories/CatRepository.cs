using Microsoft.EntityFrameworkCore;
using StealCatsImage.Application.Interfaces.Repositories;
using StealCatsImage.Domain.Entities;
using StealCatsImage.Infrastructure.Data;

namespace StealCatsImage.Infrastructure.Repositories
{
    public class CatRepository : ICatRepository
    {

        private readonly AppDbContext _db;

        public CatRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddRangeAsync(List<CatEntity> cats, CancellationToken ct = default)
        {
            await _db.Cats.AddRangeAsync(cats, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsByCatIdAsync(string catId, CancellationToken ct = default)
        {
            return  await _db.Cats.AnyAsync(x => x.CatId == catId, ct);
        }

        public async Task<HashSet<string>> GetExistingCatIdsAsync(List<string> catIds, CancellationToken ct = default)
        {
            var list = catIds.ToList();
            if (list.Count == 0) return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var existing = await _db.Cats.Where(c => list.Contains(c.CatId)).Select(c => c.CatId).ToListAsync(ct);
            return existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public async Task<CatEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Cats.AsNoTracking().Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<(List<CatEntity> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var query = _db.Cats.AsNoTracking().Include(x => x.Tags).OrderByDescending(x => x.Created);

            var total = await query.CountAsync(ct);

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return (items, total);
        }

        public async Task<(List<CatEntity> Items, int TotalCount)> GetPagedByTagAsync(string tag, int page, int pageSize, CancellationToken ct = default)
        {
            var normalized = tag.Trim().ToLower();

            var query = _db.Cats.AsNoTracking().Include(x => x.Tags).Where(c => c.Tags.Any(t => t.Name.ToLower() == normalized))
                        .OrderByDescending(x => x.Created);
             
            var total = await query.CountAsync(ct);

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

            return (items, total);
        }
    }
}
