using Shipping.Utilities;

namespace Shipping.Repositories.CrudService
{
	public interface ICrudService<Model>
	{
		IEnumerable<Model> GetAll();

		Task<ReturnState> Delete(int? id);

		Task<Model?> GetById(int? id);

		public Task<ReturnState> Create(Model model);

		public Task<ReturnState> Edit(Model model);

		public bool Save();
	}
}
