using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace AgriFood.Models
{
    public class ValidateImageAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            int MaxContentLength = 1024 * 1024 * 2; //Max 2 MB file
            string[] AllowedFileExtensions = new
                string[] { ".jpg" };

            var file = value as IFormFile;

            if (file == null)
                return true;
            else if (!AllowedFileExtensions.Contains((file != null) ?
                file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLower()
                : string.Empty))
            {
                ErrorMessage = "Please upload Your Photo of type: " +
                    string.Join(", ", AllowedFileExtensions);
                return false;
            }
            else if (file.Length > MaxContentLength)
            {
                ErrorMessage = "Your Photo is too large, maximum allowed size is : "
                    + (MaxContentLength / 1024).ToString() + "MB";
                return false;
            }
            else
                return true;
        }
    }
}