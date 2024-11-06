// Services/StockService.cs
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication11.Data;
using WebApplication11.Models;

namespace WebApplication11.Services
{
    public class StockService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly StockContext _context;

        public StockService(HttpClient httpClient, IConfiguration configuration, StockContext context)
        {
            _httpClient = httpClient;
            _apiKey = configuration["AlphaVantage:ApiKey"];
            //H9ZP7PSEVPP098UN
            //WZOFK9WPU946W1Q4
            _context = context;
        }

        public async Task<StockPrice> GetStockPriceAsync(string symbol)
        {
            var url = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={_apiKey}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonDocument.Parse(json);

            var timeSeries = data.RootElement.GetProperty("Time Series (Daily)");
            var latestDate = timeSeries.EnumerateObject().First();
            var latestData = latestDate.Value;

            return new StockPrice
            {
                Symbol = symbol,
                Date = DateTime.Parse(latestDate.Name),
                Open = decimal.Parse(latestData.GetProperty("1. open").GetString() ?? "0"),
                High = decimal.Parse(latestData.GetProperty("2. high").GetString() ?? "0"),
                Low = decimal.Parse(latestData.GetProperty("3. low").GetString() ?? "0"),
                Close = decimal.Parse(latestData.GetProperty("4. close").GetString() ?? "0"),
                RetrievedAt = DateTime.Now
            };
        }

        public async Task<decimal> GetUsdToEurRateAsync()
        {
            var response = await _httpClient.GetAsync("https://api.exchangerate-api.com/v4/latest/USD");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve exchange rates.");

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            // Взима курса за EUR от отговора
            var eurRate = data.GetProperty("rates").GetProperty("EUR").GetDecimal();
            return eurRate;
        }

        public async Task<decimal> GetStockPriceInEurAsync(string symbol)
        {
            // Извличане на последната налична цена на акциите от базата данни
            var stockPrice = await _context.StockPrices
                                .Where(sp => sp.Symbol == symbol)
                                .OrderByDescending(sp => sp.Date)
                                .FirstOrDefaultAsync();

            if (stockPrice == null)
                throw new Exception("Stock price not found in the database.");

            var stockPriceInUsd = stockPrice.Close;

            // Вземане на курса USD/EUR
            var usdToEurRate = await GetUsdToEurRateAsync();

            // Конвертиране в евро
            var stockPriceInEur = stockPriceInUsd * usdToEurRate;
            return stockPriceInEur;
        }

        public async Task SaveDefaultStockPricesAsync()
        {
            // Списък с 5 символа, които да се запишат автоматично
            var symbols = new List<string> { "AAPL", "GOOGL", "MSFT", "AMZN", "TSLA" };

            foreach (var symbol in symbols)
            {
                var stockPrice = await GetStockPriceAsync(symbol);

                // Проверка дали успешно е извлечена цената
                if (stockPrice != null)
                {
                    _context.StockPrices.Add(stockPrice);
                }
            }

            // Запазваме всички записи наведнъж
            await _context.SaveChangesAsync();
        }
    }
}
