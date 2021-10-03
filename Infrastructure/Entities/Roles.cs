using System.Collections.Generic;

namespace Infrastructure.Entities
{
    public partial class Roles
    {
        public Roles()
        {
            UserRoles = new HashSet<UserRoles>();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
    }
}
