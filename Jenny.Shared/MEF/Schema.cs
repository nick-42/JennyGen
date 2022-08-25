using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Config;
using Jenny.MEF.Schema;

namespace Jenny.Core.MEF
{
	public class Schema : ISchema
	{
		public event EventHandler<IgnoreTableEventArgs> IgnoreTable;
		public event EventHandler<IgnoreColumnEventArgs> IgnoreColumn;

		public event EventHandler<TableEventArgs> InspectTable;
		public event EventHandler<ColumnEventArgs> InspectColumn;

		public bool CallIgnoreTable( ITable table )
		{
			if ( IgnoreTable == null ) return false;

			var args = new IgnoreTableEventArgs
				{
					Table = table,
				}
			;

			IgnoreTable( null, args );

			return args.Ignore;
		}

		public bool CallIgnoreColumn( ITable table, IColumn column )
		{
			if ( IgnoreColumn == null ) return false;

			var args = new IgnoreColumnEventArgs
				{
					Table = table,
					Column = column,
				}
			;

			IgnoreColumn( null, args );

			return args.Ignore;
		}

		public void CallInspectTable( ITable table )
		{
			if ( InspectTable == null ) return;

			var args = new TableEventArgs
				{
					Table = table,
				}
			;

			InspectTable( null, args );
		}

		public void CallInspectColumn( ITable table, IColumn column )
		{
			if ( InspectColumn == null ) return;

			var args = new ColumnEventArgs
				{
					Table = table,
					Column = column,
				}
			;

			InspectColumn( null, args );
		}
	}
}
