using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shipping.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shipping.ViewModels
{
	public class KhachHangViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập họ tên!")]
		[MaxLength(100)]
		[DisplayName("Họ tên")]
		public string Ten { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập Email!")]
		[EmailAddress]
		[DisplayName("Email")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
		[StringLength(100, ErrorMessage = "Mật khẩu phải dài ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[DisplayName("Mật khẩu")]
		public string Password { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng xác nhận mật khẩu!")]
		[DataType(DataType.Password)]
		[DisplayName("Xác nhận mật khẩu")]
		[Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
		public string ConfirmPassword { get; set; } = null!;

		[MaxLength(20)]
		[DisplayName("CCCD")]
		public string? CCCD { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
		[MaxLength(20)]
		[DisplayName("Số điện thoại")]
		public string SDT { get; set; } = null!;

		[MaxLength(20)]
		[DisplayName("Mã số thuế")]
		public string? MaSoThue { get; set; }

		[Required]
		[DisplayName("Loại KH")]
		public LoaiKhachHang LoaiKhachHang { get; set; } = LoaiKhachHang.CaNhan;

		[Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
		[DisplayName("Địa chỉ")]
		public string DiaChi { get; set; } = null!;

		[Required]
		[DisplayName("Tỉnh/Thành phố")]
		public string TinhThanhId { get; set; } = null!;

		[Required]
		[DisplayName("Phường/Xã")]
		public string PhuongXaId { get; set; } = null!;

		public bool IsDeleted { get; set; }

		[ValidateNever]
		public string UserId { get; set; } = null!;
	}
}
