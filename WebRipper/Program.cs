using System;
using WebRipper.Formatters;
using WebRipper.Sources;

namespace WebRipper
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var formatter = GetFormatter(args);
			if (formatter == null)
			{
				ShowHelp();
				return;
			}

			var source = new FinanceYahooSource(new WebAdapter());
			var list = source.GetRecords().Result;

			Console.Write(formatter.Format(list));
		}

		private static void ShowHelp()
		{
			Console.WriteLine("Usage: WebRipper.exe [json|csv]");
		}

		private static IFormatter GetFormatter(string[] args)
		{
			if (args.Length == 0)
				return new JsonFormat(); // default

			if (args.Length != 1)
				return null;

			switch (args[0].ToLower())
			{
				case "json":
					return new JsonFormat();

				case "csv":
					return new CsvFormat();

				default:
					return null;
			}
		}
	}
}
