using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Core.EXE
{
	public static class CommandLine
	{
		public static Jenny.Core.MEF.Commands Parse( string[] args )
		{
			var result = new Jenny.Core.MEF.Commands();

			foreach ( var arg in args )
			{
				if ( arg.StartsWith( "-Command:", StringComparison.InvariantCultureIgnoreCase ) )
				{
					if ( Enum.TryParse( arg.Substring( "-Command:".Length ), out MEF.Command command ) )
					{
						result.Command = command;
					}
				}

				if ( arg.StartsWith( "-ConfigExe:", StringComparison.InvariantCultureIgnoreCase ) )
				{
					result.ConfigExeFilepath = arg.Substring( "-ConfigExe:".Length );
				}

				if ( arg.StartsWith( "-SolutionDirpath:", StringComparison.InvariantCultureIgnoreCase ) )
				{
					result.SolutionDirpath = arg.Substring( "-SolutionDirpath:".Length );
				}

				if ( arg.StartsWith( "-MetadataDbContextProject:", StringComparison.InvariantCultureIgnoreCase ) )
				{
					result.MetadataDbContextProject = arg.Substring( "-MetadataDbContextProject:".Length );
				}

				if ( arg.StartsWith( "-MetadataDbContextName:", StringComparison.InvariantCultureIgnoreCase ) )
				{
					result.MetadataDbContextName = arg.Substring( "-MetadataDbContextName:".Length );
				}

				if ( arg.StartsWith( "-DbContextProject:", StringComparison.InvariantCultureIgnoreCase ) )
				{
					var command = arg.Substring( "-DbContextProject:".Length );

					var commands = command.Split( new[] { '|' }, StringSplitOptions.RemoveEmptyEntries );

					if ( commands.Length != 2 ) throw new ApplicationException( "Invalid parameter: " + arg );

					var project = commands[ 0 ];
					var assembly = commands[ 1 ];

					result.DbContextProjects.Add(
						new MEF.Project
						{
							Name = project,
							AssemblyFilepath = assembly,
						}
					);
				}
			}

			return result;
		}
	}
}
