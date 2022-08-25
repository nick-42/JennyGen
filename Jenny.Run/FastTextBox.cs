using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jenny
{
	public partial class FastTextBox : UserControl
	{
		readonly Font _font = new( "Lucida Console", 9.75f, FontStyle.Regular );
		readonly SynchronizationContext _syncContext = null;

		Size _size = Size.Empty;
		bool _autoScroll = true;
		int _verticalScrollPosition = 0;
		int _viewportHeight = 0;

		public FastTextBox()
		{
			InitializeComponent();

			_syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

			BackColor = SystemColors.Window;
			DoubleBuffered = true;
			AutoScroll = false;
			VScroll = true;

			SizeChanged += Handle_SizeChanged;
			Scroll += Handle_Scroll;
			MouseWheel += Handle_MouseWheel;
		}

		void Handle_SizeChanged( object sender, EventArgs e )
		{
			_size = Size;

			Invalidate();
		}

		void Handle_Scroll( object sender, ScrollEventArgs e )
		{
			if ( e.ScrollOrientation != ScrollOrientation.VerticalScroll ) return;

			_autoScroll = false;

			_verticalScrollPosition = e.NewValue;

			Invalidate();
		}

		void Handle_MouseWheel( object sender, MouseEventArgs e )
		{
			_autoScroll = false;

			_verticalScrollPosition = -AutoScrollPosition.Y;

			Invalidate();
		}

		void SetVerticalScroll()
		{
			_syncContext.Post( o =>
			{
				AutoScrollMinSize = new Size( 0, _viewportHeight );

				if ( _autoScroll )
				{
					_verticalScrollPosition = Math.Max( 0, _viewportHeight - Height );

					AutoScrollPosition = new( 0, _verticalScrollPosition );
				}
			}, null );
		}

		//-----------------------------------------------------------------------------------------
		// Text

		readonly object key = new();
		readonly List<string> _lines = new();
		readonly char[] _cr = new[] { '\r', '\n' };

		public void AppendText( string text )
		{
			lock ( key )
			{
				if ( String.IsNullOrWhiteSpace( text ) ) _lines.Add( text );
				else _lines.AddRange( text.Split( _cr, StringSplitOptions.RemoveEmptyEntries ) );
			}

			Invalidate();
		}

		//-----------------------------------------------------------------------------------------
		// Painting

		protected override void OnPaint( PaintEventArgs e )
		{
			base.OnPaint( e );

			DoPaint( e.Graphics );

			SetVerticalScroll();
		}

		void ConcurrentPaint()
		{
			using ( var g = CreateGraphics() ) DoPaint( g );
		}

		void DoPaint( Graphics g )
		{
			lock ( key )
			{
				g.Clear( SystemColors.Window );

				g.TranslateTransform( 0, -_verticalScrollPosition );

				var dy = 1.21f * g.MeasureString( "M", _font ).Height;
				var y = dy;

				var miny = _verticalScrollPosition;
				var maxy = _verticalScrollPosition + _size.Height;

				foreach ( var line in _lines )
				{
					if ( y >= miny && y <= maxy )
					{
						g.DrawString( line, _font, SystemBrushes.WindowText, 0, y );
					}

					y += dy;
				}

				_viewportHeight = (int) ( y + dy );
			}
		}

		//-----------------------------------------------------------------------------------------
	}
}
