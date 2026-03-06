using StealCatsImage.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCatsImage.Application.Interfaces.ServiceInterfaces
{
    public interface ICatService
    {
        Task<CatDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<(List<CatDto> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<(List<CatDto> Items, int TotalCount)> GetPagedByTagAsync(string tag, int page, int pageSize, CancellationToken ct = default);
        Task<int> FetchCatsAsync(int limit = 25, CancellationToken ct = default);
    }
}
