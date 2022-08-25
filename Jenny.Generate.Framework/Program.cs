using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Generate.Framework
{
	class Program
	{
		static int Main( string[] args )
		{
			var iProgress = new Progress<Core.RunnerProgress>( progress =>
			{
				Console.WriteLine( progress.Message );
			} );

			return Core.EXE.Generate.Main( args, "Jenny.Generate.Framework", false, iProgress );
		}
	}
}
