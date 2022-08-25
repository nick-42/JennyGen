using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Coder;
using Jenny.MEF.Schema;

using Jenny.Core.Schema;

namespace Jenny.Coder.Internal
{
	partial class DbContextPerStatement
	{

		//-----------------------------------------------------------------------------------------
		// CodeDOG

		bool CodeDOG( CoderConfig config, IModel model, ITable table, ref string s, Action<string> progress )
		{
			const int TABS = 2;
			var dateTimeColumns = table.Columns
				.Where( column =>
					column.PropertyInfo.PropertyType == typeof( DateTime )
					||
					column.PropertyInfo.PropertyType == typeof( DateTime? )
				)
				.Where( column =>
					{
						var hints = MemberCustomAttributes( table, column.Name, "UIHintAttribute" );

						if ( hints.Any( o => o.ConstructorArguments.Count > 0 && "Date".Equals( o.ConstructorArguments[ 0 ].Value ) ) ) return false;

						return true;
					}
				)
				.ToList()
			;

			s += TableHeader( table );

			//----------------------------------------------------------------------------------------------
			// namespace

			s += Eval(
@"namespace #= table.Namespace #
{
", new { config, model, table } );

			//----------------------------------------------------------------------------------------------
			// class

			s += Eval(
@"partial class #= table.Name # :
	global::NBootstrap.Global.IDog<#= table.Name #>,
	global::NBootstrap.Global.IColumnProperties#= table.DogInterfaces #
{
", new { config, model, table }, tabs: 1 );

			//----------------------------------------------------------------------------------------------
			// IdentityObject

			s += Eval(
@"public class RowIdentityClass : global::NBootstrap.Global.IRowIdentityObject<#= table.Name #>
{#
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey || o.IsTimestamp ) ) { #
	public #= c.PropertyType.Name # #= c.Name # { get; set; }#
} #

	public RowIdentityClass() { }

	public RowIdentityClass( #= table.Name # dog )
	{#
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey || o.IsTimestamp ) ) { #
		#= c.Name # = dog.#= c.Name #;#
} #
	}

	public RowIdentityClass(#
{
int i = 0;
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) {
	if ( i++ > 0 ) { #,# } #
		#= c.PropertyType.Name # #= c.name ##
}
foreach( var c in table.Columns.Where( o => o.IsTimestamp ) ) {
	if ( i++ > 0 ) { #,# } #
		#= c.PropertyType.Name # #= c.name # = default( #= c.PropertyType.Name # )#
}
} #
	)
	{#
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey || o.IsTimestamp ) ) { #
		#= c.Name # = #= c.name #;#
} #
	}

	public bool IsNew
	{
		get
		{#
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) { #
			if ( #= c.IsNewPredicate( c.Name ) # ) return true;#
} #
			return false;
		}
	}

	public string PrimaryKeyAsString { get { return """"#
{
int i = 0;
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) {
	if ( i++ > 0 ) { # + ""|""# }
			# + #= String.Format( c.TemplateFormatToString, c.Name ) ##
} 
}
	#; } }

	public static RowIdentityClass CreateFromPrimaryKeyAsString( string primaryKeyAsString )
	{
		var keys = primaryKeyAsString.Split( '|' );
			
		return new RowIdentityClass(#
{
int i = 0;
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) {
	if ( i > 0 ) { #,# } #
			#= String.Format( c.TemplateFormatFromString, ""keys[ "" + i + "" ]"" ) ##
	i++;
} 
} #
		);
	}

	public Expression<Func<#= table.Name #, bool>> Predicate()
	{
		return o =>#
{
int i = 0;
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) {
	if ( i++ > 0 ) { # &&# } #
			o.#= c.Name # == #= c.Name ##
} 
} #
		;
	}

	public override string ToString()
	{
		return PrimaryKeyAsString;
	}

	public string ToLogString()
	{
		return ""#= table.Name #.RowIdentityClass [ "" + PrimaryKeyAsString + "" ]"";
	}
}

public global::NBootstrap.Global.IRowIdentityObject IRowIdentityObject { get { return ITRowIdentityObject; } }
public global::NBootstrap.Global.IRowIdentityObject<#= table.Name #> ITRowIdentityObject { get { return RowIdentityObject; } }

public global::NBootstrap.Global.IRowIdentityObject IRowIdentityObjectFromPrimaryKeyAsString( string primaryKeyAsString )
{
	return ITRowIdentityObjectFromPrimaryKeyAsString( primaryKeyAsString );
}
public global::NBootstrap.Global.IRowIdentityObject<#= table.Name #> ITRowIdentityObjectFromPrimaryKeyAsString( string primaryKeyAsString )
{
	return RowIdentityClass.CreateFromPrimaryKeyAsString( primaryKeyAsString );
}

[NotMapped]
public RowIdentityClass RowIdentityObject
{
	get { return new RowIdentityClass( this ); }
	set
	{#
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey || o.IsTimestamp ) ) { #
		#= c.Name # = value.#= c.Name #;#
} #
	}
}
public void SetPrimaryKeyFromString( string primaryKeyAsString )
{
	RowIdentityObject = RowIdentityClass.CreateFromPrimaryKeyAsString( primaryKeyAsString );
}

public bool IsPrimaryKeyEqual( #= table.Name # o )
{#
foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) { #
	if ( #= c.Name # != o.#= c.Name # ) return false;#
} #
		
	return true;
}

public static IEqualityComparer<#= table.Name #> PrimaryKeyComparer
{
	get => new GenericEqualityComparer<#= table.Name #>( o => o.ToStringPrimaryKeys() );
}

", new { config, model, table }, TABS );

			//----------------------------------------------------------------------------------------------
			// default ctor

			if ( true || !MethodOverriddenInDOG( table, ".ctor", Type.EmptyTypes ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public #= table.Name #()
{
	DefaultConstructorImplementation();
}

partial void DefaultConstructorImplementation();

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// copy ctor

			if ( !MethodOverriddenInDOG( table, ".ctor", table.ClassType ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public #= table.Name #( #= table.Name # o, bool ignoreNull = false ) : this()
{
	if ( ignoreNull && o == null ) return;

	CopyAllColumnsFrom( o );
}

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// ColumnPropertyNames

			if ( !MethodOverriddenInDOG( table, "ColumnPropertyNames", table.ClassType ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
static string[] _StaticColumnPropertyNames;
public static string[] StaticColumnPropertyNames
{
	get
	{
		if ( _StaticColumnPropertyNames == null )
		{
			_StaticColumnPropertyNames = new string[]
			{
# foreach( var c in table.Columns ) {
#				""#= c.Name #"",
# } #			};
		}

		return _StaticColumnPropertyNames;
	}
}
//[Newtonsoft.Json.JsonIgnore]
//public string[] ColumnPropertyNames => StaticColumnPropertyNames;

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// ColumnPropertyInfos

			if ( !MethodOverriddenInDOG( table, "StaticColumnPropertyInfos", table.ClassType ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
static System.Reflection.PropertyInfo[] _StaticColumnPropertyInfos;
public static System.Reflection.PropertyInfo[] StaticColumnPropertyInfos
{
	get
	{
		if ( _StaticColumnPropertyInfos == null )
		{
			var columnPropertyNames = StaticColumnPropertyNames;

			_StaticColumnPropertyInfos = typeof( #= table.Name # )
				.GetProperties()
				.Where( p => columnPropertyNames.Contains( p.Name ) )
				.ToArray()
			;
		}

		return _StaticColumnPropertyInfos;
	}
}
//[Newtonsoft.Json.JsonIgnore]
//public System.Reflection.PropertyInfo[] ColumnPropertyInfos => StaticColumnPropertyInfos;

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// CreateColumnCopy

			if ( !MemberOverriddenInDOG( table, "CreateColumnCopy" ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public #= table.Name # CreateColumnCopy( bool ignoreNull = false, Action<#= table.Name #, #= table.Name #> fnCopy = null )
{
	var now = new #= table.Name #( this, ignoreNull );

	fnCopy?.Invoke( now, this );

	return now;
}

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// CopyAllColumnsFrom

			if ( !MemberOverriddenInDOG( table, "CopyAllColumnsFrom" ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public #= table.Name # CopyAllColumnsFrom( #= table.Name # o )
{
	return CopyKeyColumnsFrom( o ).CopyDataColumnsFrom( o );
}

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// CopyKeyColumnsFrom

			if ( !MemberOverriddenInDOG( table, "CopyKeyColumnsFrom" ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public #= table.Name # CopyKeyColumnsFrom( #= table.Name # o )
{
#	foreach( var c in table.Columns.Where( o => o.IsPrimaryKey || o.IsTimestamp ) )
	{
		if ( c.PropertyType == typeof( byte[] ) )
		{
#
	if ( o.#= c.Name # == null )
	{
		#= c.Name # = null;
	}
	else
	{
		#= c.Name # = new byte[ o.#= c.Name #.Length ];
		Buffer.BlockCopy( o.#= c.Name #, 0, #= c.Name #, 0, o.#= c.Name #.Length );
	}

#		}
		else
		{
#	#= c.Name # = o.#= c.Name #;
#		}
	}

#
	return this;
}

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// CopyDataColumnsFrom

			if ( !MemberOverriddenInDOG( table, "CopyDataColumnsFrom" ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public #= table.Name # CopyDataColumnsFrom( #= table.Name # o )
{
#		foreach( var c in table.Columns.Where( o => !o.IsPrimaryKey && !o.IsTimestamp ) )
	{
#	#= c.Name # = o.#= c.Name #;
#		} 

	if( dateTimeColumns.Any() ) { #
# }
	foreach ( var c in dateTimeColumns ) {
#	#= c.Name #_Local = o.#= c.Name #_Local;
# } #
	return this;
}

", new { config, model, table, dateTimeColumns }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// AreColumnsEqual

			if ( !MemberOverriddenInDOG( table, "AreColumnsEqual" ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public bool AreColumnsEqual( #= table.Name # o )
{
# foreach( var c in table.Columns.Where( o => !o.IsPrimaryKey && !o.IsTimestamp ) )
{
#	if ( #= c.Name # != o.#= c.Name # ) return false;
# }
#
	return true;
}

", new { config, model, table, dateTimeColumns }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// ToStringPrimaryKeys

			if ( !MemberOverriddenInDOG( table, "ToStringPrimaryKeys" ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public string ToStringPrimaryKeys()
{
	return String.Join( "", "", new string[]
	{
		#=
			String.Join( ""\r\n\t\t\t\t"", table.Columns
				.Where( o => o.IsPrimaryKey )
				.Select( o => ""\""\"" + "" + o.Name + "","" )
			)
		#
	} );
}

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// ToString

			if ( !MemberOverriddenInDOG( table, "ToString" ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public override string ToString()
{
	return ""#= table.Name # [ "" + ToStringPrimaryKeys() + "" ]"" +
		String.Join( "","", new string[]
		{
			#=
				String.Join( ""\r\n\t\t\t\t\t"", table.Columns
					.Where( o => o.IncludeInToString )
					.Select( o => ""\"" "" + o.Name + "": \"" + "" + o.Name + "","")
				)
			#
		} )
	;
}

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// ToLogString

			if ( !MemberOverriddenInDOG( table, "ToLogString" ) )
			{
				s += Eval( MarkerAttribute_DOG + @"
public string ToLogString( List<string> mapPropertyNames = null )
{
	bool include( string name ) { return mapPropertyNames == null || mapPropertyNames.Any( o => o == name ); }

	return ""#= table.Name # [ "" + ToStringPrimaryKeys() + "" ]["" +
		String.Join( "","", new string[]
		{
			#=
				String.Join( ""\r\n\t\t\t"", table.Columns
					.Where( o => !o.IsPrimaryKey && !o.IsTimestamp )
					.Select( o => ""include( \"""" + o.Name + ""\"" ) ? \"" "" + o.Name + "": \"" + "" + o.Name + "" : null,"")
				)
			#
		}
		.Where( o => o != null ) ) + "" ]""
	;
}

