using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace RestClients.Shared.ZackCRM
{
    public interface IZackCRMClient
    {
        Task<string[]> GetAllUserEmailsAsync(CancellationToken cancellationToken);
        Task AddUserAsync(string email, CancellationToken cancellationToken);
    }

    public class ZackCRMClient : IZackCRMClient
    {
        private readonly HttpClient _httpClient;

        public ZackCRMClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task AddUserAsync(string email, CancellationToken cancellationToken)
        {
            return _httpClient.PostAsync("api/users", JsonContent.Create(new { Email= email }), 
                cancellationToken);
        }

        public Task<string[]> GetAllUserEmailsAsync(CancellationToken cancellationToken)
        {
            return _httpClient.GetFromJsonAsync<string[]>("api/users", cancellationToken)!;
        }
    }
}
