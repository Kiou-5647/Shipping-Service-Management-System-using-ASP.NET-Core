using Shipping.Models.Enums;
using Shipping.Utilities;
using Shipping.ViewModels;

namespace Shipping.Repositories.DonHangService
{
	public interface IChuyenHangService
	{
		Task<IEnumerable<ChuyenHangViewModel>> GetByDonHangId(string donHangId);

		Task<IEnumerable<ChuyenHangViewModel>> GetByShipperId(int shipperId);

		Task<ReturnState> Create(ChuyenHangViewModel model);

		Task<ReturnState> UpdateStatus(string donHangId, int thuTu, TrangThaiDonHang trangThai, string? ghiChu, IFormFile? imageFile);

		Task<ReturnState> Delete(string donHangId, int thuTu);

		Task<ReturnState> PhanCongChuyenHang(ChuyenHangViewModel model);

		Task<string?> GetLastShipmentLocation(string donHangId);
	}
}
