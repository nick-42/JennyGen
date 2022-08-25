using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jenny
{
	partial class Jenny_EntityFrameworkQueryableExtensions
	{
		//-----------------------------------------------------------------------------------------------
		// IncludeIf

		public static IQueryable<T> IncludeIf<T, TProperty>(
			this IQueryable<T> source, bool activate, Expression<Func<T, TProperty>> path ) where T : class
		{
			if ( !activate ) return source;

			return source.Include( path );
		}

		//-----------------------------------------------------------------------------------------------
	}
}
