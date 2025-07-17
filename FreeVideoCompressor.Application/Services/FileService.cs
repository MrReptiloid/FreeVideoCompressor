using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FreeVideoCompressor.Application.Services;

public class FileService
{
    private const string UploadsFolder = "Uploads";

    private readonly string _uploadsFolder;
    private readonly IWebHostEnvironment _webHostEnvironment;
    
    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        
        _uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, UploadsFolder);
        Directory.CreateDirectory(_uploadsFolder); 
    }
    
    public async Task<string> SaveFileAsync(IFormFile file)
    {
        
        if (file.Length > 0)
        {
            string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            string filePath = Path.Combine(_uploadsFolder, uniqueFileName);
            await using Stream fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return filePath; 
        }

        return "Error when saving file.";
    }
}