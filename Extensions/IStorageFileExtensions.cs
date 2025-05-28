using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace WinPDFPresenter.Extensions;

public static class StorageFileExtensions {
	public static async Task<string> ReadTextFromStorageFileAsync(this IStorageFile file) {
		await using var stream = await file.OpenReadAsync();
		using var       reader = new StreamReader(stream);
		return await reader.ReadToEndAsync();
	}
}