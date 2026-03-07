using Microsoft.Extensions.Logging;
using StealCatsImage.Application.DTOs;
using StealCatsImage.Application.Interfaces.ClientInterfaces;
using StealCatsImage.Infrastructure.Clients.Models;
using System.Net.Http.Json;

namespace StealCatsImage.Infrastructure.Clients.CatApiClient
{
    public class CatApiClient : ICatApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CatApiClient> _logger;

        public CatApiClient(IHttpClientFactory httpClientFactory, ILogger<CatApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<CatApiImageDto>> GetCatsAsync(int limit, CancellationToken ct = default)
        {
            var http = _httpClientFactory.CreateClient("TheCatApi");

            // has_breeds=1 για να παίρνουμε temperament
            var url = $"images/search?limit={limit}&has_breeds=1";

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch cats from TheCatAPI (url={Url}, limit={Limit})", url, limit);
                throw;
            }
        }
    }
}
