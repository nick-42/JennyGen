using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
	public class ReferenceComparer<T> : IEqualityComparer<T> where T : class
	{
		public static ReferenceComparer<T> Instance { get; } = new ReferenceComparer<T>();

		public bool Equals( T x, T y )
		{
			return ReferenceEquals( x, y );
		}

		public int GetHashCode( T o )
		{
			return RuntimeHelpers.GetHashCode( o );
		}
	}
}
