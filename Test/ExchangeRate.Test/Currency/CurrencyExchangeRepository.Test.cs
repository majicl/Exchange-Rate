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
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-09" }, "SEK", "XXX");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_EmptyDates_ReturnsNull()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-09" }, "SEK", "XXX");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_NullDates_ThrowAnExption()
        {
            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(null, "SEK", "DOK")
            );
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_WrongFormatDates_ThrowAnExption()
        {
            // Act
            // Assert
            await Assert.ThrowsAsync<InvalidCastException>(
                () => _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-09", "2018/01/09" }, "SEK", "NOK")
            );
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsNotNull()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-09" }, "SEK", "NOK");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsMinMax()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-09" }, "SEK", "NOK");

            // Assert
            Assert.NotNull(result.Max);
            Assert.NotNull(result.Min);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsAverage()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-09" }, "SEK", "NOK");

            // Assert
            Assert.True(result.Average != 0);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsValidAverage()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-22", "2018-01-09", "2018-01-24" }, "SEK", "NOK");

            // data comes from sampleExchangeDate.json
            var avg = (0.9784726459m + 0.9839960117m + 0.9791706925m) / 3m;
            // Assert
            Assert.Equal(result.Average, avg);
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsValidMin()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-22", "2018-01-09", "2018-01-24" }, "SEK", "NOK");

            // data comes from sampleExchangeDate.json
            var min = 0.9784726459m;
            var minDate = "2018-01-22";
            // Assert
            Assert.Equal(result.Min, new Domain.Currency.ExchangeRate { Date = minDate, Rate = min });
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInput_ReturnsValidMax()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-22", "2018-01-09", "2018-01-24" }, "SEK", "NOK");

            // data comes from sampleExchangeDate.json
            var max = 0.9839960117m;
            var maxDate = "2018-01-09";
            // Assert
            Assert.Equal(result.Max, new Domain.Currency.ExchangeRate { Date = maxDate, Rate = max });
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInputInARange_ReturnsValidMax()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-10", "2018-01-12", "2018-01-11" }, "SEK", "NOK");

            // data comes from sampleExchangeDate.json
            var max = 0.9850513732m;
            var maxDate = "2018-01-11";
            // Assert
            Assert.Equal(result.Max, new Domain.Currency.ExchangeRate { Date = maxDate, Rate = max });
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInputInARange_ReturnsValidMin()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-10", "2018-01-12", "2018-01-11" }, "SEK", "NOK");

            // data comes from sampleExchangeDate.json
            var min = 0.982104728m;
            var minDate = "2018-01-12";
            // Assert
            Assert.Equal(result.Min, new Domain.Currency.ExchangeRate { Date = minDate, Rate = min });
        }

        [Fact]
        public async Task GetCurrencyExchangeRateInfo_ValidInputInARange_ReturnsValidAverage()
        {
            // Act
            var result = await _ICurrencyExchangeRepository.GetCurrencyExchangeRateInfo(new string[] { "2018-01-10", "2018-01-12", "2018-01-11" }, "SEK", "NOK");

            // data comes from sampleExchangeDate.json
            var avg = (0.982104728m + 0.9850513732m + 0.9830598308m) / 3m;
            // Assert
            Assert.Equal(result.Average, avg);
        }
    }
}
