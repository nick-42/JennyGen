using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Core
{
	public class RunnerProgress
	{
		public string Message { get; set; }

		public bool Quit { get; set; }
	}

	public static class RunnerProgressExtensions
	{
		public static void Message( this IProgress<RunnerProgress> progress, string message = null )
		{
			if ( progress != null ) progress.Report( new RunnerProgress { Message = message ?? "" } );
		}
	}
}
