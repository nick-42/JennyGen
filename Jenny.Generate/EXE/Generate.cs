using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Core.EXE
{
	public static class Generate
	{
		public static int Main( string[] args, string exe, bool fork, IProgress<RunnerProgress> iProgress )
		{
			var commands = CommandLine.Parse( args );

			if ( !commands.IsValid ) return 1;

			if ( !commands.ShowUserInterface )
			{
				return Engine.Runner.Run( commands, fork ) ? 0 : 1;
			}

			iProgress.Message( exe );
			iProgress.Message( $"CWD:{Directory.GetCurrentDirectory()}" );
			iProgress.Message( String.Join( Environment.NewLine, args ) );
			iProgress.Message();

			return Engine.Runner.Run( commands, fork, iProgress ) ? 0 : 1;
		}
	}
}
