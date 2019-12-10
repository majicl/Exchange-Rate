using System;
namespace ExchangeRate.Domain.Currency
{
    public class CurrencyExchangeRateInfo
    {
        public ExchangeRate Min { get; private set; } = null;
        public ExchangeRate Max { get; private set; } = null;
        public decimal Average { get; private set; } = 0;

        public CurrencyExchangeRateInfo(ExchangeRate minExchangeRate, ExchangeRate maxExchangeRate, decimal average) {
            Min = minExchangeRate;
            Max = maxExchangeRate;
            Average = average;
        }
        public CurrencyExchangeRateInfo() { }
    }
}
