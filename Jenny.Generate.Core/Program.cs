using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Generate.Core
{
	class Program
	{
		static int Main( string[] args )
		{
			var iProgress = new Progress<Jenny.Core.RunnerProgress>( progress =>
			{
				Console.WriteLine( progress.Message );
			} );

			return Jenny.Core.EXE.Generate.Main( args, "Jenny.Generate.Core", false, iProgress );
		}
	}
}
