namespace Indotalent.Infrastructures.Images
{
    public interface IFileImageService
    {
        Task<Guid> UploadImageAsync(IFormFile? file);
        Task<Guid> UpdateImageAsync(Guid? imageId, IFormFile? newFile);
        Task<FileImage> GetImageAsync(Guid? id);
        FileImage GetImage(Guid? id);
        Task<string> GetImageUrlFromImageIdAsync(string? id);
        string GetImageUrlFromImageId(string? id);
    }
}
