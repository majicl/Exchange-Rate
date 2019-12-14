using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

        /// <summary>
        /// This method makes a list of range based on input(list of dates)
        /// </summary>
        /// <param name="dates">the list of date.</param>
        /// <returns>a list of ranges</returns>
        private List<Tuple<string, string>> MakeRageOfTheDates(IEnumerable<DateTime> dates)
        {
            var dateFormat = "yyyy-MM-dd";
            var range = new List<Tuple<string, string>>();
            DateTime? start = null;
            var rangeSize = 0;
            dates.OrderBy(_ => _).ToList().ForEach(date =>
            {
                if (!start.HasValue)
                {
                    start = date;
                    rangeSize = 1;
                }
                else if (date.Equals(start.Value.AddDays(rangeSize)))
                {
                    rangeSize++;
                }
                else
                {
                    range.Add(new Tuple<string, string>(start.Value.ToString(dateFormat), start.Value.AddDays(rangeSize - 1).ToString(dateFormat)));
                    start = date;
                    rangeSize = 1;
                }
            });

            if (start.HasValue)
                range.Add(new Tuple<string, string>(start.Value.ToString(dateFormat), start.Value.AddDays(rangeSize - 1).ToString(dateFormat)));

            return range;
        }

        /// <summary>
        ///  This method calculates Minimum, Maximum and Average of exchange rates based on input values
        /// </summary>
        /// <param name="dates">list of dates</param>
        /// <param name="baseCurrency">source currency like 'SEK'</param>
        /// <param name="targetCurrency">destination currency like 'NOK'</param>
        /// <returns>Minimum, Maximum and Average</returns>
        public async Task<CurrencyExchangeRateInfo> GetCurrencyExchangeRateInfoAsync(string[] dates, string baseCurrency, string targetCurrency, CancellationToken cancellationToken)
        {
            // regex for validating dates
            var rgx = new Regex(@"\d{4}-\d{2}-\d{2}");

            // validate and convert all the string dates to the DateTime 
            var listOfDates = dates.Select(date =>
            {
                if (!rgx.IsMatch(date) || !DateTime.TryParse(date, out DateTime x))
                {
                    throw new InvalidCastException($"{date} does not match format yyyy-mm-dd");
                }

                return x;
            });

            // make ranges of dates for reducing the API call
            var ranges = MakeRageOfTheDates(listOfDates);

            // calling the API for all the possible ranges
            var tasks = ranges.Select(rng => HttpClientHelper
                  .GetAsync<ExchangeRatesResponse>($"{_baseUrl}history?start_at={rng.Item1}&end_at={rng.Item2}&base={baseCurrency}&symbols={targetCurrency}", cancellationToken));

            // run parallelly
            var infos = await Task.WhenAll(tasks);

            // make the result flat and ready for calculation
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
