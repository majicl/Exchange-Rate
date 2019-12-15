namespace ExchangeRate.Application.Currency
{
    public class CurrencyExchangeRateInfoDto
    {
        public ExchangeRateDto Min { get; set; } = null;
        public ExchangeRateDto Max { get; set; } = null;
        public decimal Average { get; set; } = 0;
    }
}
