using StealCatsImage.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StealCatsImage.Application.Interfaces.ClientInterfaces
{
    public interface ICatApiClient
    {
        Task<List<CatApiImageDto>> GetCatsAsync(int limit, CancellationToken ct = default);
    }
}
