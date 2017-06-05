using System.Collections.Generic;
using WebRipper.Data;

namespace WebRipper.Formatters
{
	public interface IFormatter
	{
		string Format(IEnumerable<Record> records);
	}
}