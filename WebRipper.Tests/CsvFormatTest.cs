using System;
using System.Linq;
using WebRipper.Formatters;
using Xunit;
using Record = WebRipper.Data.Record;

namespace WebRipper.Tests
{
	public class CsvFormatTest
	{
		private readonly IFormatter formatter = new CsvFormat();
		private const string Header = "\"Id\",\"Symbol\",\"Exchange\",\"Exchange Full Name\",\"Exchange Time Zone\"," +
			"\"Regular Market Price\",\"Regular Market Volume\",\"Regular Market Change %\",\"Regular Market Time\",\"Market State\"";

		[Fact]
		public void EmptyList_OnlyHeaders()
		{
			var result = formatter.Format(Enumerable.Empty<Record>());

			Assert.Equal(Header + Environment.NewLine, result);
		}

		[Fact]
		public void SimpleList_HeaderAndOneLine()
		{
			var record = new Record
			{
				Id = "id",
				Symbol = "symbol",
				Exchange = "exchange",
				ExchangeFullName = "fullName",
				ExchangeTimeZone = "timezone",
				RegularMarketPrice = 1,
				RegularMarketVolume = 2,
				RegularMarketChange = 0.5m,
				RegularMarketTime = "time",
				MarketState = "State",
			};
			var result = formatter.Format(new []{ record });

			var expected = Header + Environment.NewLine + 
				@"""id"",""symbol"",""exchange"",""fullName"",""timezone"",1,2,0.5,""time"",""State""" + Environment.NewLine;
			Assert.Equal(expected, result);
		}

		[Fact]
		public void StringValues_ProperEscape()
		{
			var record = new Record
			{
				Id = "id",
				Symbol = "sym,bol",
				Exchange = "exch;ange",
				ExchangeFullName = "full\"Name",
				ExchangeTimeZone = "time'zone",
				RegularMarketPrice = 1,
				RegularMarketVolume = 2,
				RegularMarketChange = 0.5m,
				RegularMarketTime = "time",
				MarketState = "State",
			};
			var result = formatter.Format(new []{ record });

			var expected = Header + Environment.NewLine + 
				@"""id"",""sym,bol"",""exch;ange"",""full""""Name"",""time'zone"",1,2,0.5,""time"",""State""" + Environment.NewLine;
			Assert.Equal(expected, result);
		}
	}
}
