using Common.Enums;
using System.Collections.Generic;

namespace Common.Models.Common
{
    public class PagingSortingModel<T> where T : new()
    {
        public PagingSortingModel()
        {

        }

        public T Data { get; set; }
        public SortInfo SortInfo { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
    }

    public class SortInfo
    {
        public string column { get; set; }
        public string dir { get; set; }
    }
}
