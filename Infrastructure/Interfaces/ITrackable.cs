using System;

namespace Infrastructure.Interfaces
{
    public interface ITrackable
    {
        DateTime CreationDate { get; set; }
        DateTime ModifiedDate { get; set; }
    }
}
