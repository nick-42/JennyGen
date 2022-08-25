using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Jenny.Vsix
{
	static class Runner
	{
		//-----------------------------------------------------------------------------------------
		// RunAllAsync

		public static async Task<bool> RunAllAsync()
		{
			var allProjects = await VS.Solutions.GetAllProjectsAsync();

			var configProjects = allProjects
				.Where( project => Jenny.IsConfigProject( project ) )
				.OrderBy( o => o.Name )
				.ToList()
			;

			var ok = true;
			var i = 0;

			foreach ( var project in configProjects )
			{
				await VS.StatusBar.ShowProgressAsync( $"JENNY: {project.Name}", ++i, configProjects.Count + 1 );

				ok &= await RunAsync( project );
			}

			return ok;
		}

		//-----------------------------------------------------------------------------------------
		// RunAsync

		public static async Task<bool> RunAsync( Project project )
		{
			var solution = await VS.Solutions.GetCurrentSolutionAsync();
			var solutionDirpath = Path.GetDirectoryName( solution.FullPath );

			var runFilepath = Path.Combine( solutionDirpath, ".jenny\\Jenny.Run.exe" );
			if ( !File.Exists( runFilepath ) )
			{
				await VS.MessageBox.ShowWarningAsync( "Couldn't find executable: " + runFilepath, "Jenny" );
				return false;
			}

			//await VS.Commands.ExecuteAsync( KnownCommands.View_Output );
			//await VS.Commands.ExecuteAsync( "View.Output" );

			await VS.StatusBar.ShowMessageAsync( $"JENNY: {project.Name}: building..." );
			var configBuild = await BuildProjectAsync( project );
			if ( !configBuild.Success )
			{
				await VS.StatusBar.ShowMessageAsync( $"JENNY: {project.Name}: building... FAILED" );
				return false;
			}

			var info = new ProcessStartInfo
			{
				FileName = runFilepath,
				WorkingDirectory = Path.GetDirectoryName( runFilepath ),
				UseShellExecute = false,
				RedirectStandardOutput = true,
			};

			await VS.StatusBar.ShowMessageAsync( $"JENNY: {project.Name}: getting configs..." );
			var configs = await Task.Run( () => GetConfigsAsync( project.Name, info, configBuild ) );
			if ( !configs.ok )
			{
				await VS.StatusBar.ShowMessageAsync( $"JENNY: {project.Name}: getting configs... FAILED" );
				return false;
			}

			var dals = String.Join( ",", configs.list.Select( o => o.DbContextProject ) );
			await VS.StatusBar.ShowMessageAsync( $"JENNY: {project.Name}: building: {dals}" );
			var dalBuilds = await BuildDalsAsync( configs.list );
			if ( !dalBuilds.ok )
			{
				await VS.StatusBar.ShowMessageAsync( $"JENNY: {project.Name}: building: {dals} FAILED" );
				return false;
			}

			await VS.StatusBar.ShowMessageAsync( $"JENNY: {project.Name}: running..." );
			var ok = await Task.Run( () => RunJennyAsync( solutionDirpath, info, configBuild, dalBuilds.list ) );
			if ( !ok )
			{
				await VS.StatusBar.ShowMessageAsync( $"JENNY: {project.Name}: running... FAILED" );
				return false;
			}

			await VS.StatusBar.ClearAsync();
			return true;
		}

		//-----------------------------------------------------------------------------------------
		// GetConfigsAsync

		static async Task<(bool ok, List<Core.MEF.Config> list)> GetConfigsAsync(
			string projectName,
			ProcessStartInfo info,
			BuildConfigProjectResult configBuild
		)
		{
			info.Arguments =
				"\"-ConfigExe:" + configBuild.ExeFilepath + "\" " +
				"\"-Command:OutputConfig\" ";

			var processExtract = Process.Start( info );

			const int TIMEOUT_SECONDS = 10;

			if ( !processExtract.WaitForExit( TIMEOUT_SECONDS * 1000 ) )
			{
				await VS.MessageBox.ShowWarningAsync( "Failed to extract config from: " + projectName + "\n\nProcess timed out after " + TIMEOUT_SECONDS + " seconds" );
				return (false, null);
			}

			if ( processExtract.ExitCode != 0 )
			{
				await VS.MessageBox.ShowWarningAsync( "Failed to extract config from: " + projectName + "\n\nExit code: " + processExtract.ExitCode );
				return (false, null);
			}

			try
			{
				var xml = new System.Xml.Serialization.XmlSerializer( typeof( List<Core.MEF.Config> ) );

				var configs = (List<Core.MEF.Config>) xml.Deserialize( processExtract.StandardOutput );

				return (true, configs);
			}
			catch ( Exception x )
			{
				await VS.MessageBox.ShowWarningAsync( $"Failed to extract config from: {projectName}\n\nInvalid Configuration\n\n{x}" );
				return (false, null);
			}
		}

		//-----------------------------------------------------------------------------------------
		// BuildDalsAsync

		static async Task<(bool ok, List<BuildConfigProjectResult> list)> BuildDalsAsync(
			List<Core.MEF.Config> configs
		)
		{
			var dalBuilds = new List<BuildConfigProjectResult>();

			var allProjects = await VS.Solutions.GetAllProjectsAsync();

			foreach ( var config in configs )
			{
				var dalProjects = allProjects.Where( o => o.Name == config.DbContextProject ).ToList();

				if ( dalProjects.Count == 0 )
				{
					await VS.MessageBox.ShowWarningAsync( "Failed to find project called: " + config.DbContextProject );
					return (false, null);
				}

				if ( dalProjects.Count > 1 )
				{
					await VS.MessageBox.ShowWarningAsync( $"Found {dalProjects.Count:N0} projects called: " + config.DbContextProject );
					return (false, null);
				}

				var dalBuild = await BuildProjectAsync( dalProjects[0] );

				if ( !dalBuild.Success )
				{
					await VS.MessageBox.ShowWarningAsync( "Failed to build project: " + config.DbContextProject );
					return (false, null);
				}

				dalBuilds.Add( dalBuild );
			}

			return (true, dalBuilds);
		}

		//-----------------------------------------------------------------------------------------
		// RunJennyAsync

		static Task<bool> RunJennyAsync(
			string solutionDirpath,
			ProcessStartInfo info,
			BuildConfigProjectResult configBuild,
			List<BuildConfigProjectResult> dalBuilds
		)
		{
			info.Arguments =
				"\"-ConfigExe:" + configBuild.ExeFilepath + "\" " +
				"\"-Command:RunTransform\" " +
				"\"-SolutionDirpath:" + solutionDirpath + "\" " +
				String.Join(
					" ",
					dalBuilds.Select( dalBuild =>
						"\"-DbContextProject:" + dalBuild.Project + "|" + dalBuild.ExeFilepath + "\""
					)
				)
			;

			var tcs = new TaskCompletionSource<bool>();

			var processRun = new Process { StartInfo = info, EnableRaisingEvents = true };

			processRun.Exited += ( s, e ) => tcs.SetResult( processRun.ExitCode == 0 );

			processRun.Start();

			return tcs.Task;
		}

		//-----------------------------------------------------------------------------------------
		// BuildProjectAsync

		class BuildConfigProjectResult
		{
			public string Project { get; set; }
			public bool Success { get; set; }
			public string ExeFilepath { get; set; }
		}

		static async Task<BuildConfigProjectResult> BuildProjectAsync( Project project )
		{
			if ( project == null ) return new BuildConfigProjectResult { Success = false };

			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			var result = new BuildConfigProjectResult { Project = project.Name };

			result.Success = await VS.Build.BuildProjectAsync( project );

			project.GetItemInfo( out var hierarchy, out _, out _ );
			hierarchy.GetProperty(
				Microsoft.VisualStudio.VSConstants.VSITEMID_ROOT,
				(int) Microsoft.VisualStudio.Shell.Interop.__VSHPROPID.VSHPROPID_ExtObject,
				out var objProject
			);
			var dteProject = objProject as EnvDTE.Project;

			var outputPath = Jenny.GetOutputPath( dteProject );
			var outputFilename = dteProject.Properties.Item( "OutputFileName" ).Value.ToString();

			result.ExeFilepath = Path.Combine( outputPath, outputFilename );

			return result;
		}

		//-----------------------------------------------------------------------------------------

	}
}
