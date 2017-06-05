using System.Linq;
using System.Threading.Tasks;
using Moq;
using WebRipper.Sources;
using Xunit;

namespace WebRipper.Tests
{
	public class FinanceYahooSourceTest
	{
		private readonly FinanceYahooSource source;
		private readonly Mock<IWebAdapter> webAdapter;

		public FinanceYahooSourceTest()
		{
			webAdapter = new Mock<IWebAdapter>();

			source = new FinanceYahooSource(webAdapter.Object);
		}

		[Fact]
		public async Task WebReturnEmpty_SourceReturnEmptyList()
		{
			var list = await source.GetRecords();

			Assert.Empty(list);
		}

		[Fact]
		public async Task WebReturnBadData_SourceReturnEmptyList()
		{
			webAdapter.Setup(x => x.GetStringData(It.IsAny<string>()))
				.Returns(Task.FromResult("SomeString"));

			var list = await source.GetRecords();

			Assert.Empty(list);
		}

		[Fact]
		public async Task WebReturnEmptyJson_SourceReturnEmptyList()
		{
			webAdapter.Setup(x => x.GetStringData(It.IsAny<string>()))
				.Returns(Task.FromResult("\"StreamDataStore\":{},\"NavServiceStore\":"));

			var list = await source.GetRecords();

			Assert.Empty(list);
		}

		[Fact]
		public async Task WebReturnEmptyQuoteData_SourceReturnEmptyList()
		{
			webAdapter.Setup(x => x.GetStringData(It.IsAny<string>()))
				.Returns(Task.FromResult("\"StreamDataStore\":{\"quoteData\":{}},\"NavServiceStore\":"));

			var list = await source.GetRecords();

			Assert.Empty(list);
		}

		[Fact]
		public async Task WebReturnOneRecord_AllPropertiesAreCorrect()
		{
			var recordData = @"""XXX"":{""uuid"":""id"",""symbol"":""sym"",""exchange"":""ex"",""fullExchangeName"":""fnme"",
				""exchangeTimezoneName"":""ex\u002Ftzone"",""regularMarketPrice"":{""raw"":1.2},""regularMarketVolume"":{""raw"":12345},
				""regularMarketChangePercent"":{""raw"":-0.5},""regularMarketTime"":{""fmt"":""1:10PM EDT""},""marketState"":""CLOSED"",
				""quoteType"":""INDEX"",""invalid"":""false""}";
			webAdapter.Setup(x => x.GetStringData(It.IsAny<string>()))
				.Returns(Task.FromResult("\"StreamDataStore\":{\"quoteData\":{" + recordData + "}},\"NavServiceStore\":"));

			var list = (await source.GetRecords()).ToList();

			Assert.Equal(1, list.Count);
			var record = list[0];
			Assert.Equal("id", record.Id);
			Assert.Equal("sym", record.Symbol);
			Assert.Equal("ex", record.Exchange);
			Assert.Equal("fnme", record.ExchangeFullName);
			Assert.Equal("ex/tzone", record.ExchangeTimeZone);
			Assert.Equal(1.2m, record.RegularMarketPrice);
			Assert.Equal(12345, record.RegularMarketVolume);
			Assert.Equal(-0.5m, record.RegularMarketChange);
			Assert.Equal("1:10PM EDT", record.RegularMarketTime);
			Assert.Equal("CLOSED", record.MarketState);
		}

		[Fact]
		public async Task WebReturnOneRecordWithBadType_EmptyList()
		{
			var recordData = @"""XXX"":{""uuid"":""id"",""symbol"":""sym"",""exchange"":""ex"",""fullExchangeName"":""fnme"",
				""exchangeTimezoneName"":""ex\u002Ftzone"",""regularMarketPrice"":{""raw"":1.2},""regularMarketVolume"":{""raw"":12345},
				""regularMarketChangePercent"":{""raw"":-0.5},""regularMarketTime"":{""fmt"":""1:10PM EDT""},""marketState"":""CLOSED"",
				""quoteType"":""IDX"",""invalid"":""false""}";
			webAdapter.Setup(x => x.GetStringData(It.IsAny<string>()))
				.Returns(Task.FromResult("\"StreamDataStore\":{\"quoteData\":{" + recordData + "}},\"NavServiceStore\":"));

			var list = await source.GetRecords();

			Assert.Empty(list);
		}

		[Fact]
		public async Task WebReturnOneRecordWithNotValid_EmptyList()
		{
			var recordData = @"""XXX"":{""uuid"":""id"",""symbol"":""sym"",""exchange"":""ex"",""fullExchangeName"":""fnme"",
				""exchangeTimezoneName"":""ex\u002Ftzone"",""regularMarketPrice"":{""raw"":1.2},""regularMarketVolume"":{""raw"":12345},
				""regularMarketChangePercent"":{""raw"":-0.5},""regularMarketTime"":{""fmt"":""1:10PM EDT""},""marketState"":""CLOSED"",
				""quoteType"":""INDEX""}";
			webAdapter.Setup(x => x.GetStringData(It.IsAny<string>()))
				.Returns(Task.FromResult("\"StreamDataStore\":{\"quoteData\":{" + recordData + "}},\"NavServiceStore\":"));

			var list = await source.GetRecords();

			Assert.Empty(list);
		}

		[Fact]
		public async Task WebReturnOneRecordWithEmptyProperties_AllPropertiesAreCorrect()
		{
			var recordData = @"""XXX"":{
				""quoteType"":""INDEX"",""invalid"":""false""}";
			webAdapter.Setup(x => x.GetStringData(It.IsAny<string>()))
				.Returns(Task.FromResult("\"StreamDataStore\":{\"quoteData\":{" + recordData + "}},\"NavServiceStore\":"));

			var list = (await source.GetRecords()).ToList();

			Assert.Equal(1, list.Count);
			var record = list[0];
			Assert.Equal(null, record.Id);
			Assert.Equal(null, record.Symbol);
			Assert.Equal(null, record.Exchange);
			Assert.Equal(null, record.ExchangeFullName);
			Assert.Equal(null, record.ExchangeTimeZone);
			Assert.Equal(0, record.RegularMarketPrice);
			Assert.Equal(0, record.RegularMarketVolume);
			Assert.Equal(0, record.RegularMarketChange);
			Assert.Equal(null, record.RegularMarketTime);
			Assert.Equal(null, record.MarketState);
		}
	}
}
