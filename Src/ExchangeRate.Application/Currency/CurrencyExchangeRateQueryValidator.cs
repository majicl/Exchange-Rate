using ExchangeRate.Application.Currency.Queries;
using FluentValidation;

namespace ExchangeRate.Application.Currency
{
    public class CurrencyExchangeRateQueryValidator : AbstractValidator<CurrencyExchangeRateQuery.Query>
    {
        public CurrencyExchangeRateQueryValidator()
        {
            RuleFor(_ => _.Dates).NotEmpty().NotNull().ForEach(_ => _.Matches(@"\d{4}-\d{2}-\d{2}"));
            RuleFor(_ => _.BaseCurrency).NotEmpty().Length(2, 3);
            RuleFor(_ => _.TargetCurrency).NotEmpty().Length(2, 3);
        }
    }
}
