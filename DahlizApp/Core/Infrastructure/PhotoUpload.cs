using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Core.Infrastructure
{
    public static class PhotoUpload
    {
        //Check format of photo file
        //Accepted Formats are   jpg, png , jpeg
        public static bool IsValid(IFormFile photo)
        {
            bool isvalid = false;
            if (photo.ContentType == "image/jpg" || photo.ContentType == "image/jpeg" || photo.ContentType == "image/png")
            {
                isvalid = true;
            }
            return isvalid;
        }

        //Upload file to specific Folder
        public static void UploadTo(IFormFile photo, string path)
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                photo.CopyTo(stream);
            }
        }
        //Check is photo exists on folder or not
        public static bool HasPhoto(string path)
        {
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Delete Photo
        public static void DeletePhoto(string path)
        {
            File.Delete(path);
        }
    }
}
