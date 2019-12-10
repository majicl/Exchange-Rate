using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRate.Domain.Currency;

namespace ExchangeRate.Infrastructure.Currency
{
    public class CurrencyExchangeRepository: ICurrencyExchangeRepository
    {
        private string _baseUrl;
        public CurrencyExchangeRepository(string baseUrl= "https://api.exchangeratesapi.io/")
        {
            _baseUrl = baseUrl;
        }

        public async Task<CurrencyExchangeRateInfo> GetCurrencyExchangeRateInfo(string[] dates, string baseCurrency, string targetCurrency)
        {
            var tasks = dates.Select(date =>
            HttpClientHelper
            .Get<ExchangeRatesResponse>($"{_baseUrl}history?start_at={date}&end_at={date}&base={baseCurrency}&symbols={targetCurrency}")
            );
            var infos = await Task.WhenAll(tasks);

            var rates = infos.Select(_ => _.Rates.Select(r => r.Value).First());
            var min = infos.Min(_ => _.Rates.Select(r => r.Value).First());
            var minExchange = infos.Where(_ => _.Rates.Select(r => r.Value).First() == min).Select(_ => _.StartAt).First();

            return new CurrencyExchangeRateInfo(
                new Domain.Currency.ExchangeRate { Date = DateTime.Parse(minExchange), Rate = min },
                 new Domain.Currency.ExchangeRate { Date = DateTime.Parse(minExchange), Rate = min },
                rates.Average());
        }
    }
}
