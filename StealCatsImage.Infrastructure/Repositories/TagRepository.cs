using Microsoft.EntityFrameworkCore;
using StealCatsImage.Application.Interfaces.Repositories;
using StealCatsImage.Domain.Entities;
using StealCatsImage.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCatsImage.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {

        private readonly AppDbContext _db;

        public TagRepository(AppDbContext db)
        {
            _db = db;
        }


        public async Task AddAsync(TagEntity tag, CancellationToken ct = default)
        {
            await _db.Tags.AddAsync(tag, ct);
        }

        public async Task<TagEntity?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            var normalized = name.Trim().ToLower();

            return await _db.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == normalized, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            return  _db.SaveChangesAsync(ct);
        }
    }
}
