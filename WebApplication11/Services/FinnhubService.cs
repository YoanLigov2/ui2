using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace WebApplication11.Services
{
    public class FinnhubService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public FinnhubService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Finnhub:ApiKey"];
        }

        public async Task<JObject> GetCompanyInfo(string symbol)
        {
            var requestUrl = $"https://finnhub.io/api/v1/stock/profile2?symbol={symbol}&token={_apiKey}";

            var response = await _httpClient.GetAsync(requestUrl);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Finnhub API Response: " + jsonString); // Отпечатва данните от Finnhub API в конзолата

            return JObject.Parse(jsonString);
        }

    }
}
