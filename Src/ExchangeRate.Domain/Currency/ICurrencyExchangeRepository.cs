using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRate.Domain.Currency
{
    public interface ICurrencyExchangeRepository
    {
        Task<CurrencyExchangeRateInfo> GetCurrencyExchangeRateInfoAsync(string[] dates, string baseCurrency, string targetCurrency, CancellationToken cancellationToken);
    }
}
