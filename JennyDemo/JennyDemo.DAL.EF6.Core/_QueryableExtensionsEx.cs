using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jenny
{
	partial class Jenny_QueryableExtensions
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

	public static class DbFunctionsEx
	{
		public static DateTime? AddYears( DateTime? a, int? b )
		{
			return DbFunctions.AddYears( a, b );
		}

		public static DateTime? AddDays( DateTime? a, int? b )
		{
			return DbFunctions.AddDays( a, b );
		}

		public static int? DiffYears( DateTime? a, DateTime? b )
		{
			return DbFunctions.DiffYears( a, b );
		}

		public static int? DiffDays( DateTime? a, DateTime? b )
		{
			return DbFunctions.DiffDays( a, b );
		}

		public static IQueryable<T> SubstituteDbFunctions<T>( this IQueryable<T> query )
		{
			var ex = new DbFunctionsModifier().Modify( query.Expression );

			return query.Provider.CreateQuery<T>( ex );
		}

		class DbFunctionsModifier : ExpressionVisitor
		{
			public Expression Modify( Expression ex )
			{
				return Visit( ex );
			}

			protected override Expression VisitMethodCall( MethodCallExpression node )
			{
				if ( node.Method.DeclaringType != typeof( DbFunctionsEx ) ) return base.VisitMethodCall( node );

				switch ( node.Method.Name )
				{
					case "AddYears":
					{
						var mi = typeof( DbFunctions ).GetMethod( "AddYears", new[] { typeof( DateTime? ), typeof( int? ) } );

						var ex = Expression.Call( node.Object, mi, node.Arguments[ 0 ], node.Arguments[ 1 ] );

						return ex;
					}

					case "AddDays":
					{
						var mi = typeof( DbFunctions ).GetMethod( "AddDays", new[] { typeof( DateTime? ), typeof( int? ) } );

						var ex = Expression.Call( node.Object, mi, node.Arguments[ 0 ], node.Arguments[ 1 ] );

						return ex;
					}

					case "DiffYears":
					{
						var mi = typeof( DbFunctions ).GetMethod( "DiffYears", new[] { typeof( DateTime? ), typeof( DateTime? ) } );

						var ex = Expression.Call( node.Object, mi, node.Arguments[ 0 ], node.Arguments[ 1 ] );

						return ex;
					}

					case "DiffDays":
					{
						var mi = typeof( DbFunctions ).GetMethod( "DiffDays", new[] { typeof( DateTime? ), typeof( DateTime? ) } );

						var ex = Expression.Call( node.Object, mi, node.Arguments[ 0 ], node.Arguments[ 1 ] );

						return ex;
					}

					default: throw new InvalidOperationException( "Unknown DbFunctionsEx method: " + node.Method.Name );
				}
			}
		}
	}
}
