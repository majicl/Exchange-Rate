using System;
using System.Threading.Tasks;
using ExchangeRate.Domain.Currency;
using ExchangeRate.Domain.Test.Currency.Mocks;
using Moq;
using Xunit;

namespace ExchangeRate.Domain.Test.Currency
{
    public class CurrencyExchangeRepository
    {

        private ICurrencyExchangeRepository _ICurrencyExchangeRepository;
        private Mock<ICurrencyExchangeRepository> _ICurrencyExchangeRepositoryMock = new Mock<ICurrencyExchangeRepository>();

        public CurrencyExchangeRepository()
        {
            _ICurrencyExchangeRepository = _ICurrencyExchangeRepositoryMock.Object;
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_WrongBaseCurrency_ReturnsNull()
        {
            //Arrange
            _ICurrencyExchangeRepositoryMock
                .Setup(_ => _.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "XXX", "NOK"))
                .ReturnsAsync(null as CurrencyExchangeRateInfo);

            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "XXX", "NOK");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_WrongTargetCurrency_ReturnsNull()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "XXX");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_NullOrEmptyDates_ReturnsNull()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "XXX");

            // Assert
            Assert.Null(result);

            // Act
            result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(null, "SEK", "DOK");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_WrongFormatDates_ThrowAnExption()
        {
            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidCastException>(
                () => _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019/12/12" }, "SEK", "NOK")
            );
        }


        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsNotNull()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "NOK");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsMinMax()
        {
            //Arrange
            _ICurrencyExchangeRepositoryMock
                .Setup(_ => _.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "NOK"))
                .ReturnsAsync(new CurrencyExchangeRateInfo(new Domain.Currency.ExchangeRate(), new Domain.Currency.ExchangeRate(), 0.1m));

            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "NOK");

            // Assert
            Assert.NotNull(result.Max);
            Assert.NotNull(result.Min);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsAverage()
        {
            //Arrange
            _ICurrencyExchangeRepositoryMock
                .Setup(_ => _.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "NOK"))
                .ReturnsAsync(new CurrencyExchangeRateInfo(null, null, 0.1m));

            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "NOK");

            // Assert
            Assert.NotNull(result.Average);
        }
    }
}
