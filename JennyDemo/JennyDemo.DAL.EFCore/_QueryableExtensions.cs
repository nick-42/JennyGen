// EntityFrameworkQueryableExtensions
namespace Jenny
{
public static partial class Jenny_EntityFrameworkQueryableExtensions
{
public static System.Linq.IQueryable<T0> Include<T0, T1>(
this System.Linq.IQueryable<T0> p0,
System.Linq.Expressions.Expression<System.Func<T0, T1>> p1 ) where T0 : class
{
	return Includes.SaneIncludes.Split( p0, p1 );
}

public static object IncludeAll<T0>( 
this IEnumerable<T0> p0, 
params System.Linq.Expressions.Expression<System.Func<T0, object>>[] p1 )
where T0 : NBootstrap.Global.IDog<T0>
{
	throw new NotImplementedException( "IncludeAll can only be used inside an Include( ... )" );
}

public static object IncludeAll<T0>(
this T0 p0,
params System.Linq.Expressions.Expression<System.Func<T0, object>>[] p1 )
where T0 : NBootstrap.Global.IDog<T0>
{
	throw new NotImplementedException( "IncludeAll can only be used inside an Include( ... )" );
}

public static System.Threading.Tasks.Task<System.Double> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Double>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Double>> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Double>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Single> AverageAsync( this System.Linq.IQueryable<System.Single> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Single>> AverageAsync( this System.Linq.IQueryable<System.Nullable<System.Single>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Single> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Single>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Single>> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Single>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Boolean> ContainsAsync<T0>( this System.Linq.IQueryable<T0> p0, T0 p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ContainsAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Collections.Generic.List<T0>> ToListAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T0[]> ToArrayAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToArrayAsync<T0>( p0, p1 ); }
public static System.Linq.IQueryable<T0> IgnoreAutoIncludes<T0>( this System.Linq.IQueryable<T0> p0 ) where T0 : class
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.IgnoreAutoIncludes<T0>( p0 ); }
public static System.Linq.IQueryable<T0> IgnoreQueryFilters<T0>( this System.Linq.IQueryable<T0> p0 ) where T0 : class
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.IgnoreQueryFilters<T0>( p0 ); }
public static System.Linq.IQueryable<T0> AsNoTracking<T0>( this System.Linq.IQueryable<T0> p0 ) where T0 : class
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTracking<T0>( p0 ); }
public static System.Linq.IQueryable<T0> AsNoTrackingWithIdentityResolution<T0>( this System.Linq.IQueryable<T0> p0 ) where T0 : class
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsNoTrackingWithIdentityResolution<T0>( p0 ); }
public static System.Linq.IQueryable<T0> AsTracking<T0>( this System.Linq.IQueryable<T0> p0 ) where T0 : class
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsTracking<T0>( p0 ); }
public static System.Linq.IQueryable<T0> AsTracking<T0>( this System.Linq.IQueryable<T0> p0, Microsoft.EntityFrameworkCore.QueryTrackingBehavior p1 ) where T0 : class
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsTracking<T0>( p0, p1 ); }
public static System.Linq.IQueryable<T0> TagWith<T0>( this System.Linq.IQueryable<T0> p0, System.String p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.TagWith<T0>( p0, p1 ); }
public static System.Linq.IQueryable<T0> TagWithCallSite<T0>( this System.Linq.IQueryable<T0> p0, System.String p1, System.Int32 p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.TagWithCallSite<T0>( p0, p1, p2 ); }
public static void Load<T0>( this System.Linq.IQueryable<T0> p0 )
{ Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Load<T0>( p0 ); }
public static System.Threading.Tasks.Task LoadAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.LoadAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<T1, T0>> ToDictionaryAsync<T0,T1>( this System.Linq.IQueryable<T0> p0, System.Func<T0, T1> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToDictionaryAsync<T0,T1>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<T1, T0>> ToDictionaryAsync<T0,T1>( this System.Linq.IQueryable<T0> p0, System.Func<T0, T1> p1, System.Collections.Generic.IEqualityComparer<T1> p2, System.Threading.CancellationToken p3 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToDictionaryAsync<T0,T1>( p0, p1, p2, p3 ); }
public static System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<T1, T2>> ToDictionaryAsync<T0,T1,T2>( this System.Linq.IQueryable<T0> p0, System.Func<T0, T1> p1, System.Func<T0, T2> p2, System.Threading.CancellationToken p3 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToDictionaryAsync<T0,T1,T2>( p0, p1, p2, p3 ); }
public static System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<T1, T2>> ToDictionaryAsync<T0,T1,T2>( this System.Linq.IQueryable<T0> p0, System.Func<T0, T1> p1, System.Func<T0, T2> p2, System.Collections.Generic.IEqualityComparer<T1> p3, System.Threading.CancellationToken p4 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToDictionaryAsync<T0,T1,T2>( p0, p1, p2, p3, p4 ); }
public static System.Threading.Tasks.Task ForEachAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Action<T0> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ForEachAsync<T0>( p0, p1, p2 ); }
public static System.Collections.Generic.IAsyncEnumerable<T0> AsAsyncEnumerable<T0>( this System.Linq.IQueryable<T0> p0 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AsAsyncEnumerable<T0>( p0 ); }
public static System.String ToQueryString( this System.Linq.IQueryable p0 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToQueryString( p0 ); }
public static System.Threading.Tasks.Task<System.Boolean> AnyAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Boolean> AnyAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AnyAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Boolean> AllAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AllAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Int32> CountAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Int32> CountAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Int64> LongCountAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.LongCountAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Int64> LongCountAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.LongCountAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<T0> FirstAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T0> FirstAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<T0> FirstOrDefaultAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T0> FirstOrDefaultAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<T0> LastAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.LastAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T0> LastAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.LastAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<T0> LastOrDefaultAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.LastOrDefaultAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T0> LastOrDefaultAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.LastOrDefaultAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<T0> SingleAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T0> SingleAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<T0> SingleOrDefaultAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T0> SingleOrDefaultAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Boolean>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SingleOrDefaultAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<T0> MinAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.MinAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T1> MinAsync<T0,T1>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, T1>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.MinAsync<T0,T1>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<T0> MaxAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.MaxAsync<T0>( p0, p1 ); }
public static System.Threading.Tasks.Task<T1> MaxAsync<T0,T1>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, T1>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.MaxAsync<T0,T1>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Decimal> SumAsync( this System.Linq.IQueryable<System.Decimal> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Decimal>> SumAsync( this System.Linq.IQueryable<System.Nullable<System.Decimal>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Decimal> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Decimal>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Decimal>> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Decimal>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Int32> SumAsync( this System.Linq.IQueryable<System.Int32> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Int32>> SumAsync( this System.Linq.IQueryable<System.Nullable<System.Int32>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Int32> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Int32>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Int32>> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Int32>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Int64> SumAsync( this System.Linq.IQueryable<System.Int64> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Int64>> SumAsync( this System.Linq.IQueryable<System.Nullable<System.Int64>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Int64> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Int64>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Int64>> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Int64>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Double> SumAsync( this System.Linq.IQueryable<System.Double> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Double>> SumAsync( this System.Linq.IQueryable<System.Nullable<System.Double>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Double> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Double>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Double>> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Double>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Single> SumAsync( this System.Linq.IQueryable<System.Single> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Single>> SumAsync( this System.Linq.IQueryable<System.Nullable<System.Single>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Single> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Single>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Single>> SumAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Single>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.SumAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Decimal> AverageAsync( this System.Linq.IQueryable<System.Decimal> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Decimal>> AverageAsync( this System.Linq.IQueryable<System.Nullable<System.Decimal>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Decimal> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Decimal>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Decimal>> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Decimal>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Double> AverageAsync( this System.Linq.IQueryable<System.Int32> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Double>> AverageAsync( this System.Linq.IQueryable<System.Nullable<System.Int32>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Double> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Int32>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Double>> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Int32>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Double> AverageAsync( this System.Linq.IQueryable<System.Int64> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Double>> AverageAsync( this System.Linq.IQueryable<System.Nullable<System.Int64>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Double> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Int64>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Double>> AverageAsync<T0>( this System.Linq.IQueryable<T0> p0, System.Linq.Expressions.Expression<System.Func<T0, System.Nullable<System.Int64>>> p1, System.Threading.CancellationToken p2 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync<T0>( p0, p1, p2 ); }
public static System.Threading.Tasks.Task<System.Double> AverageAsync( this System.Linq.IQueryable<System.Double> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
public static System.Threading.Tasks.Task<System.Nullable<System.Double>> AverageAsync( this System.Linq.IQueryable<System.Nullable<System.Double>> p0, System.Threading.CancellationToken p1 )
{ return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.AverageAsync( p0, p1 ); }
}
}
