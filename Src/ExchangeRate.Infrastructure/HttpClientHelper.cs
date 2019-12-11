using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExchangeRate.Infrastructure
{
    public static class HttpClientHelper
    {
        public static async Task<T> Get<T>(string url)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var resp = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(resp);
            }
        }
    }
}
