using System.Collections.Generic;

namespace Common.Models.Common
{
    public class ListModel<T> : BaseModel
    {
        public ListModel()
        {
            List = new List<T>();
        }

        public List<T> List { get; set; }
        public int TotalRecordsCount { get; set; }
    }
}
