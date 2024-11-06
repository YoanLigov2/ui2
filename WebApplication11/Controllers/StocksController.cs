using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication11.Models;
using WebApplication11.Services;
using WebApplication11.Data;

namespace WebApplication11.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly StockService _stockService;
        private readonly StockContext _context;
        public StocksController(StockService stockService, StockContext context)
        {
            _stockService = stockService;
            _context = context;
        }

        [HttpGet("{symbol}")]
        public async Task<ActionResult<StockPrice>> GetStockPrice(string symbol)
        {
            var stockPrice = await _stockService.GetStockPriceAsync(symbol);
            if (stockPrice == null)
                return NotFound("Stock not found or error retrieving data.");

            return Ok(stockPrice);
        }

        [HttpGet("last20")]
        public async Task<IActionResult> GetLast20StockPrices()
        {
            var stockPrices = await _context.StockPrices
                                    .OrderByDescending(sp => sp.Date)
                                    .Take(20)
                                    .ToListAsync();
            return Ok(stockPrices);
        }

        [HttpGet("{symbol}/priceInEur")]
        public async Task<IActionResult> GetStockPriceInEur(string symbol)
        {
            try
            {
                var priceInEur = await _stockService.GetStockPriceInEurAsync(symbol);
                return Ok(new { Symbol = symbol, PriceInEur = priceInEur });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving stock price in EUR: {ex.Message}");
            }
        }

        [HttpPost("save-default")]
        public async Task<IActionResult> SaveDefaultStockPrices()
        {
            try
            {
                await _stockService.SaveDefaultStockPricesAsync();
                return Ok("Stock prices for default symbols have been saved.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving default stock prices: {ex.Message}");
            }
        }
    }
}
