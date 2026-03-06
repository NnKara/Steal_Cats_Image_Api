using StealCatsImage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCatsImage.Application.Interfaces.Repositories
{
    public interface ICatRepository
    {
        Task AddRangeAsync(List<CatEntity> cats, CancellationToken ct = default);

        Task<CatEntity?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<bool> ExistsByCatIdAsync(string catId, CancellationToken ct = default);

        Task<HashSet<string>> GetExistingCatIdsAsync(List<string> catIds, CancellationToken ct = default);

        Task<(List<CatEntity> Items, int TotalCount)> GetPagedAsync(int page,int pageSize,CancellationToken ct = default);

        Task<(List<CatEntity> Items, int TotalCount)> GetPagedByTagAsync(string tag,int page,int pageSize,CancellationToken ct = default);
    }
}
