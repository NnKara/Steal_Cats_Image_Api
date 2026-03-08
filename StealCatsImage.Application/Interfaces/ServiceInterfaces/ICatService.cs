using StealCatsImage.Application.DTOs;

namespace StealCatsImage.Application.Interfaces.ServiceInterfaces
{
    public interface ICatService
    {
        Task<CatDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<PageCatResponseDto<CatDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<PageCatResponseDto<CatDto>> GetPagedByTagAsync(string tag, int page, int pageSize, CancellationToken ct = default);
        Task<int> FetchCatsAsync(int limit = 25, CancellationToken ct = default);
    }
}
