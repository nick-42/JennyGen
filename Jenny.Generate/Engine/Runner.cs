using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Jenny.Core.MEF;
using Jenny.MEF.Coder;
using Jenny.MEF.Config;

namespace Jenny.Core.Engine
{
	public static class Runner
	{
		public static bool Run( Commands commands, bool fork, IProgress<RunnerProgress> progress = null )
		{
			try
			{
				return RunCore( commands, fork, progress );
			}
			catch ( Exception x )
			{
				var i = -1;
				for ( var x1 = x; x1 != null; x1 = x1.InnerException )
				{
					progress.Message();
					progress.Message( "==========" );
					progress.Message( "EXCEPTION:" + ( ++i > 0 ? $" INNER #{i}" : "" ) );
					progress.Message( "==========" );

					progress.Message( x1.ToString() );
				}

				return false;
			}
		}

		static bool RunCore( Commands commands, bool fork, IProgress<RunnerProgress> progress )
		{
			progress.Message( commands.ToString() );
			progress.Message();

			if ( !File.Exists( commands.ConfigExeFilepath ) )
			{
				progress.Message( "Config assembly does not exist at: " + commands.ConfigExeFilepath );
				return false;
			}

			var allDalProjectsExist = true;

			foreach ( var dalProject in commands.DbContextProjects )
			{
				if ( !File.Exists( dalProject.AssemblyFilepath ) )
				{
					progress.Message( "DbContext assembly does not exist at: " + dalProject.AssemblyFilepath );
					allDalProjectsExist = false;
				}
			}

			if ( !allDalProjectsExist ) return false;

			var container = new Container( commands.ConfigExeFilepath );

			switch ( commands.Command )
			{
				case Command.OutputConfig:
					return RunOutputConfig( container );

				case Command.OutputMetadata:
					return RunOutputMetadata( commands, container );

				case Command.RunTransform:
					return RunTransform( commands, fork, container, progress );

				default: throw new ApplicationException( "Unknown Command: " + commands.Command );
			}
		}

		static bool RunOutputConfig( Container container )
		{
			var listOfConfig = container
				.Adapters
				.Select( adapter => { var config = new Config(); adapter.Config( config ); return config; } )
				.ToList()
			;

			var s = new System.Xml.Serialization.XmlSerializer( listOfConfig.GetType() );

			var settings = new System.Xml.XmlWriterSettings
			{
				//Encoding = Encoding.ASCII,
				Indent = true,
			};

			using var writer = System.Xml.XmlWriter.Create( Console.Out, settings );

			s.Serialize( writer, listOfConfig );

			return true;
		}

		static bool RunOutputMetadata( Commands commands, Container container )
		{
			var pairs = container
				.Adapters
				.Select( adapter =>
					{
						var config = new ConfigEx(); adapter.Config( config );
						var schema = new SchemaEx(); adapter.Schema( schema );
						config.ConfigExeFilepath = commands.ConfigExeFilepath;

						return new { config, schema };
					}
				)
				.Where( o => o.config.DbContextProject == commands.MetadataDbContextProject )
				.Where( o => o.config.DbContextName == commands.MetadataDbContextName )
				.ToList()
			;

			if ( pairs.Count != 1 ) throw new ApplicationException( $"Need exactly one matching config - found {pairs.Count}" );

			var config = pairs[ 0 ].config;
			var schema = pairs[ 0 ].schema;

			var dbContextProject = commands.DbContextProjects.Single( o => o.Name == config.DbContextProject );
			config.DbContextAssemblyFilepath = dbContextProject.AssemblyFilepath;

			var model = RunMetadataParser( commands, config, schema, null );

			var s = new System.Xml.Serialization.XmlSerializer( typeof( Schema.Model ) );

			var settings = new System.Xml.XmlWriterSettings
			{
				//Encoding = Encoding.ASCII,
				Indent = true,
			};

			using var writer = System.Xml.XmlWriter.Create( Console.Out, settings );

			s.Serialize( writer, model );

			return true;
		}

