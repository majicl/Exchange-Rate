using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExchangeRate.Infrastructure
{
    public static class HttpClientHelper
    {
        public static async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();
                var resp = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(resp);
            }
        }
    }
}
