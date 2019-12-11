using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeRate.Domain.Currency;

namespace ExchangeRate.Infrastructure.Currency
{
    public class CurrencyExchangeRepository : ICurrencyExchangeRepository
    {
        private string _baseUrl;
        public CurrencyExchangeRepository(string baseUrl = "https://api.exchangeratesapi.io/")
        {
            _baseUrl = baseUrl;
        }

        public async Task<CurrencyExchangeRateInfo> GetCurrencyExchangeRateInfo(string[] dates, string baseCurrency, string targetCurrency)
        {
            var tasks = dates.Select(date =>
            HttpClientHelper
            .Get<ExchangeRatesResponse>($"{_baseUrl}history?start_at={date}&end_at={date}&base={baseCurrency}&symbols={targetCurrency}"));

            var infos = await Task.WhenAll(tasks);

            var allRates = infos
                .SelectMany(_ =>
                _.rates
                .SelectMany(r => r.Value.Where(rt => rt.Key == targetCurrency)
                .Select(v => new
                {
                    Rate = v.Value,
                    Date = r.Key
                })));

            if (!allRates.Any())
            {
                return null;
            }

            var min = allRates.Min(_ => _.Rate);
            var max = allRates.Max(_ => _.Rate);
            var average = allRates.Average(_ => _.Rate);

            var minExchange = allRates.First(_ => _.Rate == min);
            var maxExchange = allRates.First(_ => _.Rate == max);

            return new CurrencyExchangeRateInfo(
                new Domain.Currency.ExchangeRate { Date = DateTime.Parse(minExchange.Date), Rate = minExchange.Rate },
                new Domain.Currency.ExchangeRate { Date = DateTime.Parse(maxExchange.Date), Rate = maxExchange.Rate },
                average
            );
        }
    }
}
