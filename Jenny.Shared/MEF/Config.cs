using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Config;

namespace Jenny.Core.MEF
{
	[Serializable]
	public class CoderSettings : Dictionary<CoderConfigKey, object>, ICoderSettings
	{
	}

	public class Config : IConfig
	{
		// input
		public DbContextBuild DbContextBuild { get; set; }
		public string DbContextProject { get; set; }
		public string DbContextName { get; set; }
		public string DbContextConnectionString { get; set; }
		public List<string> LoadAssemblies { get; set; }

		// output
		public string CoderName { get; set; }
		public string Namespace { get; set; }
		public IncludeMode IncludeMode { get; set; }
		public string QueryableExtensionsFilepath { get; set; }

		ICoderSettings IConfig.CoderSettings { get { return CoderSettings; } }
		[System.Xml.Serialization.XmlIgnore]
		public CoderSettings CoderSettings { get; } = new CoderSettings();

		// Jenny runner
		public bool KeepProgressWindowOpen { get; set; }
	}
}
