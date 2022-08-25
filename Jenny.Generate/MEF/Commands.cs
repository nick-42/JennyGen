using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Core.MEF
{
	public enum Command
	{
		OutputConfig = 16,
		RunTransform = 32,
		OutputMetadata = 64,
	}

	public class Project
	{
		public string Name { get; set; }
		public string AssemblyFilepath { get; set; }
	}

	public class Commands
	{
		public bool IsCommandValid => Enum.IsDefined( typeof( Command ), Command );

		public bool IsValid
		{
			get
			{
				if ( !IsCommandValid ) return false;
				if ( String.IsNullOrWhiteSpace( ConfigExeFilepath ) ) return false;
				return true;
			}
		}

		public bool ShowUserInterface => Command switch
		{
			Command.OutputConfig => false,
			Command.RunTransform => true,
			Command.OutputMetadata => false,
			_ => throw new ApplicationException( "Unknown Command: " + Command ),
		};

		public Command Command { get; set; }
		public string ConfigExeFilepath { get; set; }
		public string SolutionDirpath { get; set; }
		public List<Project> DbContextProjects { get; set; } = new List<Project>();
		public string MetadataDbContextProject { get; set; }
		public string MetadataDbContextName { get; set; }

		public string ToArguments( Command? command = null )
		{
			return
				$"\"-ConfigExe:{ConfigExeFilepath}\" " +
				$"\"-Command:{command ?? Command}\" " +
				$"\"-SolutionDirpath:{SolutionDirpath}\" " +
				String.Join( " ", DbContextProjects.Select( dalBuild =>
					$"\"-DbContextProject:{dalBuild.Name}|{dalBuild.AssemblyFilepath}\""
				) );
		}

		public override string ToString()
		{
			var cr = Environment.NewLine;

			return "Command: " + Command +
				cr + "SolutionDirpath: " + SolutionDirpath +
				cr + "ConfigExeFilepath: " + ConfigExeFilepath +
				cr + "DbContextProjects: " +
				String.Join( "", DbContextProjects.Select( ( o, i ) => cr + i + " [ " + o.Name + " : " + o.AssemblyFilepath + " ]" ) )
			;
		}
	}
}
