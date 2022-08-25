using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Coder;
using Jenny.MEF.Schema;

using Jenny.Core.Schema;

namespace Jenny.Coder.Internal
{
	partial class DbContextPerStatement
	{

		//-----------------------------------------------------------------------------------------
		// CodeDAL

		bool CodeDAL( CoderConfig config, IModel model, ITable table, ref string s, Action<string> progress )
		{
			s += TableHeader( table, 1 );

			// class
			s += Eval(
@"	partial class #= table.Name #
	{
", new { config, model, table } );

			// end class
			s = s.Substring( 0, s.Length - 2 );
			s += " }\r\n";

			return true;
		}

		//-----------------------------------------------------------------------------------------

	}
}
