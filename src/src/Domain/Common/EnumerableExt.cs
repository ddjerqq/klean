using System.Linq.Expressions;
using Domain.Abstractions;

namespace Domain.Common;

public static class EnumerableExt
{
    public static IEnumerable<(T Item, int Index)> Enumerate<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Select((item, idx) => (item, idx));
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> query, bool condition, Func<T, bool> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IQueryable<TEntity> WhereIsNotDeleted<TEntity>(this IQueryable<TEntity> queryable) where TEntity : ITrackedEntity
    {
        return queryable.Where(e => e.Deleted == null);
    }
}