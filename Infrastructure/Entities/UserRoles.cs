using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Infrastructure.Entities
{
    public partial class UserRoles: ITrackable, IUserTrackable
    {
        public UserRoles()
        {
           
        }

        public string ID { get; set; }
        public string UserID { get; set; }
        public string RoleID { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public virtual Users User { get; set; }
        public virtual Roles Role { get; set; }
    }
}
