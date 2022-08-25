using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Vsix.Commands
{
	[Command( PackageGuids.guidJennyRunProjectCmdSetString, PackageIds.cmdidJennyRunProject )]
	sealed class RunProjectCommand : BaseCommand<RunProjectCommand>
	{
		protected override void BeforeQueryStatus( EventArgs e )
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var project = ToolkitShared.GetActiveProject();
			if ( project == null ) return;

			var isConfigProject = Jenny.IsConfigProject( project );

			Command.Visible = isConfigProject;
			Command.Enabled = isConfigProject;
		}

		protected override async Task ExecuteAsync( OleMenuCmdEventArgs e )
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			var project = ToolkitShared.GetActiveProject();
			if ( project == null ) return;

			await Runner.RunAsync( project );
		}
	}
}
