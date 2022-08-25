using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBootstrap.Global
{
	public interface IRepo
	{
		void RecreateReadContext();

		ITransaction BeginTransaction();
	}
}
