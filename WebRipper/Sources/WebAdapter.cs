using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebRipper.Sources
{
	public class WebAdapter : IWebAdapter
	{
		public async Task<string> GetStringData(string urlAddress)
		{
			var request = (HttpWebRequest)WebRequest.Create(urlAddress);

			try
			{
				using (var response = (HttpWebResponse)await request.GetResponseAsync())
				{
					if (response.StatusCode == HttpStatusCode.OK)
					{
						using (var receiveStream = response.GetResponseStream())
						using (var readStream = GetStreamReader(response.CharacterSet, receiveStream))
						{
							return readStream.ReadToEnd();
						}
					}

					return null;
				}
			}
			catch (Exception)
			{
				// ignore exception and return empty data
				return null;
			}
		}

		private static StreamReader GetStreamReader(string characterSet, Stream receiveStream)
		{
			return characterSet == null
				? new StreamReader(receiveStream)
				: new StreamReader(receiveStream, Encoding.GetEncoding(characterSet));
		}
	}
}
