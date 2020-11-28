using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAFF_server.DTOs
{
    public class FormModel
    {
        public IFormFile File { get; set; }
    }
}
