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
	public class Column : IColumn
	{
		[XmlIgnore] public PropertyInfo PropertyInfo { get; set; }
		[XmlIgnore] public Type PropertyType { get; set; }

		public string Name { get; set; }
		public string name { get { return Char.ToLowerInvariant( Name[ 0 ] ) + Name.Substring( 1 ); } }

		public int Order { get; set; }

		public bool NotMapped { get; set; }

		public bool IsPrimaryKey { get; set; }
		public bool IsTimestamp { get; set; }
		//public string ForeignKey { get; set; }

		public bool Required { get; set; }
		public int? StringLengthMax { get; set; }
		public int? StringLengthMin { get; set; }

		// schems
		public bool IncludeInToString { get; set; }

		public Column()
		{
			Order = Int32.MaxValue;
		}

		public override string ToString()
		{
			return Name;
		}

		class GenericType
		{
			public string Name { get; set; }
			public List<GenericType> GenericTypes { get; set; }

			public static string TypeAsString( Type type )
			{
				var nullable = type.Name == "Nullable`1";

				var s = nullable ? "" : type.Name;
				var g = s.IndexOf( '`' );
				if ( g >= 0 ) s = s.Substring( 0, g );

				var generics = type.GenericTypeArguments;

				if ( generics.Length > 0 )
				{
					if ( !nullable ) s += "<";

					s += String.Join( ", ", generics.Select( generic => TypeAsString( generic ) ) );

					if ( !nullable ) s += ">";
				}

				if ( nullable ) s += "?";

				return s;
			}
		}

		public string TypeAsString { get { return GenericType.TypeAsString( PropertyType ); } }

		public string IsNewPredicate(string prop)
		{
			return PropertyType switch
			{
				var o when o == typeof(Int32) => $"{prop} <= 0",
				var o when o == typeof(String) => $"String.IsNullOrWhiteSpace( {prop} )",
				_ => $"{prop} != default",
			};
		}

		public string ToStringDDL()
		{
			return String.Format( "{0,-30}{1} {2} {3}{4}{5}",
				Name,
				( IsPrimaryKey && IsTimestamp ? " PK TS" : IsPrimaryKey ? " PK" : IsTimestamp ? " TS" : "   " ),
				( Required ? "NOT NULL" : "    NULL" ),
				TypeAsString,
				( StringLengthMin == null ? null : " MIN( " + StringLengthMin + " )" ),
				( StringLengthMax == null ? null : " MAX( " + StringLengthMax + " )" )
			);
		}


		string _TemplateFormatToString = null;
		public string TemplateFormatToString
		{
			get { return _TemplateFormatToString ?? ( PropertyType == typeof( string ) ? "{0}" : "{0}.ToString()" ); }
			set { _TemplateFormatToString = value; }
		}

		string _TemplateFormatFromString = null;
		public string TemplateFormatFromString
		{
			get { return _TemplateFormatFromString ?? ( PropertyType == typeof( string ) ? "{0}" : PropertyType.FullName + ".Parse( {0} )" ); }
			set { _TemplateFormatFromString = value; }
		}

	}
}
