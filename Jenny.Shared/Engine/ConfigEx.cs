using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Core.Engine
{
	public class ConfigEx : MEF.Config
	{
		public string ConfigExeFilepath { get; set; }

		public string DbContextAssemblyFilepath { get; set; }

		public string DbContextNamespaceName { get; set; }

		public string FullOutputFilepath( string solutionDirpath, string filepath )
		{
			if ( String.IsNullOrWhiteSpace( solutionDirpath ) ) return filepath;

			if ( Path.IsPathRooted( filepath ) ) return filepath;

			return Path.Combine( solutionDirpath, filepath );
		}
	}
}
