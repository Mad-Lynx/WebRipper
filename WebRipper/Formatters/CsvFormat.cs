using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using WebRipper.Data;

namespace WebRipper.Formatters
{
	public class CsvFormat : IFormatter
	{
		private const string ColSeparator = ",";
		private static readonly string RowFormat = String.Join(ColSeparator, Enumerable.Range(0, 10).Select(i => $"{{{i}}}"));

		public string Format(IEnumerable<Record> records)
		{
			var sb = new StringBuilder();

			Write(sb, GetHeaders());
			foreach (var record in records)
			{
				Write(sb,
					Escape(record.Id),
					Escape(record.Symbol),
					Escape(record.Exchange),
					Escape(record.ExchangeFullName),
					Escape(record.ExchangeTimeZone),
					record.RegularMarketPrice,
					record.RegularMarketVolume,
					record.RegularMarketChange,
					Escape(record.RegularMarketTime),
					Escape(record.MarketState)
					);
			}

			return sb.ToString();
		}

		private static string Escape(string value)
		{
			return "\"" + value?.Replace("\"", "\"\"") + "\"";
		}

		private static void Write(StringBuilder sb, params object[] cols)
		{
			sb.AppendFormat(CultureInfo.InvariantCulture, RowFormat, cols);
			sb.AppendLine();
		}

		private static object[] GetHeaders()
		{
			return new object[]
			{
				Escape("Id"),
				Escape("Symbol"),
				Escape("Exchange"),
				Escape("Exchange Full Name"),
				Escape("Exchange Time Zone"),
				Escape("Regular Market Price"),
				Escape("Regular Market Volume"),
				Escape("Regular Market Change %"),
				Escape("Regular Market Time"),
				Escape("Market State"),
			};
		}
	}
}
