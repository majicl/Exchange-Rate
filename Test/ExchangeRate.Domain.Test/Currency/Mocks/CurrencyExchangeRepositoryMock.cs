using System;
using System.Threading.Tasks;
using ExchangeRate.Domain.Currency;

namespace ExchangeRate.Domain.Test.Currency.Mocks
{
    public class CurrencyExchangeRepositoryMock: ICurrencyExchangeRepository
    {
        public Task<CurrencyExchangeRateInfo> GetCurrencyExchangeRateInfo(string[] dates, string baseCurrency, string targetCurrency)
        {
            throw new NotImplementedException();
        }
    }
}
