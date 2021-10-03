using Common.Models.Common;
using System;
using System.Collections.Generic;

namespace Common.Models.Users
{
    public class LookupsModel : BaseModel
    {
        public LookupsModel()
        {
            Roles = new List<RoleModel>();
        }

        public List<RoleModel> Roles { get; set; }
    }
}
