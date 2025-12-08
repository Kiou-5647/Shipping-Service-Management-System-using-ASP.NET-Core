using Shipping.Utilities;

namespace Shipping.Repositories.UserService
{
	public interface IUserService<Model, ViewModel>
	{
		public IEnumerable<Model> GetAll();

		public IEnumerable<Model> GetAllActive();

		public IEnumerable<Model> GetAllInactive();

		public Task<ReturnState> Delete(int? id);

		public Task<Model?> GetById(int? id);

		public Task<ReturnState> Create(ViewModel model);

		public Task<ReturnState> Edit(ViewModel model);

		public bool Save();
	}
}
