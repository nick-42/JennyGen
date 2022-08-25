using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.MEF.Schema
{
	public interface IColumn
	{
		PropertyInfo PropertyInfo { get; set; }
		Type PropertyType { get; set; }

		string Name { get; set; }
		string name { get; }

		int Order { get; set; }

		bool NotMapped { get; set; }

		bool IsPrimaryKey { get; set; }
		bool IsTimestamp { get; set; }
		//string ForeignKey { get; set; }

		bool Required { get; set; }
		int? StringLengthMax { get; set; }
		int? StringLengthMin { get; set; }

		// schems
		bool IncludeInToString { get; set; }

		string IsNewPredicate(string prop);
		string ToStringDDL();

		string TemplateFormatToString { get; set; }
		string TemplateFormatFromString { get; set; }
	}
}
