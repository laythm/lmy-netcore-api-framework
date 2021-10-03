using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Api.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController()
        {
        }

        protected string getImageExtension(string fileName)
        {
            string ext = Path.GetExtension(fileName.Trim('"'));
            if (string.IsNullOrEmpty(ext))
            {
                ext = ".jpg";
            }

            return ext;
        }

        protected string getImageUploadFilePathFromEncodedName(string encodedName)
        {
            if (encodedName.Contains("$"))
            {
                var parameters = encodedName.Split("$");

                return Path.Combine(parameters[1], parameters[2], parameters[3]);
            }

            return "";
        }

        protected string getImageUploadFilePathParameters(out string encodedPath)
        {
            encodedPath = "$" + DateTime.Now.Year.ToString() + "$" + DateTime.Now.Month.ToString() + "$" + DateTime.Now.Day.ToString() + "$";

            return Path.Combine(DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
        }

        protected string getImageThumbnailFileName(string fileName)
        {
            string thumbnailFileName = Path.GetFileNameWithoutExtension(fileName) + "_thumbnail_" + Path.GetExtension(fileName);

            return thumbnailFileName;
        }
    }
}
