using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shipping.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shipping.ViewModels
{
	public class ShipperViewModel
	{
		[Key]
		public int Id { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập họ tên!")]
		[MaxLength(100)]
		[DisplayName("Họ tên")] // Cập nhật DisplayName
		public string Ten { get; set; } = null!; // Đã đổi từ HoTen sang Ten

		[Required(ErrorMessage = "Vui lòng nhập Email!")]
		[EmailAddress]
		[DisplayName("Email")]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
		[StringLength(100, ErrorMessage = "Mật khẩu phải dài ít nhất {2} và tối đa {1} ký tự.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[DisplayName("Mật khẩu")]
		[ValidateNever]
		public string Password { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập CCCD!")]
		[MaxLength(20)]
		[DisplayName("CCCD")]
		public string CCCD { get; set; } = null!;

		[Required(ErrorMessage = "Vui lòng nhập số điện thoại!")]
		[MaxLength(20)]
		[DisplayName("Số điện thoại")]
		public string SDT { get; set; } = null!;

		[Required]
		[DisplayName("Giới tính")]
		public bool GioiTinh { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập ngày sinh!")]
		[DataType(DataType.Date)]
		[DisplayName("Ngày sinh")]
		public DateTime NgaySinh { get; set; }

		[Required(ErrorMessage = "Vui lòng nhập địa chỉ!")]
		[DisplayName("Địa chỉ")]
		public string DiaChi { get; set; } = null!;

		[Required]
		[DisplayName("Tỉnh/Thành phố")]
		public string TinhThanhId { get; set; } = null!;

		[Required]
		[DisplayName("Phường/Xã")]
		public string PhuongXaId { get; set; } = null!;

		[Required]
		[DisplayName("Loại shipper")]
		public LoaiShipper LoaiShipper { get; set; }

		[Required]
		[DisplayName("Trạng thái")]
		public TrangThaiShipper TrangThai { get; set; } = TrangThaiShipper.Nghi;

		[Required]
		[MaxLength(20)]
		[DisplayName("Biển số xe")]
		public string BienSoXe { get; set; } = null!;

		[ValidateNever]
		[DisplayName("Ảnh đại diện")]
		public IFormFile? file { get; set; }

		[DisplayName("Chi nhánh")]
		public int? ChiNhanhId { get; set; }

		public bool IsDeleted { get; set; } = false;

		[ValidateNever]
		public string UserId { get; set; } = null!;

		public string? HinhAnh {  get; set; }
	}
}
