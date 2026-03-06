using StealCatsImage.Application.DTOs;
using StealCatsImage.Application.Interfaces.ClientInterfaces;
using StealCatsImage.Infrastructure.Clients.Models;
using System.Net.Http.Json;

namespace StealCatsImage.Infrastructure.Clients.CatApiClient
{
    public class CatApiClient : ICatApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CatApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<CatApiImageDto>> GetCatsAsync(int limit, CancellationToken ct = default)
        {
            var http = _httpClientFactory.CreateClient("TheCatApi");

            // has_breeds=1 για να παίρνουμε temperament
            var url = $"images/search?limit={limit}&has_breeds=1";

            var response = await http.GetFromJsonAsync<List<CatApiImageResponse>>(url, ct) ?? new List<CatApiImageResponse>();

            return response.Select(x => new CatApiImageDto
            {
                CatId = x.Id,
                ImageUrl = x.Url,
                Width = x.Width,
                Height = x.Height,
                Temperament = x.Breeds?.FirstOrDefault()?.Temperament
            }).ToList();
        }
    }
}
