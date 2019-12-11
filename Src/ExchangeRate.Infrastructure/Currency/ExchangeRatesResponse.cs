using System;
using System.Collections.Generic;

namespace ExchangeRate.Infrastructure.Currency
{
    public class ExchangeRatesResponse
    {
        public string Base { get; set; }
        public Dictionary<string, Dictionary<string, decimal>> rates { get; set; }
        // DateTimeOffset
        public string start_at { get; set; }
        public string end_at { get; set; }

    }

}