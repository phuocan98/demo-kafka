using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.FaceDetections.Requests;
using Project.Modules.FaceDetections.Services;
using Project.Modules.UploadFiles.Services;
using System.Threading.Tasks;

namespace Project.FaceDetections
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceDetectionsController : BaseController
    {
        private readonly IFaceDetectionService FaceDetectionService;
        public FaceDetectionsController(IFaceDetectionService faceDetectionService)
        {
            FaceDetectionService = faceDetectionService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            var register = FaceDetectionService.Register(request);
            if (register.data is null)
            {
                return ResponseBadRequest(register.message);
            }
            return ResponseOk(register.data, register.message);
        }

        [HttpPost("detect")]
        public async Task<IActionResult> Detect([FromForm] RegisterRequest request)
        {
            var detect = FaceDetectionService.Detect(request);
            if (detect.data is null)
            {
                return ResponseBadRequest(detect.message);
            }
            return ResponseOk(detect.data, detect.message);
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteRequest request)
        {
            var delete = FaceDetectionService.Delete(request);
            if (delete.data is null)
            {
                return ResponseBadRequest(delete.message);
            }
            return ResponseOk(delete.data, delete.message);
        }

    }
}
