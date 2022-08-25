using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Jenny.Includes
{
	public static class SaneIncludes
	{
		static readonly MethodInfo EnumerableSelectMethodInfo
			= typeof( Enumerable )
				.GetTypeInfo()
				.GetDeclaredMethods( nameof( Enumerable.Select ) )
				.Single( o => o.GetParameters().Skip( 1 ).FirstOrDefault()
					?.ParameterType.GetGenericArguments().Count() == 2 );

		static readonly MethodInfo TypedIncludeMethodInfo
			= typeof( EntityFrameworkQueryableExtensions )
				.GetTypeInfo()
				.GetDeclaredMethods( nameof( EntityFrameworkQueryableExtensions.Include ) )
				.Single( o => o.IsGenericMethod && o.GetGenericArguments().Count() == 2 );

		static readonly MethodInfo ThenIncludeEnumerableMethodInfo
			= typeof( EntityFrameworkQueryableExtensions )
				.GetTypeInfo()
				.GetDeclaredMethods( nameof( EntityFrameworkQueryableExtensions.ThenInclude ) )
				.Single( o => o.GetParameters().FirstOrDefault()
					?.ParameterType.GetGenericArguments().Skip( 1 ).FirstOrDefault()
					?.Name == "IEnumerable`1"
				);

		public static IQueryable<T0> Split<T0, T1>(
			IQueryable<T0> query,
			Expression<System.Func<T0, T1>> path
		)
		{
			var visitor = new Visitor( path );

			if ( !visitor.Includes.Any() ) return ChainInclude( query, path, visitor.Include );

			var q = visitor.Includes.Aggregate(
				query,
				( q, include ) => ChainInclude( q, path, include )
			);

			return q;
		}

		static IQueryable<T0> ChainInclude<T0, T1>(
			IQueryable<T0> query,
			Expression<System.Func<T0, T1>> path,
			Include include
		)
		{
			var p1 = path.Parameters[ 0 ];

			var q = TypedIncludeMethodInfo
				.MakeGenericMethod( p1.Type, include.Expression.Type )
				.Invoke( null, new object[] { query, Expression.Lambda( include.Expression, p1 ) } );
			;

			foreach ( var then in include.Thens )
			{
				var p2 = ExtractParameter( then );

				q = ThenIncludeEnumerableMethodInfo
					.MakeGenericMethod( p1.Type, p2.Type, then.Type )
					.Invoke( null, new object[] { q, Expression.Lambda( then, p2 ) } );
				;
			}

			return (IQueryable<T0>) q;
		}

		static ParameterExpression ExtractParameter( Expression x )
		{
			while ( x != null )
			{
				var p = x as ParameterExpression;
				if ( p != null ) return p;

				x = ( x as MemberExpression )?.Expression ??
					( x as MethodCallExpression )?.Arguments?.FirstOrDefault();
			}

			throw new ApplicationException( $"Couldn't find parameter type for: {x}" );
		}

		class Include
		{
			public Expression Expression { get; set; }

			public List<Expression> Thens { get; set; } = new();

			public void Add( Expression x )
			{
				if ( Expression == null )
				{
					Expression = x;
				}
				else
				{
					Thens.Add( x );
				}
			}

			public Include Clone()
			{
				return new()
				{
					Expression = Expression,
					Thens = Thens.ToList(),
				};
			}

			public IEnumerable<Expression> All
			{
				get
				{
					if ( Expression != null ) yield return Expression;
					foreach ( var x in Thens ) yield return x;
				}
			}

			public Include InsertRoot( Expression xRoot )
			{
				var isEnumerable = typeof( IEnumerable ).IsAssignableFrom( xRoot.Type );

				if ( Expression == null )
				{
					Expression = xRoot;
				}
				else if ( isEnumerable )
				{
					Thens.Insert( 0, Expression );
					Expression = xRoot;
				}
				else
				{
					Expression = NBootstrap.Global.ParameterReplacer.Run(
						ExtractParameter( Expression ),
						xRoot,
						Expression
					);
				}

				return this;
			}

			public override string ToString()
			{
				return $"[ {Expression} ][ {String.Join( ", ", Thens )} ]";
			}
		}

		class Visitor : ExpressionVisitor
		{
			public Include Include { get; } = new();
			public List<Include> Includes { get; } = new();

			Expression _root;
			Expression _current;
			bool insideIncludeAll = false;

			public Visitor( Expression path )
			{
				_root = path;

				_current = path is LambdaExpression lambda ? lambda.Body : path;

				Visit( path );

				if ( !insideIncludeAll ) Include.Add( _current );
			}

			protected override Expression VisitMethodCall( MethodCallExpression node )
			{
				if ( insideIncludeAll ) return base.VisitMethodCall( node );

				if ( node.Method.Name == "IncludeAll" )
				{
					insideIncludeAll = true;

					var xRoot = node.Arguments[ 0 ];
					var pRoot = ExtractParameter( xRoot );
					//Include.Add( xRoot );

					var xPaths = (NewArrayExpression) node.Arguments[ 1 ];

					foreach ( var xPath in xPaths.Expressions )
					{
						var lambda =
							xPath as LambdaExpression ??
							( xPath as UnaryExpression )?.Operand as LambdaExpression;

						var pPath = ExtractParameter( lambda.Body );
						var target = lambda.Body.Type;

						var addSelect = typeof( IEnumerable ).IsAssignableFrom( xRoot.Type );

						if ( addSelect )
						{
							var path = Expression.Call(
								instance: null,
								method: EnumerableSelectMethodInfo.MakeGenericMethod( pPath.Type, typeof( object ) ),
								arg0: xRoot,
								arg1: lambda
							);

							var visitor = new Visitor( path );

							if ( !visitor.Includes.Any() ) Includes.Add( Merge( visitor.Include ) );
							else Includes.AddRange( visitor.Includes.Select( o => Merge( o ) ) );
						}
						else
						{
							var visitor = new Visitor( lambda.Body );

							if ( !visitor.Includes.Any() ) Includes.Add( Merge( visitor.Include.InsertRoot( xRoot ) ) );
							else Includes.AddRange( visitor.Includes.Select( o => Merge( o.InsertRoot( xRoot ) ) ) );
						}

						Include Merge( Include child )
						{
							var o = Include?.Clone() ?? new();

							foreach ( var x in child.All ) o.Add( x );

							return o;
						}
					}
				}

				if ( node.Method.Name == nameof( Enumerable.Select ) )
				{
					Include.Add( node.Arguments[ 0 ] );

					_current = ( (LambdaExpression) node.Arguments[ 1 ] ).Body;
				}

				return base.VisitMethodCall( node );
			}
		}
	}
}
