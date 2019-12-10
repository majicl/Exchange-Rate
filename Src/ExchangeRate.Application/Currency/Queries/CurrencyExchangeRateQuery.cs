using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ExchangeRate.Application.Errors;
using ExchangeRate.Domain.Currency;
using MediatR;

namespace ExchangeRate.Application.Currency.Queries
{
    public class CurrencyExchangeRateQuery
    {
        public class Query : IRequest<ExchangeRateDto>
        {
            public string[] Dates { get;  set; } 
            public string BaseCurrency { get;  set; } 
            public string TargetCurrency { get;  set; } 
        }

        public class Handler : IRequestHandler<Query, ExchangeRateDto>
        {
            private readonly ICurrencyExchangeRepository _currencyExchangeRepository;
            public Handler(ICurrencyExchangeRepository currencyExchangeRepository)
            {
                _currencyExchangeRepository = currencyExchangeRepository;
            }

            public async Task<ExchangeRateDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var currencyExchangeRateInfo = await _currencyExchangeRepository.GetCurrencyExchangeRateInfo(request.Dates, request.BaseCurrency, request.TargetCurrency);

                if (currencyExchangeRateInfo == null)
                    throw new RestException(HttpStatusCode.BadRequest, new { });

                return new ExchangeRateDto
                {
                    
                };
            }
        }
    }
}
