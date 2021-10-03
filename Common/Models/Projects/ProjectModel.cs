using Common.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models.Projects
{
    public class ProjectModel:BaseModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
