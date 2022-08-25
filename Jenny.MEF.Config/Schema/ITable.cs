using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.MEF.Schema
{
	public interface ITable
	{
		Type ClassType { get; set; }
		List<MemberInfo> ClassMembers { get; set; }

		string NamespaceName { get; }
		string Namespace { get; set; }
		string Name { get; set; }
		string name { get; }

		List<IColumn> Columns { get; set; }
		List<INavigationProperty> NavigationProperties { get; set; }

		List<IColumn> PrimaryKeys { get; }

		bool SuppressCrudLogging { get; set; }

		string DogInterfaces { get; set; }

		string DropDownListSort { get; set; }
		string DropDownListText { get; set; }
	}
}
