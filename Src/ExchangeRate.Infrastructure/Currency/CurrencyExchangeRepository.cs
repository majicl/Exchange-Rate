using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExchangeRate.Domain.Currency;

namespace ExchangeRate.Infrastructure.Currency
{
    public class CurrencyExchangeRepository : ICurrencyExchangeRepository
    {
        private string _baseUrl;
        public CurrencyExchangeRepository(string baseUrl)
        {
            _baseUrl = baseUrl;
        }
        public CurrencyExchangeRepository()
        {
        }

        public async Task<CurrencyExchangeRateInfo> GetCurrencyExchangeRateInfo(string[] dates, string baseCurrency, string targetCurrency)
        {
            var rgx = new Regex(@"\d{4}-\d{2}-\d{2}");
            var tasks = dates.Select(date =>
            {
                if (!rgx.IsMatch(date) || !DateTime.TryParse(date, out DateTime x))
                {
                    throw new InvalidCastException($"{date} does not match format yyyy-mm-dd");
                }

                return HttpClientHelper
                  .Get<ExchangeRatesResponse>($"{_baseUrl}history?start_at={date}&end_at={date}&base={baseCurrency}&symbols={targetCurrency}");
            });

            var infos = await Task.WhenAll(tasks);

            var allRates = infos
                .SelectMany(_ =>
                _.rates
                .Where(_ => dates.Contains(_.Key))
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
                new Domain.Currency.ExchangeRate { Date = minExchange.Date, Rate = minExchange.Rate },
                new Domain.Currency.ExchangeRate { Date = maxExchange.Date, Rate = maxExchange.Rate },
                average
            );
        }
    }
}
