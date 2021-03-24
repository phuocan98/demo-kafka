using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.FaceDetections.Requests
{
    public class DetectRequest
    {
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FolderPath { get; set; }
    }
}
