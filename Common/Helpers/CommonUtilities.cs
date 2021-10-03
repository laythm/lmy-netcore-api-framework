using System;
using System.Linq;
using System.Linq.Expressions;

namespace Common
{
    public static class CommonUtilities
    {
        public static IOrderedQueryable<TSource> GetOrderBy<TSource, TKey>(IQueryable<TSource> query, string dir , Expression<Func<TSource, TKey>> keySelector)
        {
            if (!string.IsNullOrEmpty(dir))
            {
                if (dir == Enums.SortDirection.asc)
                {
                    return query.OrderBy(keySelector);
                }
                else
                {
                    return query.OrderByDescending(keySelector);
                }
            }
            else
            {
                return query.OrderBy(keySelector);
            }
        }
    }
}
