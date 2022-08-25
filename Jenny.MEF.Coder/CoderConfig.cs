using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Config;

namespace Jenny.MEF.Coder
{
	public class CoderConfig
	{
		// input
		public DbContextBuild DbContextBuild { get; set; }
		public string DbContextProject { get; set; }
		public string DbContextName { get; set; }
		public string DbContextConnectionString { get; set; }

		// output
		public string CoderName { get; set; }
		public string Namespace { get; set; }

		// coder
		public CoderSettingsForCoder CoderSettings { get; set; }

		// configEx
		public string DbContextNamespaceName { get; set; }
	}

	public class CoderSettingsForCoder
	{
		Dictionary<string, object> _Settings = new Dictionary<string, object>();

		public CoderSettingsForCoder( ICoderSettings codeSettings, string coderName )
		{
			foreach ( var setting in codeSettings )
			{
				if ( setting.Key.CoderName == coderName )
				{
					_Settings.Add( setting.Key.Setting, setting.Value );
				}
			}
		}

		public object this[ string setting ]
		{
			get
			{
				object o;

				_Settings.TryGetValue( setting, out o );

				return o;
			}
		}
	}
}
