using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Api
{
    public class UserInfo
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public bool Sex { get; set; }
        public Guid Id { get; set; }
    }
}
