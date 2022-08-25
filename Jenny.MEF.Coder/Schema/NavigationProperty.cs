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
	public class NavigationProperty : INavigationProperty
	{
		[XmlIgnore] public PropertyInfo PropertyInfo { get; set; }

		public RelationshipMultiplicity FromRelationshipMultiplicity { get; set; }
		public RelationshipMultiplicity ToRelationshipMultiplicity { get; set; }

		public bool SuppressLinkMethods { get; set; }

		Type _otherTableType = null;
		[XmlIgnore]
		public Type OtherTableType
		{
			get
			{
				if ( _otherTableType != null ) return _otherTableType;

				if ( PropertyInfo.PropertyType.IsGenericType ) return PropertyInfo.PropertyType.GenericTypeArguments[0];

				return PropertyInfo.PropertyType;
			}

			set { _otherTableType = value; }
		}

		public override string ToString()
		{
			return
				FromRelationshipMultiplicity + " " + PropertyInfo.DeclaringType.Name +
				" => " +
				ToRelationshipMultiplicity + " " + PropertyInfo.PropertyType.Name
			;
		}
	}
}
