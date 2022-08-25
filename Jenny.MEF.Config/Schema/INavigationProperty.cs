using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.MEF.Schema
{
	public interface INavigationProperty
	{
		PropertyInfo PropertyInfo { get; set; }

		RelationshipMultiplicity FromRelationshipMultiplicity { get; set; }
		RelationshipMultiplicity ToRelationshipMultiplicity { get; set; }

		bool SuppressLinkMethods { get; set; }

		Type OtherTableType { get; set; }

		//IRelationshipEndMember FromEndMember { get; set; }
		//IRelationshipEndMember ToEndMember { get; set; }

		//IRelationshipEndMember OtherEndMember( string thisTableName );
	}
}
