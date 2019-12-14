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
        public class Query : IRequest<CurrencyExchangeRateInfoDto>
        {
            public string[] Dates { get; set; }
            public string BaseCurrency { get; set; }
            public string TargetCurrency { get; set; }
        }

        public class Handler : IRequestHandler<Query, CurrencyExchangeRateInfoDto>
        {
            private readonly ICurrencyExchangeRepository _currencyExchangeRepository;
            public Handler(ICurrencyExchangeRepository currencyExchangeRepository)
            {
                _currencyExchangeRepository = currencyExchangeRepository;
            }

            public async Task<CurrencyExchangeRateInfoDto> Handle(Query request, CancellationToken cancellationToken)
            {
                var currencyExchangeRateInfo = await _currencyExchangeRepository.GetCurrencyExchangeRateInfo(request.Dates, request.BaseCurrency, request.TargetCurrency);

                if (currencyExchangeRateInfo == null)
                    throw new RestException(HttpStatusCode.BadRequest, new { Message = "No result found with this query" });

                // Todo: auto-mapper also can be used
                return new CurrencyExchangeRateInfoDto
                {
                    Average = currencyExchangeRateInfo.Average,
                    Min = new ExchangeRateDto { Date = currencyExchangeRateInfo.Min.Date, Rate = currencyExchangeRateInfo.Min.Rate },
                    Max = new ExchangeRateDto { Date = currencyExchangeRateInfo.Max.Date, Rate = currencyExchangeRateInfo.Max.Rate }
                };
            }
        }
    }
}
