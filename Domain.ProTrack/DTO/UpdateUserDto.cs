using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ProTrack.DTO
{
    public class UpdateUserDto
    {
        public string EmialAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ContactNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int Age { get; set; }
    }
}
