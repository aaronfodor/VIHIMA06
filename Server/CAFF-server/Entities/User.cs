using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CAFF_server.Entities
{
    public class User: IdentityUser
    {
        public string Name { get; set; }
        public bool Banned { get; set; }
    }
}
