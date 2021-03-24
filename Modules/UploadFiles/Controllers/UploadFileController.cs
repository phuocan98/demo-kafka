using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Project.App.Controllers;
using Project.Modules.UploadFiles.Requests;
using Project.Modules.UploadFiles.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Project.Modules.UploadFiles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : BaseController
    {
        private readonly IUploadFileService UploadFileService;
        public UploadFileController(IUploadFileService uploadFileService)
        {
            UploadFileService = uploadFileService;
        }
        [HttpPost]
        public IActionResult UploadFile([FromForm] UploadFileRequest request)
        {
            var upload = UploadFileService.Upload(request.File, request.FolderPath).Result;
            if (!upload.check)
            {
                return ResponseBadRequest("An error occurred");
            }
            return ResponseOk(new { Path = request.FolderPath, FileName = request.File.FileName }, "Success");
        }

        [HttpGet]
        public IActionResult GetFile([FromQuery] GetFileRequest request)
        {
            var getFile = UploadFileService.GetFile(request.FileFullName).Result;
            if (getFile.data is null)
            {
                return ResponseBadRequest(getFile.message);
            }
            new FileExtensionContentTypeProvider().TryGetContentType(request.FileFullName, out string contentType);
            return File((byte[])getFile.data, contentType ?? "application/octet-stream");
        }

        [HttpGet("a")]
        public IActionResult GetFileLink([FromQuery] GetFileRequest request)
        {
            var getFile = UploadFileService.GetUrl(request.FileFullName);
            return ResponseOk(getFile);
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            string key = Request.Query["key"].ToString();
            if (string.IsNullOrEmpty(key))
            {
                return ResponseBadRequest("KeyNotNull");
            }
            var result = UploadFileService.DeleteFile(key).Result;
            if (!result.check)
            {
                return ResponseBadRequest(result.message);
            }
            return ResponseOk(result.message);
        }
    }
}
