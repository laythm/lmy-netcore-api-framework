using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Infrastructure.Entities
{
    public partial class Users : ITrackable, IUserTrackable
    {
        public Users()
        {
            UserRoles = new HashSet<UserRoles>();
        }

        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }
}