", new { config, model, table }, TABS );
			}

			//----------------------------------------------------------------------------------------------
			// DateTime Locals

			{
				if ( dateTimeColumns.Count > 0 )
				{
					s += "\t\t// DateTime locals\r\n\r\n";

					foreach ( var column in dateTimeColumns )
					{
						if ( !MemberOverriddenInDOG( table, column.Name + "_Local" ) )
						{
							var mandatory = ( column.PropertyInfo.PropertyType == typeof( DateTime ) );

							s += Eval( MarkerAttribute_DOG + @"
[NotMapped]
public DateTime#= mandatory ? """" : ""?"" # #= column #_Local { get; set; }

", new { mandatory, column = column.Name }, TABS );
						}

						if ( !MemberOverriddenInDOG( table, column.Name + "_GmtToLocal" ) )
						{
							s += Eval( MarkerAttribute_DOG + @"
public void #= column #_GmtToLocal( TimeZoneInfo tzi )
{
	#= column #_Local = TimeZones.GmtToLocal_Property( tzi, #= column # );
}

", new { column = column.Name }, TABS );
						}

						if ( !MemberOverriddenInDOG( table, column.Name + "_LocalToGmt" ) )
						{
							s += Eval( MarkerAttribute_DOG + @"
public void #= column #_LocalToGmt( TimeZoneInfo tzi )
{
	#= column # = TimeZones.LocalToGmt_Property( tzi, #= column #_Local );
}

", new { column = column.Name }, TABS );
						}
					}
				}

				if ( !MemberOverriddenInDOG( table, "All_GmtToLocal" ) )
				{
					s += Eval( MarkerAttribute_DOG + @"
public void All_GmtToLocal( TimeZoneInfo tzi )
{
# foreach ( var column in dateTimeColumns ) { #	#= column.Name #_GmtToLocal( tzi );
# } #	All_GmtToLocal_Partial( tzi );
}
partial void All_GmtToLocal_Partial( TimeZoneInfo tzi );

", new { dateTimeColumns }, TABS );
				}

				if ( !MemberOverriddenInDOG( table, "All_LocalToGmt" ) )
				{
					s += Eval( MarkerAttribute_DOG + @"
public void All_LocalToGmt( TimeZoneInfo tzi )
{
# foreach ( var column in dateTimeColumns ) { #	#= column.Name #_LocalToGmt( tzi );
# } #	All_LocalToGmt_Partial( tzi );
}
partial void All_LocalToGmt_Partial( TimeZoneInfo tzi );

", new { dateTimeColumns }, TABS );
				}
			}

			//----------------------------------------------------------------------------------------------
			// end class

			s = s.Substring( 0, s.Length - 2 );
			s += "\t}\r\n}\r\n";

			//----------------------------------------------------------------------------------------------

			return true;
		}

		//-----------------------------------------------------------------------------------------

	}
}
