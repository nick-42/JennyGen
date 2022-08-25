using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.Core.Schema;

namespace Jenny.MEF.Coder
{
	public class CoderFile
	{
		public string Filepath { get; set; }
		public string Contents { get; set; }

		public CoderFile()
		{
		}

		public CoderFile( string filepath )
		{
			Filepath = filepath;
		}

		public CoderFile( string filepath, string contents )
		{
			Filepath = filepath;
			Contents = contents;
		}
	}

	public class CoderFiles
	{
		public List<CoderFile> Files { get; private set; }

		public CoderFiles()
		{
			Files = new List<CoderFile>();
		}
	}

	public interface ICoder
	{
		string CoderName { get; }

		bool Code( CoderConfig config, Model model, CoderFiles files, Action<string> progress );
	}
}
