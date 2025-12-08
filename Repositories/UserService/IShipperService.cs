using Shipping.Models;
using Shipping.Utilities;
using Shipping.ViewModels;

namespace Shipping.Repositories.UserService
{
	public interface IShipperService : IUserService<Shipper, ShipperViewModel>
	{
		Task<Shipper?> GetByUserId(string userId);

		Task<ReturnState> UpdateState(string userId, string viTri, int trangThai);

		Task<IEnumerable<dynamic>> GetAvailableShippers(string locationId);
	}
}
