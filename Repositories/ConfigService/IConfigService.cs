using Shipping.Models;
using Shipping.Utilities;

namespace Shipping.Repositories.ConfigService
{
	public interface IConfigService
	{
		public IEnumerable<Config> GetAll();
		public Task<ReturnState> Edit(List<Config> configs);
	}
}
