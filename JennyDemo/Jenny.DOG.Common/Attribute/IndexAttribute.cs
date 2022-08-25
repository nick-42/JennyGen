using System;

namespace NBootstrap.Global.DOG
{
	[AttributeUsage( AttributeTargets.Property, AllowMultiple = true )]
	public class IndexAttribute : Attribute
	{
		public IndexAttribute( bool isUnique = false )
		{
			IsUnique = isUnique;
		}

		public IndexAttribute( string name, bool isUnique = false, int order = 0 )
		{
			Name = name;
			IsUnique = isUnique;
			Order = order;
		}

		public string Name { get; set; }
		public bool IsUnique { get; set; }
		public int Order { get; set; }
	}
}
