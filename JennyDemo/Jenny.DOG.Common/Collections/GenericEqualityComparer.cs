using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
	public static class GenericEqualityComparerExtensions
	{
		public static IEnumerable<IGrouping<TKey, TSource>> GenericGroupBy<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TKey, object> keyExtractor
		)
			where TKey : class
		{
			return source.GroupBy( keySelector, new GenericEqualityComparer<TKey>( keyExtractor ) );
		}
	}

	public class GenericEqualityComparer<T> : IEqualityComparer<T> where T : class
	{
		readonly Func<T, T, bool> _comparer;
		readonly Func<T, object> _keyExtractor;

		public GenericEqualityComparer( Func<T, object> keyExtractor )
		{
			_keyExtractor = keyExtractor;
			_comparer = null;
		}

		public GenericEqualityComparer( Func<T, T, bool> comparer )
		{
			_keyExtractor = null;
			_comparer = comparer;
		}

		public bool Equals( T x, T y )
		{
			if ( x == null && y == null ) return true;
			if ( x == null || y == null ) return false;

			if ( _comparer != null ) return _comparer( x, y );

			var xValue = _keyExtractor( x );
			var yValue = _keyExtractor( y );

			if ( xValue == null && yValue == null ) return true;
			if ( xValue == null || yValue == null ) return false;

			return xValue.Equals( yValue );
		}

		public int GetHashCode( T o )
		{
			if ( o != null && _keyExtractor != null )
			{
				var oValue = _keyExtractor( o );

				if ( oValue != null ) return oValue.GetHashCode();
			}

			return RuntimeHelpers.GetHashCode( o );
		}
	}
}
