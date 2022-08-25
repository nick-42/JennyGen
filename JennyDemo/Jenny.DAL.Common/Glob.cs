using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBootstrap.EF
{
	public class SqlExecuteEventArgs
	{
		public string Expression { get; set; }
	}

	public static class Glob
	{
		public static event EventHandler<SqlExecuteEventArgs> SqlExecute = delegate { };

		public static EventHandler<SqlExecuteEventArgs> ExtraLogger { get; set; }

		public static void RaiseSqlExecute( string expression )
		{
			var args = new SqlExecuteEventArgs { Expression = expression };

			ExtraLogger?.Invoke( null, args );

			SqlExecute( null, args );
		}
	}
}
