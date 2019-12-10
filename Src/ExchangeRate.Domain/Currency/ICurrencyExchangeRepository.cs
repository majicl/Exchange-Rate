using System;
using System.Threading.Tasks;

namespace ExchangeRate.Domain.Currency
{
    public interface ICurrencyExchangeRepository
    {
        Task<CurrencyExchangeRateInfo> GetCurrencyExchangeRateInfo(string[] dates, string baseCurrency, string targetCurrency);
    }
}
