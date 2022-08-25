using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jenny
{
	public static class NBootstrapQueryableExtensionsEx
	{
		//-----------------------------------------------------------------------------------------------
		// WhereIf

		public static IQueryable<TSource> WhereIf<TSource>( this IQueryable<TSource> source, bool activate, Expression<Func<TSource, bool>> predicate )
		{
			if ( !activate ) return source;

			return source.Where( predicate );
		}

		public static IQueryable<TSource> WhereIf<TSource>( this IQueryable<TSource> source, bool activate, Expression<Func<TSource, int, bool>> predicate )
		{
			if ( !activate ) return source;

			return source.Where( predicate );
		}

		//-----------------------------------------------------------------------------------------------
	}
}
