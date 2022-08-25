using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.MEF.Schema
{
	public interface IModel
	{
		List<ITable> Tables { get; set; }
	}
}
