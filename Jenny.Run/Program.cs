using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jenny.Core;

namespace Jenny.Run
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var args = Environment.GetCommandLineArgs();

			//MessageBox.Show( String.Join( " ", args ) );

			var commands = Core.EXE.CommandLine.Parse( args );

			if ( !commands.IsValid ) return;

			if ( !commands.ShowUserInterface )
			{
				Core.Engine.Runner.Run( commands, true );
			}
			else
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );

				var form = new MainForm();

				var progress = form.RunnerProgress;

				form.Shown += ( s, e ) => Task.Run( () =>
				{
					progress.Message( String.Join( Environment.NewLine, args ) );
					progress.Message();

					Core.Engine.Runner.Run( commands, true, form.RunnerProgress );
				} );

				Application.Run( form );
			}
		}
	}
}
