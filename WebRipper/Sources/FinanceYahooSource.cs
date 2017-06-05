using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using WebRipper.Data;

namespace WebRipper.Sources
{
	public class FinanceYahooSource
	{
		private readonly IWebAdapter webAdapter;

		public FinanceYahooSource(IWebAdapter webAdapter)
		{
			this.webAdapter = webAdapter;
		}

		public async Task<IEnumerable<Record>> GetRecords()
		{
			var data = await webAdapter.GetStringData("https://finance.yahoo.com/world-indices");

			var json = GetRelevantJson(data);
			if (String.IsNullOrEmpty(json))
				return Enumerable.Empty<Record>();

			var list = JsonConvert.DeserializeXNode(json)
				.Descendants("quoteData")
				.SelectMany(x => x.Elements())
				.SelectMany(CreateRecord)
				.ToList();

			return list;
		}

		private static string GetRelevantJson(string s)
		{
			var startStr = "\"StreamDataStore\":{";
			var endStr = "},\"NavServiceStore\":";

			if (String.IsNullOrEmpty(s))
				return null;

			var start = s.IndexOf(startStr, StringComparison.Ordinal);
			if (start == -1)
				return null;

			var end = s.IndexOf(endStr, start, StringComparison.Ordinal);
			if (end == -1)
				return null;

			return "{" + s.Substring(start, end - start + 1) + "}";
		}

		private IEnumerable<Record> CreateRecord(XElement quoteData)
		{
			var type = GetNode(quoteData, "quoteType");
			var invalid = GetNode(quoteData, "invalid");
			if (type != "INDEX" || !String.Equals(invalid, "false", StringComparison.OrdinalIgnoreCase))
				yield break;

			var record = new Record
			{
				Id = GetNode(quoteData, "uuid"),
				Symbol = GetNode(quoteData, "symbol"),
				Exchange = GetNode(quoteData, "exchange"),
				ExchangeFullName = GetNode(quoteData, "fullExchangeName"),
				ExchangeTimeZone = GetNode(quoteData, "exchangeTimezoneName"),
				RegularMarketPrice = GetDecimalNode(quoteData, "regularMarketPrice"),
				RegularMarketVolume = GetDecimalNode(quoteData, "regularMarketVolume"),
				RegularMarketChange = GetDecimalNode(quoteData, "regularMarketChangePercent"),
				RegularMarketTime = GetFormatedNode(quoteData, "regularMarketTime"),
				MarketState = GetNode(quoteData, "marketState"),
			};
			yield return record;
		}

		private static string GetNode(XElement quoteData, string nodeName)
		{
			return quoteData.Descendants(nodeName).Select(x => x.Value).FirstOrDefault();
		}

		private static string GetFormatedNode(XElement quoteData, string nodeName)
		{
			return quoteData.Descendants(nodeName).Descendants("fmt").Select(x => x.Value).FirstOrDefault();
		}

		private static decimal GetDecimalNode(XElement quoteData, string nodeName)
		{
			var value = quoteData.Descendants(nodeName).Descendants("raw").Select(x => x.Value).FirstOrDefault();
			decimal result;
			return Decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out result)
				? result
				: 0;
		}
	}
}
