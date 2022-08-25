using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.MEF.Config
{
	public class CoderConfigKey
	{
		public string CoderName { get; set; }
		public string Setting { get; set; }

		public CoderConfigKey()
		{
		}

		public CoderConfigKey( string coderName, string setting )
		{
			CoderName = coderName;
			Setting = setting;
		}
	}

	public interface ICoderSettings : IDictionary<CoderConfigKey, object>
	{
	}

	public enum DbContextBuild
	{
		NONE = 0,
		EF6Framework,
		EF6Core,
		EFCore,
	}

	public enum IncludeMode
	{
		EF6,
		EFCore,
	}

	public interface IConfig
	{
		// input
		DbContextBuild DbContextBuild { get; set; }
		string DbContextProject { get; set; }
		string DbContextName { get; set; }
		string DbContextConnectionString { get; set; }
		List<string> LoadAssemblies { get; set; }

		// output
		string Namespace { get; set; }
		string CoderName { get; set; }
		IncludeMode IncludeMode { get; set; }
		string QueryableExtensionsFilepath { get; set; }

		// coder
		ICoderSettings CoderSettings { get; }

		// Jenny runner
		bool KeepProgressWindowOpen { get; set; }
	}
}
