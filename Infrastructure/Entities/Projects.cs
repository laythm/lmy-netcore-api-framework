using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;

namespace Infrastructure.Entities
{
    public partial class Projects : ITrackable, IUserTrackable
    {
        public Projects()
        {

        }

        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
