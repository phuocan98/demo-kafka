using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.FaceDetections.Requests
{
    public class DeleteRequest
    {
        [Required]
        public string FaceId { get; set; }
        [Required]
        public string Record { get; set; }
    }
}
