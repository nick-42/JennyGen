using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;

#pragma warning disable EF1001

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;

// http://stackoverflow.com/questions/1839901/how-to-wrap-entity-framework-to-intercept-the-linq-expression-just-before-execut
// http://blogs.msdn.com/b/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
// http://msdn.microsoft.com/en-us/library/bb882521.aspx

namespace NBootstrap.EF
{
	public class QueryTranslator<T> :
		IOrderedQueryable<T>,
		IAsyncEnumerable<T>
	{
		readonly string _Name = null;
		readonly string _File = null;
		readonly int _Line = 0;

		readonly TimeZoneInfo _TimeZoneInfo = null;

		public Type ElementType => typeof( T );

		public Expression Expression { get; private set; }

		public IQueryProvider Provider => _Provider;

		QueryTranslatorProvider<T> _Provider = null;

		public QueryTranslator( TimeZoneInfo tzi, IQueryable source, string name, string file, int line )
		{
			_Name = name;
			_File = file;
			_Line = line;

			_TimeZoneInfo = tzi;

			Expression = Expression.Constant( this );

			_Provider = new QueryTranslatorProvider<T>( _TimeZoneInfo, source, GetQueryCompiler( source ), name, file, line );

			//Glob.RaiseSqlExecute( "read: [" + typeof(T).Name + "] " + _Name + " " + _File + "[" + _Line + "]" );
		}

		public QueryTranslator( TimeZoneInfo tzi, IQueryable source, Expression e, string name, string file, int line )
		{
			_Name = name;
			_File = file;
			_Line = line;

			_TimeZoneInfo = tzi;

			Expression = e ?? throw new ArgumentNullException( "e" );

			_Provider = new QueryTranslatorProvider<T>( _TimeZoneInfo, source, GetQueryCompiler( source ), name, file, line );
		}

		Microsoft.EntityFrameworkCore.Query.Internal.IQueryCompiler GetQueryCompiler( IQueryable source )
		{
			var t = typeof( Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider )
				.GetField( "_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance );

			var o = t.GetValue( source.Provider );

			return (Microsoft.EntityFrameworkCore.Query.Internal.IQueryCompiler) o;
		}

		public IEnumerator<T> GetEnumerator()
		{
			var sw = System.Diagnostics.Stopwatch.StartNew();

			var enumerable = _Provider.ExecuteEnumerable( Expression );

			var list = new List<T>( enumerable );

			var msFetched = sw.Elapsed.TotalMilliseconds;

			if ( _TimeZoneInfo != null )
			{
				Global.DOG.TimeZones.GmtToLocal_ObjectGraph( _TimeZoneInfo, list );
			}

			_Provider.LogResult( msFetched, sw.Elapsed.TotalMilliseconds - msFetched, list, Expression );

			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			var sw = System.Diagnostics.Stopwatch.StartNew();

			var enumerable = _Provider.ExecuteEnumerable( Expression );

			var list = new List<T>( enumerable );

			var msFetched = sw.Elapsed.TotalMilliseconds;

			if ( _TimeZoneInfo != null )
			{
				NBootstrap.Global.DOG.TimeZones.GmtToLocal_ObjectGraph( _TimeZoneInfo, list );
			}

			_Provider.LogResult( msFetched, sw.Elapsed.TotalMilliseconds - msFetched, list, Expression );

			return list.GetEnumerator();
		}

		IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator( CancellationToken cancellationToken = default )
		{
			var enumerable = _Provider.ExecuteAsync<IAsyncEnumerable<T>>( Expression, cancellationToken );

			var enumerator = enumerable.GetAsyncEnumerator( cancellationToken );

			return enumerator;
		}
	}

	public class QueryTranslatorVisitor : ExpressionVisitor
	{
		IQueryable _source;

		public QueryTranslatorVisitor( IQueryable source )
		{
			_source = source;
		}

		protected override Expression VisitConstant( ConstantExpression c )
		{
			// fix up the Expression tree to work with EF again
			if ( c.Type.IsGenericType && c.Type.GetGenericTypeDefinition() == typeof( QueryTranslator<> ) )
			{
				return _source.Expression;
			}
			else
			{
				return base.VisitConstant( c );
			}
		}
	}

	public class QueryTranslatorProvider<T> : Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider, IAsyncQueryProvider
	{
		readonly string _name = null;
		readonly string _file = null;
		readonly int _line = 0;
		string Filename => String.IsNullOrWhiteSpace( _file ) ? "" : System.IO.Path.GetFileName( _file );

		readonly TimeZoneInfo _timeZoneInfo = null;

		internal IQueryable _source;

		public QueryTranslatorProvider(
			TimeZoneInfo tzi,
			IQueryable source,
			Microsoft.EntityFrameworkCore.Query.Internal.IQueryCompiler queryCompiler,
			string name, string file, int line
		)
			: base( queryCompiler )
		{
			_name = name;
			_file = file;
			_line = line;

			_timeZoneInfo = tzi;

			_source = source ?? throw new ArgumentNullException( "source" );
		}

		public override IQueryable CreateQuery( Expression expression )
		{
			if ( expression == null ) throw new ArgumentNullException( "expression" );
			var elementType = expression.Type.GetGenericArguments().First();
			var result = (IQueryable) Activator.CreateInstance(
				typeof( QueryTranslator<> ).MakeGenericType( elementType ),
				new object[] { _timeZoneInfo, _source, expression, _name, _file, _line }
			);
			return result;
		}

		public override IQueryable<TElement> CreateQuery<TElement>( Expression expression )
		{
			if ( expression == null ) throw new ArgumentNullException( "expression" );

			return new QueryTranslator<TElement>( _timeZoneInfo, _source, expression, _name, _file, _line ) as IQueryable<TElement>;
		}

		public override object Execute( Expression expression )
		{
			if ( expression == null ) throw new ArgumentNullException( "expression" );

			var translated = Visit( expression );

			LogExpression( translated );

			var sw = System.Diagnostics.Stopwatch.StartNew();

			var result = _source.Provider.Execute( translated );

			var msFetched = sw.Elapsed.TotalMilliseconds;

			if ( _timeZoneInfo != null )
			{
				Global.DOG.TimeZones.GmtToLocal_ObjectGraph( _timeZoneInfo, result );
			}

			LogResult( msFetched, sw.Elapsed.TotalMilliseconds - msFetched, result, expression );

			return result;
		}

		public override TResult Execute<TResult>( Expression expression )
		{
			if ( expression == null ) throw new ArgumentNullException( "expression" );

			//Log.Info( "REPO expression: TResult" );
			//LogExpression( expression );

			var result = Execute( expression );

			return (TResult) result;
		}

		public override TResult ExecuteAsync<TResult>( Expression expression, CancellationToken cancellationToken )
		{
			if ( expression == null ) throw new ArgumentNullException( "expression" );

			var translated = Visit( expression );

			LogExpression( translated );

			var sw = System.Diagnostics.Stopwatch.StartNew();

			var result = ( _source.Provider as IAsyncQueryProvider ).ExecuteAsync<TResult>( translated, cancellationToken );

			var msFetched = sw.Elapsed.TotalMilliseconds;

			if ( _timeZoneInfo != null )
			{
				Global.DOG.TimeZones.GmtToLocal_ObjectGraph( _timeZoneInfo, result );
			}

			LogResult( msFetched, sw.Elapsed.TotalMilliseconds - msFetched, result, expression );

			return result;
		}

		internal IEnumerable<T> ExecuteEnumerable( Expression expression )
		{
			if ( expression == null ) throw new ArgumentNullException( "expression" );

			var translated = Visit( expression );

			LogExpression( translated );

			var enumerable = _source.Provider.CreateQuery<T>( translated );

			return enumerable;
		}

		Expression Visit( Expression x )
		{
			return new QueryTranslatorVisitor( _source ).Visit( x );
		}

		void LogExpression( Expression x )
		{
			//Glob.RaiseSqlExecute( "expression: " + Mono.Linq.Expressions.CSharp.ToCSharpCode( x ) );
			//Glob.RaiseSqlExecute( "read: [" + typeof( T ).Name + "] " + _Name + " " + _Filename + "[" + _Line + "] " + x );
		}

		public void LogResult( double msFetched, double msTimeZones, object result, Expression expression )
		{
			var sResult = "READ( " + msFetched.ToString( "N0" ) + " ms ) ";

			var sRead = _name + "() " + Filename + "[" + _line + "] " + expression;

			var type = result?.GetType();

			if ( type != null && type.IsA( typeof( IList<> ), true ) )
			{
				var genericType = type.GetGenericArguments().FirstOrDefault();

				var logList = QueryProviderLogging.LogLists.SingleOrDefault( o => o.Type == genericType );

				var lines = logList?.Lines ?? 3;

				var list = result as IList;

				//Glob.RaiseSqlExecute( "returned: ( " + msFetched.ToString( "N0" ) + " + " + msTimeZones.ToString( "N0" ) + " ms ) LIST " + list.Count.ToString( "N0" ) + " objects" );
				Glob.RaiseSqlExecute( sResult + "LIST " + list?.Count.ToString( "N0" ) + " " + genericType?.Name + " - " + sRead );

				for ( var i = 0; i < list.Count && i < lines; i++ )
				{
					Glob.RaiseSqlExecute( i.ToString( "000" ) + ": " + ( list[ i ] ?? "NULL" ) );
				}
			}
			else
			{
				//Glob.RaiseSqlExecute( "returned: ( " + msFetched.ToString( "N0" ) + " + " + msTimeZones.ToString( "N0" ) + " ms ) " + ( result ?? "NULL" ) );
				Glob.RaiseSqlExecute( sResult + ( result ?? "NULL" ) + " - " + sRead );
			}
		}
	}

	public static class QueryProviderLogging
	{
		public class LogList
		{
			public Type Type { get; set; }
			public int Lines { get; set; }

			public LogList() { }

			public LogList( Type type, int lines )
			{
				Type = type;
				Lines = lines;
			}
		}

		public static List<LogList> LogLists { get; set; } = new List<LogList>();
	}
}
