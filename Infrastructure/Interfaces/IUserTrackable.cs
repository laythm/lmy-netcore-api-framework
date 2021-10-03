using System;

namespace Infrastructure.Interfaces
{
    public interface IUserTrackable
    {
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
    }
}
