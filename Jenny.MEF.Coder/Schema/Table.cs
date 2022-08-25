using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Jenny.MEF.Schema;

namespace Jenny.Core.Schema
{
	public class Table : ITable
	{
		[XmlIgnore] public Type ClassType { get; set; }
		[XmlIgnore] public List<MemberInfo> ClassMembers { get; set; }

		public string NamespaceName { get { return Namespace + "." + Name; } }
		public string Namespace { get; set; }
		public string Name { get; set; }
		public string name { get { return Name[0].ToString().ToLowerInvariant() + Name.Substring( 1 ); } }

		[XmlIgnore] public List<IColumn> Columns { get; set; } = new List<IColumn>();
		[XmlIgnore] public List<INavigationProperty> NavigationProperties { get; set; } = new List<INavigationProperty>();

		public List<Column> XmlColumns
		{
			get => Columns.Cast<Column>().ToList();
			set => Columns = value.Cast<IColumn>().ToList();
		}

		public List<NavigationProperty> XmlNavigationProperties
		{
			get => NavigationProperties.Cast<NavigationProperty>().ToList();
			set => NavigationProperties = value.Cast<INavigationProperty>().ToList();
		}

		public bool SuppressCrudLogging { get; set; }

		public string DogInterfaces { get; set; }

		public string DropDownListSort { get; set; }
		public string DropDownListText { get; set; }

		public override string ToString()
		{
			return Name;
		}

		[XmlIgnore]
		public List<IColumn> PrimaryKeys => Columns.Where( o => o.IsPrimaryKey ).OrderBy( o => o.Order ).ToList();

		public void FixupColumnOrders()
		{
			var sorted = new List<IColumn>();

			sorted.AddRange( Columns.Where( o => o.IsPrimaryKey && !o.IsTimestamp ).Where( o => o.Order < Int32.MaxValue ).OrderBy( o => o.Order ) );
			sorted.AddRange( Columns.Where( o => o.IsPrimaryKey && !o.IsTimestamp ).Where( o => o.Order == Int32.MaxValue ).OrderBy( o => o.Name ) );

			sorted.AddRange( Columns.Where( o => o.IsTimestamp ).Where( o => o.Order < Int32.MaxValue ).OrderBy( o => o.Order ) );
			sorted.AddRange( Columns.Where( o => o.IsTimestamp ).Where( o => o.Order == Int32.MaxValue ).OrderBy( o => o.Name ) );

			sorted.AddRange( Columns.Where( o => !o.IsPrimaryKey && !o.IsTimestamp ).Where( o => o.Order < Int32.MaxValue ).OrderBy( o => o.Order ) );
			sorted.AddRange( Columns.Where( o => !o.IsPrimaryKey && !o.IsTimestamp ).Where( o => o.Order == Int32.MaxValue ).OrderBy( o => o.Name ) );

			Columns = sorted;
		}
	}
}
