using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.UploadFiles.Requests
{
    public class GetFileRequest
    {
        [Required]
        public string FileFullName { get; set; }
    }
}
