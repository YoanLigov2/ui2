using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication11.Services;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WebApplication11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinnhubController : ControllerBase
    {
        private readonly FinnhubService _finnhubService;

        public FinnhubController(FinnhubService finnhubService)
        {
            _finnhubService = finnhubService;
        }

        [HttpGet("{symbol}")]
        public async Task<IActionResult> GetStockInfo(string symbol)
        {
            JObject companyInfo = await _finnhubService.GetCompanyInfo(symbol);

            if (companyInfo == null || !companyInfo.HasValues)
            {
                return NotFound("Компанията не беше намерена.");
            }

            Console.WriteLine("Response from Controller: " + companyInfo.ToString()); // Отпечатва отговора от контролера

            return Ok(companyInfo.ToString());
        }

    }
}
