using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBootstrap.Global.Logging
{
	public class LogToConsole : ILog
	{
		static string Format( string message, LogParams lps, Exception x, bool raw, string callerMemberName, string callerFilePath, int callerLineNumber )
		{
			return
				(
					raw ? null : String.Format( "{0,-32}",
						(
						" " + callerMemberName + ( callerLineNumber <= 0 ? "" : "{" + callerLineNumber + "}" ) +
						"(" + ( lps == null ? "" : " " + lps + " " ) + ")"
						)
					)
				) +
				" " + message +
				( x == null ? null : " THREW EXCEPTION:\n" + x )
			;
		}

		public void Raw(
			Level level,
			string message,
			LogParams lps,
			Exception x,
			bool raw,
			bool ignoreLogIgnore,
			string callerMemberName,
			string callerFilePath,
			int callerLineNumber
		)
		{
			var s = Format( message, lps, x, raw, callerMemberName, callerFilePath, callerLineNumber );

			Console.WriteLine( s );
		}
	}
}
