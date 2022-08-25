using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.MEF.Config
{
	public interface IAdapter
	{
		void Config( IConfig config );
		void Schema( ISchema schema );
	}
}