		static Schema.Model RunMetadataParser( Commands commands, ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
		{
			if ( !String.IsNullOrWhiteSpace( config.QueryableExtensionsFilepath ) )
			{
				var tQueryableExtensions = config.DbContextBuild switch
				{
					DbContextBuild.EF6Framework => EF6.FindQueryableExtensions.Run( config, schema, progress ),
					DbContextBuild.EF6Core => EF6.FindQueryableExtensions.Run( config, schema, progress ),
					DbContextBuild.EFCore => EFCore.FindQueryableExtensions.Run( config, schema, progress ),
					_ => unknownDbContextBuildType( progress )
				};

				if ( tQueryableExtensions != null && !String.IsNullOrWhiteSpace( config.QueryableExtensionsFilepath ) )
				{
					var source = TypeToSource.Run( 
						tQueryableExtensions, 
						config.DbContextBuild == DbContextBuild.EFCore,
						config.IncludeMode == IncludeMode.EFCore
					);

					foreach ( var dalProject in commands.DbContextProjects )
					{
						var filepath = Path.Combine(
							commands.SolutionDirpath,
							dalProject.Name,
							config.QueryableExtensionsFilepath
						);

						File.WriteAllText( filepath, source );
					}
				}
			}

			return config.DbContextBuild switch
			{
				DbContextBuild.EF6Framework => EF6.MetadataParser.Run( config, schema, progress ),
				DbContextBuild.EF6Core => EF6.MetadataParser.Run( config, schema, progress ),
				DbContextBuild.EFCore => EFCore.MetadataParser.Run( config, schema, progress ),
				_ => unknownDbContextBuildModel( progress )
			};

			Type unknownDbContextBuildType( IProgress<RunnerProgress> progress )
			{
				progress.Message( $"Illegal config.DbContextBuild: {config.DbContextBuild}" );
				return null;
			}
			Schema.Model unknownDbContextBuildModel( IProgress<RunnerProgress> progress )
			{
				progress.Message( $"Illegal config.DbContextBuild: {config.DbContextBuild}" );
				return null;
			}
		}

#if !NET
		static class EFCore
		{
			public static class FindQueryableExtensions
			{
				public static Type Run( ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
				{
					throw new NotImplementedException( "EFCore doesn't run on .NET framework" );
				}
			}
			public static class MetadataParser
			{
				public static Schema.Model Run( ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
				{
					throw new NotImplementedException( "EFCore doesn't run on .NET framework" );
				}
			}
		}
#endif

		static bool RunTransform( Commands commands, bool fork, Container container, IProgress<RunnerProgress> progress )
		{
			progress.Message( "Loading config from: " + commands.ConfigExeFilepath );
			progress.Message();

			var pairs = container
				.Adapters
				.Select( adapter =>
					{
						var config = new ConfigEx(); adapter.Config( config );
						var schema = new SchemaEx(); adapter.Schema( schema );
						config.ConfigExeFilepath = commands.ConfigExeFilepath;

						return new { config, schema };
					}
				)
				.ToList()
			;

			bool? keepProgressWindowOpen = null;

			foreach ( var pair in pairs )
			{
				var config = pair.config;
				var schema = pair.schema;

				if ( keepProgressWindowOpen == null ) keepProgressWindowOpen = false;
				if ( config.KeepProgressWindowOpen ) keepProgressWindowOpen = true;

				var dbContextProject = commands.DbContextProjects.Single( o => o.Name == config.DbContextProject );
				config.DbContextAssemblyFilepath = dbContextProject.AssemblyFilepath;

				if ( fork )
				{
					if ( !ForkTransform( commands, config, progress ) ) return false;
				}
				else
				{
					if ( !RunTransform( commands, fork, container, config, schema, progress ) ) return false;
				}

				progress.Message();
			}

			progress.Message(
@"
********************************************************************************************************************************
***                                                                                                                          ***
***                                                         SUCCESS                                                          ***
***                                                                                                                          ***
********************************************************************************************************************************"
			);

			if ( keepProgressWindowOpen != null && !keepProgressWindowOpen.Value && progress != null ) progress.Report( new RunnerProgress { Quit = true } );

			return true;
		}

		static bool ForkTransform( Commands commands, ConfigEx config, IProgress<RunnerProgress> progress )
		{
			var exe = config.DbContextBuild switch
			{
				DbContextBuild.EF6Framework => Path.Combine( "Jenny.Generate.Framework", "Jenny.Generate.Framework.exe" ),
				DbContextBuild.EF6Core => Path.Combine( "Jenny.Generate.Core", "Jenny.Generate.Core.exe" ),
				DbContextBuild.EFCore => Path.Combine( "Jenny.Generate.Core", "Jenny.Generate.Core.exe" ),
				_ => null
			};

			if ( String.IsNullOrWhiteSpace( exe ) )
			{
				progress.Message( $"Illegal config.DbContextBuild: {config.DbContextBuild}" );
				return false;
			}

			var dir = Directory.GetCurrentDirectory();
			var runFilepath = Path.Combine( dir, exe );

			if ( !File.Exists( runFilepath ) )
			{
				progress.Message( $"Couldn't find executable: {runFilepath}" );
				return false;
			}

			var info = new ProcessStartInfo
			{
				FileName = runFilepath,
				WorkingDirectory = Path.GetDirectoryName( runFilepath ),
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				Arguments = commands.ToArguments(),
			};

			progress.Message( $"Forking: {runFilepath}" );
			progress.Message( $"Arguments: {info.Arguments}" );
			progress.Message( $"" );

			var process = Process.Start( info );

			var tOutput = Task.Run( async () =>
			{
				for (; ; )
				{
					var line = await process.StandardOutput.ReadLineAsync();
					if ( line == null ) return;
					progress.Message( line );
				}
			} );

			const int TIMEOUT_SECONDS = 3 * 60;

			var timeout =
				!process.WaitForExit( TIMEOUT_SECONDS * 1000 ) ||
				!tOutput.Wait( TIMEOUT_SECONDS * 1000 );

			if ( timeout )
			{
				progress.Message( $"{runFilepath} Process timed out after {TIMEOUT_SECONDS} seconds" );
				return false;
			}

			if ( process.ExitCode != 0 )
			{
				progress.Message( $"{runFilepath} Exit code: {process.ExitCode}" );
				return false;
			}

			return true;
		}

		static bool RunTransform( Commands commands, bool fork, Container container, ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
		{
			progress.Message( "--------------------------------------------------------------------------------------------------------------------------------" );
			progress.Message( "Processing Project: " + config.DbContextProject );
			progress.Message( "Assembly Filepath: " + config.DbContextAssemblyFilepath );
			progress.Message( "DbContext: " + config.DbContextName );
			progress.Message( "DbContextBuild: " + config.DbContextBuild );

			if ( config.LoadAssemblies != null )
			{
				foreach ( var file in config.LoadAssemblies )
				{
					var path = Path.Combine( commands.SolutionDirpath, file );
					progress.Message( "Loading assembly: " + path );
					System.Reflection.Assembly.LoadFrom( path );
				}
			}

			var model = fork && false
				? ForkMetadata( commands, config, schema, progress )
				: RunMetadataParser( commands, config, schema, progress );

			if ( model == null )
			{
				progress.Message( $"model is null: bailing..." );
				return false;
			}

			var coder = container.Coder( config.CoderName );
			if ( coder == null )
			{
				progress.Message( "Failed to load Coder: " + config.CoderName );
				return false;
			}
			progress.Message( "Coder: " + config.CoderName );

			Glob.ConfigExeFilepath = commands.ConfigExeFilepath;
			Glob.ProgressMessage = progress.Message;

			var coderConfig = new CoderConfig
			{
				// input
				DbContextBuild = config.DbContextBuild,
				DbContextProject = config.DbContextProject,
				DbContextName = config.DbContextName,
				DbContextConnectionString = config.DbContextConnectionString,

				// output
				CoderName = config.CoderName,
				Namespace = config.Namespace,

				// coder
				CoderSettings = new CoderSettingsForCoder( config.CoderSettings, coder.CoderName ),
			};

			// configEx
			coderConfig.DbContextNamespaceName = config.DbContextNamespaceName;

			var files = new CoderFiles();

			if ( !coder.Code( coderConfig, model, files, progress.Message ) ) return false;

			progress.Message(
@"--------------------------------------------------------------------------------------------------------------------------------
Writing output files...
"
			);

			foreach ( var file in files.Files )
			{
				var outputFilepath = config.FullOutputFilepath( commands.SolutionDirpath, file.Filepath );

				progress.Message( "Writing: " + outputFilepath );
				File.WriteAllText( outputFilepath, file.Contents );
			}

			progress.Message(
@$"
Finished Project: {config.DbContextProject}
--------------------------------------------------------------------------------------------------------------------------------"
			);

			return true;
		}

		static Schema.Model ForkMetadata( Commands commands, ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
		{
			var exe = config.DbContextBuild switch
			{
				DbContextBuild.EF6Framework => Path.Combine( "Jenny.Generate.Framework", "Jenny.Generate.Framework.exe" ),
				DbContextBuild.EF6Core => Path.Combine( "Jenny.Generate.Core", "Jenny.Generate.Core.exe" ),
				DbContextBuild.EFCore => Path.Combine( "Jenny.Generate.Core", "Jenny.Generate.Core.exe" ),
				_ => null
			};

			if ( String.IsNullOrWhiteSpace( exe ) )
			{
				progress.Message( $"Illegal config.DbContextBuild: {config.DbContextBuild}" );
				return null;
			}

			var dir = Directory.GetCurrentDirectory();
			var runFilepath = Path.Combine( dir, exe );

			if ( !File.Exists( runFilepath ) )
			{
				progress.Message( $"Couldn't find executable: {runFilepath}" );
				return null;
			}

			var info = new ProcessStartInfo
			{
				FileName = runFilepath,
				WorkingDirectory = Path.GetDirectoryName( runFilepath ),
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				Arguments = commands.ToArguments( Command.OutputMetadata ) +
					$" \"-MetadataDbContextProject:{config.DbContextProject}\"" +
					$" \"-MetadataDbContextName:{config.DbContextName}\""
			};

			progress.Message( $"Forking: {runFilepath}" );
			progress.Message( $"Arguments: {info.Arguments}" );
			progress.Message( $"" );

			var process = Process.Start( info );

			const int TIMEOUT_SECONDS = 60;

			if ( !process.WaitForExit( TIMEOUT_SECONDS * 1000 ) )
			{
				progress.Message( $"{runFilepath} Process timed out after {TIMEOUT_SECONDS} seconds" );
				return null;
			}

			if ( process.ExitCode != 0 )
			{
				progress.Message( $"{runFilepath} Exit code: {process.ExitCode}" );
				return null;
			}

			try
			{
				var xml = new System.Xml.Serialization.XmlSerializer( typeof( Schema.Model ) );

				var model = (Schema.Model) xml.Deserialize( process.StandardOutput );

				return model;
			}
			catch ( Exception x )
			{
				progress.Message( $"Failed to extract metadata for: {config.DbContextName}\n\nInvalid Metadata\n\n{x}" );
				return null;
			}
		}
	}
}
