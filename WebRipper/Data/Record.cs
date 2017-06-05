using System;

namespace WebRipper.Data
{
	public class Record
	{
		public string Id { get; set; }

		public string Symbol { get; set; }

		public string Exchange { get; set; }

		public string ExchangeFullName { get; set; }

		public string ExchangeTimeZone { get; set; }

		public decimal RegularMarketPrice { get; set; }

		public decimal RegularMarketVolume { get; set; }

		public decimal RegularMarketChange { get; set; }

		public string RegularMarketTime { get; set; }

		public string MarketState { get; set; }
	}
}
