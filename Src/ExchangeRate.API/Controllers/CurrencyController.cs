using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRate.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        [HttpGet("ExchangeRate")]
        public IEnumerable<string> ExchangeRate()
        {
            return new string[] { "A" };
        }
    }
}
