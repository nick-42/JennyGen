using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Jenny.Core;

namespace Jenny
{
	public partial class MainForm : Form
	{
		public Progress<RunnerProgress> RunnerProgress = null;

		public MainForm()
		{
			InitializeComponent();

			FormClosing += ( s, e ) =>
				{
					Properties.Settings.Default.MainFormPosition =
						Location.X + ";" + Location.Y + ";" +
						Size.Width + ";" + Size.Height
					;

					Properties.Settings.Default.Save();
				}
			;

			var mainFormPosition = Properties.Settings.Default.MainFormPosition;

			if ( mainFormPosition != null )
			{
				var values = mainFormPosition.Split( new[] { ";" }, StringSplitOptions.RemoveEmptyEntries );
				if ( values.Length == 4 )
				{
					if ( !Int32.TryParse( values[ 0 ], out var x ) ) goto failMainFormPosition;
					if ( !Int32.TryParse( values[ 1 ], out var y ) ) goto failMainFormPosition;
					if ( !Int32.TryParse( values[ 2 ], out var w ) ) goto failMainFormPosition;
					if ( !Int32.TryParse( values[ 3 ], out var h ) ) goto failMainFormPosition;

					Location = new Point( x, y );
					Size = new Size( w, h );
				}
			}

failMainFormPosition:

			RunnerProgress = new Progress<RunnerProgress>( HandleProgress );

			//Load += ( s, e ) => Task.Run( () => AddLines() );
		}

		void HandleProgress( RunnerProgress progress )
		{
			if ( progress.Quit )
			{
				Close();
				return;
			}

			fastTextBox1.AppendText( progress.Message );
		}

		//async Task AddLines()
		//{
		//	//return;
		//	foreach ( var line in OUTPUT.Split( new[] { "\r\n" }, StringSplitOptions.None ) )
		//	{
		//		//Invoke( () => fastTextBox1.AppendText( line ) );
		//		fastTextBox1.AppendText( $"OUTPUT: {line}" );
		//		//await Task.Delay( 1 );
		//	}
		//}
	}
}
