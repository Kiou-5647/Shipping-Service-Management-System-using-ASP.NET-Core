namespace Shipping.Repositories.GeoService
{
	public interface IGeoService<Model>
	{
		IEnumerable<Model> GetAll();
		Task<Model?> GetById(string id);
	}
}
