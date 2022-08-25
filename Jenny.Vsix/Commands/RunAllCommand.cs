using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Vsix.Commands
{
	[Command( PackageGuids.guidJennyRunAllCmdSetString, PackageIds.cmdidJennyRunAll )]
	sealed class RunAllCommand : BaseCommand<RunAllCommand>
	{
		protected override async Task ExecuteAsync( OleMenuCmdEventArgs e )
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			await Runner.RunAllAsync();
		}
	}
}
