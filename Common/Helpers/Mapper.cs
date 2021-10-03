using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Common.Helpers
{
    public class Mapper
    {
        public static void Fill<S, D>(S source, D destination, params Expression<Func<S, object>>[] execludedProperties)
        {

            if (source != null)
            {
                List<string> execludedPropertiesNames = execludedProperties.Select(x => GetExpressionName(x)).ToList();

                foreach (PropertyInfo propDes in destination.GetType().GetProperties())
                {
                    if (!propDes.CanWrite) continue;
                    if (!execludedPropertiesNames.Any(x => x == propDes.Name))
                    {
                        foreach (PropertyInfo propSource in source.GetType().GetProperties())//.Where(x => (x.PropertyType.GetGenericArguments().Count() < 1)))
                        {
                            if (propDes.Name == propSource.Name)
                            {
                                if (propSource.CanRead)
                                {
                                    propDes.SetValue(destination, propSource.GetValue(source));
                                }
                                break;

                            }
                        }
                    }
                }
                //System.Reflection.PropertyInfo[] destProperties = destination.GetType().GetProperties();

                //foreach (System.Reflection.PropertyInfo sourceProperty in source.GetType().GetProperties().Where(x => (x.PropertyType.GetGenericArguments().Count() < 1)))
                //{
                //    if (execludeList == null || execludeList.All(x => x != sourceProperty.Name))
                //    {
                //        foreach (System.Reflection.PropertyInfo destProperty in destProperties)
                //        {
                //            if (destProperty.Name == sourceProperty.Name &&
                //        destProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                //            {
                //                if (destProperty.CanWrite)
                //                    destProperty.SetValue(destination, sourceProperty.GetValue(
                //                        source, new object[] { }), new object[] { });

                //                break;
                //            }
                //        }
                //    }
                //}
            }
        }


        public static void FillList<S, D>(List<S> source, List<D> destination, params Expression<Func<S, object>>[] execludedProperties)
        {

            foreach (S item in source)
            {
                D dest = Activator.CreateInstance<D>();
                Fill(item, dest, execludedProperties);
                destination.Add(dest);
            }
        }

        private static string GetExpressionName<S>(Expression<Func<S, object>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                UnaryExpression unaryExpression = (UnaryExpression)expression.Body;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }

            return memberExpression.Member.Name;
        }
    }
}
