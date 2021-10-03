using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models.Users
{
    public class SearchModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string[] Roles{ get; set; }
    }
}
