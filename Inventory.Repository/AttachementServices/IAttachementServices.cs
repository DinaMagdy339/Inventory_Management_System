using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.AttachementServices
{
    public interface IAttachementServices
    {
        //Upload

        public (bool IsSuccess, string? FileName, string? ErrorMessage)
         UploadAttachement(IFormFile file, string folderName);
                //Delete
        bool DeleteAttachement(string filePath);
    }
}
