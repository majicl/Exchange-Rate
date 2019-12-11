using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExchangeRate.Application.Currency.Queries;
using ExchangeRate.Application.Currency;
using System.Net;

namespace ExchangeRate.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private IMediator _mediator { get; }

        public CurrencyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("ExchangeRate")]
        [ProducesResponseType(typeof(ExchangeRateDto), (int)HttpStatusCode.Created)]
        public async Task<ExchangeRateDto> ExchangeRate([FromBody]CurrencyExchangeRateQuery.Query query)
        {
            var result = await _mediator.Send(query);
            return result;
        }
    }
}
