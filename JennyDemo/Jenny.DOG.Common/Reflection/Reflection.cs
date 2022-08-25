using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
	public static class ReflectionExtensions
	{
		public static bool IsA( this Type source, Type target, bool includeTarget )
		{
			for ( var t = source ; t != null ; t = t.BaseType )
			{
				if ( includeTarget )
				{
					if ( t == target ) return true;

					if ( t.IsGenericType && t.GetGenericTypeDefinition() == target ) return true;

					foreach ( var i in t.GetInterfaces() )
					{
						if ( IsA( i, target, true ) ) return true;
					}
				}

				includeTarget = true;
			}

			return false;
		}
	}
}
