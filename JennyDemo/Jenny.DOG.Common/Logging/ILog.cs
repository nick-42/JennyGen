using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public static class Log
	{
		public static Func<string, NBootstrap.Global.Logging.ILog> Logger { get; set; }

		static void Safe( string category, Action<NBootstrap.Global.Logging.ILog> fn )
		{
			if ( Logger == null )
			{
				Diagnostics.Debug.WriteLine( "Logger is null" );
			}
			else
			{
				var logger = Logger( category );

				if ( logger == null )
				{
					Diagnostics.Debug.WriteLine( "logger for " + category + " is null" );
				}
				else
				{
					fn( logger );
				}
			}
		}

		public class LogEventArgs : EventArgs
		{
			public NBootstrap.Global.Logging.Level Level { get; set; }
			public string Category { get; set; }
			public string Formatted { get; set; }
		}

		public static event EventHandler<LogEventArgs> Logged = delegate { };

		public static void RaiseLogged(
			NBootstrap.Global.Logging.Level level,
			string category,
			string formatted
		)
		{
			Logged( null, new LogEventArgs
			{
				Level = level,
				Category = category,
				Formatted = formatted,
			} );
		}

		public static void Raw(
			NBootstrap.Global.Logging.Level level,
			string message,
			LogParams lps = null,
			Exception x = null,
			bool raw = false,
			string category = null,
			bool ignoreLogIgnore = false,
			[CallerMemberName] string callerMemberName = "",
			[CallerFilePath] string callerFilePath = "",
			[CallerLineNumber] int callerLineNumber = 0
		)
		{
			Safe( category, log => log.Raw( level, message, lps, x, raw, ignoreLogIgnore, callerMemberName, callerFilePath, callerLineNumber ) );
		}

		public static void Debug( string message = null, LogParams lps = null, Exception x = null, bool raw = false, string category = null, bool ignoreLogIgnore = false, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0 ) { Raw( NBootstrap.Global.Logging.Level.Debug, message, lps, x, raw, category, ignoreLogIgnore, callerMemberName, callerFilePath, callerLineNumber ); }
		public static void Info( string message = null, LogParams lps = null, Exception x = null, bool raw = false, string category = null, bool ignoreLogIgnore = false, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0 ) { Raw( NBootstrap.Global.Logging.Level.Info, message, lps, x, raw, category, ignoreLogIgnore, callerMemberName, callerFilePath, callerLineNumber ); }
		public static void Warn( string message = null, LogParams lps = null, Exception x = null, bool raw = false, string category = null, bool ignoreLogIgnore = false, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0 ) { Raw( NBootstrap.Global.Logging.Level.Warn, message, lps, x, raw, category, ignoreLogIgnore, callerMemberName, callerFilePath, callerLineNumber ); }
		public static void Error( string message = null, LogParams lps = null, Exception x = null, bool raw = false, string category = null, bool ignoreLogIgnore = false, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0 ) { Raw( NBootstrap.Global.Logging.Level.Error, message, lps, x, raw, category, ignoreLogIgnore, callerMemberName, callerFilePath, callerLineNumber ); }
		public static void Fatal( string message = null, LogParams lps = null, Exception x = null, bool raw = false, string category = null, bool ignoreLogIgnore = false, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0 ) { Raw( NBootstrap.Global.Logging.Level.Fatal, message, lps, x, raw, category, ignoreLogIgnore, callerMemberName, callerFilePath, callerLineNumber ); }
	}
}

namespace NBootstrap.Global.Logging
{
	public interface ILog
	{
		void Raw( Level level, string message, LogParams lps, Exception x, bool raw, bool ignoreLogIgnore, string callerMemberName, string callerFilePath, int callerLineNumber );
	}

	public enum Level
	{
		Debug,
		Info,
		Warn,
		Error,
		Fatal,
	}
}
