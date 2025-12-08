using Shipping.ViewModels;

namespace Shipping.Repositories.Dashboard
{
	public interface IDashboardService
	{
		Task<DashboardViewModel> GetDashboardData();
	}
}
