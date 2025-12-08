using Shipping.Models;
using Shipping.Utilities;
using Shipping.ViewModels;

namespace Shipping.Repositories.DonHangService
{
	public interface IDonHangService
	{
		IEnumerable<DonHang> GetAll();

		Task<DonHang?> GetById(string id);

		Task<IEnumerable<DonHang>> GetByKhachHangId(int khachHangId);

		Task<ReturnState> Create(DonHangViewModel model);

		Task<DonHangViewModel> CalculateShippingFee(DonHangViewModel model);

		Task<ReturnState> ValidateOrderConstraints(DonHangViewModel model);

		Task<ReturnState> Delete(string id);

		Task<ReturnState> Cancel(string id, string lyDoHuy);

		Task<ReturnState> UpdatePaymentStatus(string id, bool daThanhToan, string? paymentImage = null,  int? nhanVienId = null, int? shipperId = null);
	}
}
