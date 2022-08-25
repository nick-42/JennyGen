using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public class LogParam
	{
		public string Name { get; }
		public string Value { get; }

		public LogParam( string n, object v )
		{
			Name = n;

			if ( v == null )
			{
				Value = "NULL";
			}
			else if ( v is string )
			{
				Value = v.ToString();
			}
			else if ( v is IEnumerable enumerable )
			{
				Value = String.Join( ", ", enumerable.Cast<object>().Select( o => o?.ToString() ?? "NULL" ) );
			}
			else
			{
				Value = v.ToString();
			}
		}
	}

	public class LogParams
	{
		public List<LogParam> List { get; private set; }

		public LogParams()
		{
			List = new List<LogParam>();
		}

		public LogParams( IList<LogParam> list )
			: this()
		{
			foreach ( var o in list ) Add( o.Name, o.Value );
		}

		public LogParams( string n0, object v0 )
			: this()
		{
			Add( n0, v0 );
		}

		public LogParams( string n0, object v0, string n1, object v1 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7, string n8, object v8 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
			Add( n8, v8 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7, string n8, object v8, string n9, object v9 )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
			Add( n8, v8 );
			Add( n9, v9 );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7, string n8, object v8, string n9, object v9, string nA, object vA )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
			Add( n8, v8 );
			Add( n9, v9 );
			Add( nA, vA );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7, string n8, object v8, string n9, object v9, string nA, object vA, string nB, object vB )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
			Add( n8, v8 );
			Add( n9, v9 );
			Add( nA, vA );
			Add( nB, vB );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7, string n8, object v8, string n9, object v9, string nA, object vA, string nB, object vB, string nC, object vC )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
			Add( n8, v8 );
			Add( n9, v9 );
			Add( nA, vA );
			Add( nB, vB );
			Add( nC, vC );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7, string n8, object v8, string n9, object v9, string nA, object vA, string nB, object vB, string nC, object vC, string nD, object vD )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
			Add( n8, v8 );
			Add( n9, v9 );
			Add( nA, vA );
			Add( nB, vB );
			Add( nC, vC );
			Add( nD, vD );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7, string n8, object v8, string n9, object v9, string nA, object vA, string nB, object vB, string nC, object vC, string nD, object vD, string nE, object vE )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
			Add( n8, v8 );
			Add( n9, v9 );
			Add( nA, vA );
			Add( nB, vB );
			Add( nC, vC );
			Add( nD, vD );
			Add( nE, vE );
		}

		public LogParams( string n0, object v0, string n1, object v1, string n2, object v2, string n3, object v3, string n4, object v4, string n5, object v5, string n6, object v6, string n7, object v7, string n8, object v8, string n9, object v9, string nA, object vA, string nB, object vB, string nC, object vC, string nD, object vD, string nE, object vE, string nF, object vF )
			: this()
		{
			Add( n0, v0 );
			Add( n1, v1 );
			Add( n2, v2 );
			Add( n3, v3 );
			Add( n4, v4 );
			Add( n5, v5 );
			Add( n6, v6 );
			Add( n7, v7 );
			Add( n8, v8 );
			Add( n9, v9 );
			Add( nA, vA );
			Add( nB, vB );
			Add( nC, vC );
			Add( nD, vD );
			Add( nE, vE );
			Add( nF, vF );
		}

		public void Add( string n, object v )
		{
			List.Add( new LogParam( n, v ) );
		}

		public override string ToString()
		{
			if ( List == null || List.Count == 0 ) return String.Empty;

			return String.Join( ", ", List.Select( p => p.Name + ": " + p.Value ) );
		}
	}
}
