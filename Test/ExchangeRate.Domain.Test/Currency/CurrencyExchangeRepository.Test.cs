using System;
using System.IO;
using System.Threading.Tasks;
using ExchangeRate.Domain.Currency;
using ExchangeRate.Infrastructure.Currency;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ExchangeRate.Domain.Test.Currency
{
    public class CurrencyExchangeRepositoryTest
    {

        private ICurrencyExchangeRepository _ICurrencyExchangeRepository;

        private string StubbingExchangeRateAPI()
        {
            // Stubbing ExchangeRateAPI.io
            var server = FluentMockServer.Start();
            server
              .Given(Request.Create().WithPath("/history"))
              .RespondWith(
                    Response
                    .Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(r =>
                    {
                        return File.ReadAllText("./Currency/sampleExchangeDate.json");
                    })
                );
            return $"{server.Urls[0]}/";
        }

        public CurrencyExchangeRepositoryTest()
        {
            _ICurrencyExchangeRepository = new CurrencyExchangeRepository(StubbingExchangeRateAPI());
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_WrongBaseCurrency_ReturnsNull()
        {
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
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "NOK");

            // Assert
            Assert.NotNull(result.Max);
            Assert.NotNull(result.Min);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsAverage()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2019-12-12" }, "SEK", "NOK");

            // Assert
            Assert.NotNull(result.Average);
        }
    }
}
