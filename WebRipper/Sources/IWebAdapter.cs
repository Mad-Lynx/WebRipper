using System.Threading.Tasks;

namespace WebRipper.Sources
{
	public interface IWebAdapter
	{
		Task<string> GetStringData(string urlAddress);
	}
}