using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public static class IEnumerableExtenstions
	{
		public static IEnumerable<TSource> EnumerableWhereIf<TSource>( this IEnumerable<TSource> source, bool activate, Func<TSource, bool> predicate )
		{
			if ( !activate ) return source;

			return source.Where( predicate );
		}

		public static IEnumerable<TSource> EnumerableWhereIf<TSource>( this IEnumerable<TSource> source, bool activate, Func<TSource, int, bool> predicate )
		{
			if ( !activate ) return source;

			return source.Where( predicate );
		}
	}
}
