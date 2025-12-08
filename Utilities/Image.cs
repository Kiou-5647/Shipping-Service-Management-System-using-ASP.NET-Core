namespace Shipping.Utilities
{
	public class Image
	{
		public static string? Upload(IFormFile file, string? folder)
		{
			string? uploadFileName = null;
			if (file != null)
			{
				uploadFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

				string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folder);
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}

				var filePath = Path.Combine(folderPath, uploadFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					file.CopyTo(stream);
				}
			}
			return uploadFileName;
		}
	}
}
