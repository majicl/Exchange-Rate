namespace ExchangeRate.Domain.Currency
{
    public class ExchangeRate
    {
        public decimal Rate { get; set; }
        public string Date { get; set; }

        public override bool Equals(object obj)
        {
            var exchange = obj as ExchangeRate;
            if (exchange == null)
            {
                return false;
            }

            if (this.Date == exchange.Date && this.Rate == exchange.Rate)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
