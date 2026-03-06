using StealCatsImage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCatsImage.Application.Interfaces.Repositories
{
    public interface ITagRepository
    {
        Task<TagEntity?> GetByNameAsync(string name, CancellationToken ct = default);
        Task AddAsync(TagEntity tag, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
