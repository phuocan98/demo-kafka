using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.FaceDetections.Requests
{
    public class RegisterRequest
    {

        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string FolderPath { get; set; }
    }
}
