using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.MEF.Coder
{
	public static class Glob
	{
		public static Action<string> ProgressMessage { get; set; }

		public static string ConfigExeFilepath { get; set; }

		public static string AssemblyCacheDirpath => String.IsNullOrWhiteSpace( ConfigExeFilepath ) ? null : Path.GetDirectoryName( ConfigExeFilepath );
	}
}
