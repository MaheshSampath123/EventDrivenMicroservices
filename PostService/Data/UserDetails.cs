using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Data
{
    public class UserDetails
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}
