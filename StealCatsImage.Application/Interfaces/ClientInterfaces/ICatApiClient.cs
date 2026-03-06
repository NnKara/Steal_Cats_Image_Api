using StealCatsImage.Application.DTOs;

namespace StealCatsImage.Application.Interfaces.ClientInterfaces
{
    public interface ICatApiClient
    {
        Task<List<CatApiImageDto>> GetCatsAsync(int limit, CancellationToken ct = default);
    }
}
