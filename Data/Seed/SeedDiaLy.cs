using System.Text.Json.Serialization;

namespace Shipping.Seed
{
	public class PhuongJson
	{
		[JsonPropertyName("Code")]
		public string Code { get; set; } = null!;

		// Ánh xạ đến trường 'FullName'
		[JsonPropertyName("FullName")]
		public string FullName { get; set; } = null!;

		// Ánh xạ đến trường 'ProvinceCode'
		[JsonPropertyName("ProvinceCode")]
		public string ProvinceCode { get; set; } = null!;
	}
	public class TinhJson
	{
		[JsonPropertyName("Code")]
		public string Code { get; set; } = null!;

		[JsonPropertyName("FullName")]
		public string FullName { get; set; } = null!;

		[JsonPropertyName("Wards")]
		public List<PhuongJson> Wards { get; set; } = new List<PhuongJson>();
	}
}
