using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Coder;

using Jenny.Core.Schema;

namespace Jenny.Coder.Internal
{
	[Export(typeof(ICoder))]
	[ExportMetadata("Name", Coder.DbContextPerStatement.CoderName)]
	[PartCreationPolicy(CreationPolicy.Shared)]
	partial class DbContextPerStatement : ICoder
	{
		public CoderConfig CoderConfig { get; set; }

		public string CoderName { get { return Coder.DbContextPerStatement.CoderName; } }

		public bool Code(CoderConfig config, Model model, CoderFiles files, Action<string> progress)
		{
			CoderConfig = config;

			var dogFilepath = config.CoderSettings[Settings.OUTPUT_FILEPATH_DOG]?.ToString();
			var dalFilepath = config.CoderSettings[Settings.OUTPUT_FILEPATH_DAL]?.ToString();
			var repFilepath = config.CoderSettings[Settings.OUTPUT_FILEPATH_REP]?.ToString();
			var ctxFilepath = config.CoderSettings[Settings.OUTPUT_FILEPATH_CTX]?.ToString();
			var ddlFilepath = config.CoderSettings[Settings.OUTPUT_FILEPATH_DDL]?.ToString();

			var dogContent = HeaderDOG(config, model);
			var dalContent = HeaderDAL(config, model);
			var repContent = HeaderREP(config, model);
			var ctxContent = HeaderCTX(config, model);
			var ddlContent = HeaderDDL(config, model);

			if (!CommonREP(config, model, ref repContent, progress)) return false;
			if (!CommonCTX(config, model, ref ctxContent, progress)) return false;

			progress("");

			foreach (var table in model.Tables)
			{
				var sb = new StringBuilder();

				sb.AppendLine("----------------------------------------------------------------");
				sb.AppendLine("TABLE: " + table.Name);
				sb.AppendLine(new String('-', ("TABLE: " + table.Name).Length));

				foreach (var column in table.Columns) sb.AppendLine(column.ToStringDDL());

				progress(sb.ToString());

				if (!CodeDOG(config, model, table, ref dogContent, progress)) return false;
				if (!CodeDAL(config, model, table, ref dalContent, progress)) return false;
				if (!CodeREP(config, model, table, ref repContent, progress)) return false;
				if (!CodeCTX(config, model, table, ref ctxContent, progress)) return false;
				if (!CodeDDL(config, model, table, ref ddlContent, progress)) return false;
			}

			dogContent += "\r\n//-----------------------------------------------------------------------------------------------\r\n";
			dalContent += "\r\n\t//-----------------------------------------------------------------------------------------------\r\n\r\n}\r\n";
			repContent += "\r\n\t\t//-----------------------------------------------------------------------------------------------\r\n\r\n\t}\r\n}\r\n";
			ctxContent += "\r\n\t\t\t//-----------------------------------------------------------------------------------------------\r\n\r\n\t\t}\r\n\t}\r\n}\r\n";
			ddlContent += "\r\n\t//-----------------------------------------------------------------------------------------------\r\n\r\n}\r\n";

			if (dogFilepath != null) files.Files.Add(new CoderFile(dogFilepath, dogContent));
			if (dalFilepath != null) files.Files.Add(new CoderFile(dalFilepath, dalContent));
			if (repFilepath != null) files.Files.Add(new CoderFile(repFilepath, repContent));
			if (ctxFilepath != null) files.Files.Add(new CoderFile(ctxFilepath, ctxContent));
			if (ddlFilepath != null) files.Files.Add(new CoderFile(ddlFilepath, ddlContent));

			return true;
		}

		//-----------------------------------------------------------------------------------------

	}
}
