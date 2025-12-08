using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shipping.Models
{
	public class VungMien
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(100)]
		public string TenVungMien { get; set; } = null!;

		//Navigation
		public virtual ICollection<TinhThanh> TinhThanhs { get; set; } = null!;
	}
}
