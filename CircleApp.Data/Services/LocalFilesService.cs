using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CircleApp.Data.Helpers.Enums;
using CircleApp.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Hosting;


namespace CircleApp.Data.Services
{
    public class LocalFilesService : IFilesService
    {
        private readonly string _basePath;

        public LocalFilesService()
        {
            // Get the application's base directory  
            _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "images");
        }

        public async Task<string> UploadImageAsync(IFormFile file, ImageFileType imageFileType)
        {
            if (file == null || file.Length == 0)
                return "";

            string containerName = imageFileType switch
            {
                ImageFileType.PostImage => "posts",
                ImageFileType.StoryImage => "stories",
                ImageFileType.ProfilePicture => "profiles",
                ImageFileType.CoverImage => "covers",
                _ => throw new ArgumentException("Invalid file type")
            };

            string folderPath = Path.Combine(_basePath, containerName);

            // Ensure directory exists  
            Directory.CreateDirectory(folderPath);

            // Generate a unique filename  
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(folderPath, fileName);

            // Save the file  
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return a web-accessible URL path  
            return $"/images/{containerName}/{fileName}";
        }
    }
}
