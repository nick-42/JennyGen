using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Jenny.MEF.Schema;

namespace Jenny.Core.Schema
{
	public class Model : IModel
	{
		[XmlIgnore]
		public List<ITable> Tables { get; set; } = new List<ITable>();

		public List<Table> XmlTables
		{
			get => Tables.Cast<Table>().ToList();
			set => Tables = value.Cast<ITable>().ToList();
		}
	}
}
