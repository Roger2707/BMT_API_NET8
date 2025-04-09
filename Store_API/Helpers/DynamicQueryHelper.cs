using System.Linq.Expressions;

namespace Store_API.Helpers
{
    public static class DynamicQueryHelper
    {
        // Basic dynamic Where clause
        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        // Dynamic OrderBy
        public static IQueryable<T> OrderByDynamic<T>(
            this IQueryable<T> query,
            string propertyName,
            bool ascending = true)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            string methodName = ascending ? "OrderBy" : "OrderByDescending";
            var result = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda));

            return query.Provider.CreateQuery<T>(result);
        }

        // Dynamic Select
        public static IQueryable<TResult> SelectDynamic<T, TResult>(
            this IQueryable<T> query,
            params string[] propertyNames)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var properties = propertyNames.Select(name => Expression.Property(parameter, name));
            var newExpression = Expression.New(typeof(TResult));
            var bindings = properties.Select((p, i) => Expression.Bind(typeof(TResult).GetProperties()[i], p));
            var memberInit = Expression.MemberInit(newExpression, bindings);
            var lambda = Expression.Lambda<Func<T, TResult>>(memberInit, parameter);

            return query.Select(lambda);
        }
    }
} 