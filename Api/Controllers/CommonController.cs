using System;
using System.IO;
using System.Threading.Tasks;
using Common.Enums;
using Common.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : BaseController
    {
        private IHostingEnvironment _hostingEnvironment;
        ILogger<CommonController> _logger;
        ICommonService _commonService;
        IConfiguration _configuration;

        public CommonController(
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            ICommonService commonService,
            ILogger<CommonController> logger) :
           base()
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _commonService = commonService;
            _logger = logger;
        }

        [HttpGet("GetImage")]
        public FileResult GetImage(string fileName, bool isThumbnail)
        {
            BaseModel returnModel = new BaseModel();

            try
            {
                string folderName = "UploadFiles";
                string webRootPath = _hostingEnvironment.WebRootPath;

                string newPath = Path.Combine(webRootPath,
                    folderName,
                    getImageUploadFilePathFromEncodedName(fileName));

                IFileProvider provider = new PhysicalFileProvider(newPath);

                if (isThumbnail)
                {
                    string thumbnailFileName = getImageThumbnailFileName(fileName);
                    IFileInfo thumbnailFileInfo = provider.GetFileInfo(thumbnailFileName);
                    var thumbnailReadStream = thumbnailFileInfo.CreateReadStream();
                    return File(thumbnailReadStream, "image/jpeg");
                }

                IFileInfo fileInfo = provider.GetFileInfo(fileName);
                var readStream = fileInfo.CreateReadStream();
                //return File(readStream, "application/octet-stream", fileName);
                return File(readStream, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                returnModel.AddError(ApiMessages.Error);
            }

            return null;
        }

        [HttpGet("CheckInternet")]
        public BaseModel CheckInternet()
        {
            BaseModel baseModel = new BaseModel();
            baseModel.AddSuccess(ApiMessages.Success);
            return baseModel;
        }


        [HttpPost("LogClientError")]
        public async Task LogClientError(ClientErrorModel errorModel)
        {
            try
            {
               await _commonService.LogClientError(errorModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        [HttpGet("GetLatestAppVersionInfo")]
        public AppVersisonInfoModel GetLatestAppVersionInfo()
        {
            AppVersisonInfoModel model = new AppVersisonInfoModel();
            model.LatestMobileAppVersion = Convert.ToInt32(_configuration["AppSettings:LatestMobileAppVersion"]);
            model.LatestAPIVersion = Convert.ToInt32(_configuration["AppSettings:LatestAPIVersion"]);
            model.AddSuccess(ApiMessages.Success);

            return model;
        }
    }
}

