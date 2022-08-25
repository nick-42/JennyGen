using System;
using System.Collections.Generic;
using System.Text;

namespace JennyDemo.DOG
{
	public static class Glob
	{
		public const string ConnectionString =
			@"server=.\SQLEXPRESS; database=JennyDemo; " +
			@"Integrated Security=True; Encrypt=False; " +
			@"MultipleActiveResultSets=True;";
	}
}
