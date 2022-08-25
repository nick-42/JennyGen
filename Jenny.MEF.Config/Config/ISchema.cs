using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Schema;

namespace Jenny.MEF.Config
{
	public interface ISchema
	{
		event EventHandler<IgnoreTableEventArgs> IgnoreTable;
		event EventHandler<IgnoreColumnEventArgs> IgnoreColumn;

		event EventHandler<TableEventArgs> InspectTable;
		event EventHandler<ColumnEventArgs> InspectColumn;
	}

	public class TableEventArgs
	{
		public ITable Table { get; set; }
	}

	public class IgnoreTableEventArgs : TableEventArgs
	{
		public bool Ignore { get; set; }
	}

	public class ColumnEventArgs
	{
		public ITable Table { get; set; }
		public IColumn Column { get; set; }
	}

	public class IgnoreColumnEventArgs : ColumnEventArgs
	{
		public bool Ignore { get; set; }
	}
}
