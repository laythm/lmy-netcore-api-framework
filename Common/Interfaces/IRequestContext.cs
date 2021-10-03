using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Interfaces
{
    public interface IRequestContext
    {
        string CurrentUserID { get; }
        string[] CurrentUserRoles { get; }
    }
}
