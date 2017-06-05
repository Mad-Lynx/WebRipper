using System.Collections.Generic;
using Newtonsoft.Json;
using WebRipper.Data;

namespace WebRipper.Formatters
{
	public class JsonFormat : IFormatter
	{
		public string Format(IEnumerable<Record> records)
		{
			return JsonConvert.SerializeObject(records, Formatting.None);
		}
	}
}
