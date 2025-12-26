using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.AttachementServices
{
    public class AttachementServices : IAttachementServices
    {
        List<string> allowedExtensions = [ ".jpg", ".jpeg", ".png", ".pdf", ".docx" ];
        const int maxSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public (bool IsSuccess, string? FileName, string? ErrorMessage)
         UploadAttachement(IFormFile file, string folderName)
        {
            if (file == null)
                return (false, null, "Please select an image");

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return (false, null, "Only jpg, jpeg, png images are allowed");

            if (file.Length == 0)
                return (false, null, "The selected file is empty");

            if (file.Length > maxSizeInBytes)
                return (false, null, "Image size must be less than 5 MB");

            var folderPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "Files", folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return (true, uniqueFileName, null);
        }

        public bool DeleteAttachement(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }

    }
}
