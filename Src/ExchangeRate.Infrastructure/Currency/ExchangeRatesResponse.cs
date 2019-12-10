using System;
using System.Collections.Generic;

namespace ExchangeRate.Infrastructure.Currency
{
    public class ExchangeRatesResponse
    {
        public string Base { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
        // DateTimeOffset
        public string StartAt { get; set; }
        public string EndAt { get; set; }

    }
}