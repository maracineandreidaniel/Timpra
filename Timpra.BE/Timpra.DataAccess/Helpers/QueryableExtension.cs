using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dashboard.Core.Extensions
{
    public static class QueryableExtension
    {
        public static IQueryable<TEntity> OrderByDynamic<TEntity>(this IQueryable<TEntity> source, string orderByProperty, string? order)
        {
            string command = order == "desc" ? "OrderByDescending" : "OrderBy";
            var type = typeof(TEntity);
            var pascalCaseProperty = char.ToUpper(orderByProperty[0]) + orderByProperty.Substring(1);
            var property = type.GetProperty(pascalCaseProperty);
            var parameter = Expression.Parameter(type, "p");
            if (property == null || order == null)
            {
                return source;
            }
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}