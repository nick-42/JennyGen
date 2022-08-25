using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
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
		// TableHeader

		string TableHeader( ITable table, int tabs = 0, bool crlfBefore = true )
		{
			var t = new string( '\t', tabs );

			return
				( crlfBefore ? "\r\n" : "" ) +
				$"{t}//------------------------------------------------------------------------------------------------\r\n" +
				$"{t}// TABLE: " + table.Name + "\r\n" +
				"\r\n"
			;
		}

		//-----------------------------------------------------------------------------------------
		// MarkerAttribute

		string MarkerRootName { get { return "Generated_By_Jenny"; } }

		string Marker_DOG { get { return MarkerRootName + "_DOG"; } }
		string Marker_DAL { get { return MarkerRootName + "_DAL"; } }
		string Marker_REP { get { return MarkerRootName + "_REPO"; } }
		string Marker_CTX { get { return MarkerRootName + "_CTX"; } }
		string Marker_DDL { get { return MarkerRootName + "_DDL"; } }

		string MarkerAttributeClass_DOG { get { return Marker_DOG + "Attribute"; } }
		string MarkerAttributeClass_DAL { get { return Marker_DAL + "Attribute"; } }
		string MarkerAttributeClass_REP { get { return Marker_REP + "Attribute"; } }
		string MarkerAttributeClass_CTX { get { return Marker_CTX + "Attribute"; } }
		string MarkerAttributeClass_DDL { get { return Marker_DDL + "Attribute"; } }

		string MarkerAttribute_DOG { get { return "[" + Marker_DOG + "]"; } }
		string MarkerAttribute_DAL { get { return "[global::" + CoderConfig.Namespace + ".DAL." + Marker_DAL + "]"; } }
		string MarkerAttribute_REP { get { return "[global::" + CoderConfig.Namespace + ".REPO." + Marker_REP + "]"; } }
		string MarkerAttribute_CTX { get { return "[global::" + CoderConfig.Namespace + ".DAL.Context." + Marker_CTX + "]"; } }
		string MarkerAttribute_DDL { get { return "[global::" + CoderConfig.Namespace + ".DDL." + Marker_DDL + "]"; } }

		//-----------------------------------------------------------------------------------------
		// Overridden

		//public class CustomAttributeInfo
		//{
		//	public int ConstructorArgument { get; set; }
		//	public string AttributeName { get; set; }

		//	public Type ArgumentType { get; set; }
		//	public object ArgumentValue { get; set; }
		//}

		//List<CustomAttributeInfo> CustomAttributes<T>( ITable table, string member ) where T : Attribute
		//{
		//	return CustomAttributes( table, member, typeof( T ).Name )
		//		.SelectMany( ca =>
		//			new List<CustomAttributeInfo>()
		//			.Concat( ca.ConstructorArguments
		//				.Select( ( o, i ) => new CustomAttributeInfo
		//					{
		//						ConstructorArgument = i,
		//						ArgumentType = o.ArgumentType,
		//						ArgumentValue = o.Value,
		//					}
		//				)
		//			)
		//			.Concat( ca.NamedArguments
		//				.Select( o => new CustomAttributeInfo
		//					{
		//						AttributeName = o.MemberName,
		//						ArgumentType = o.TypedValue.ArgumentType,
		//						ArgumentValue = o.TypedValue.Value,
		//					}
		//				)
		//			 )
		//		)
		//		.ToList()
		//	;
		//}

		List<MemberInfo> FindMemberInfos( ITable table, string member )
		{
			return table.ClassMembers
				.Where( m => m.DeclaringType != typeof( object ) )
				.Where( m => m.Name == member )
				.ToList()
			;
		}

		List<MemberInfo> FindMethodInfos( ITable table, string member, params Type[] paramTypes )
		{
			return FindMemberInfos( table, member )
				.Select( m => m is MethodInfo mi
					? new { m, ps = mi.GetParameters() }
					: m is ConstructorInfo ci
					? new { m, ps = ci.GetParameters() }
					: null
				)
				.Where( m => m != null )
				.Where( m =>
				{
					if ( paramTypes.Length != m.ps.Length ) return false;

					for ( var i = 0; i < m.ps.Length; i++ )
					{
						if ( paramTypes[i] != m.ps[i].ParameterType ) return false;
					}

					return true;
				} )
				.Select( m => m.m )
				.ToList()
			;
		}

		List<Attribute> MemberAttributes( ITable table, string member, string attributeTypeName )
		{
			var members = FindMemberInfos( table, member );

			return members
				.SelectMany( mi => Attribute.GetCustomAttributes( mi ) )
				.Where( ca => ca.GetType().Name == attributeTypeName )
				.ToList()
			;
		}

		List<CustomAttributeData> MemberCustomAttributes( ITable table, string member, string markerAttributeName )
		{
			var members = FindMemberInfos( table, member );

			return members
				.SelectMany( mi => CustomAttributeData.GetCustomAttributes( mi ) )
				.Where( ca => ca.AttributeType.Name == markerAttributeName )
				.ToList()
			;
		}

		List<CustomAttributeData> MethodCustomAttributes( ITable table, string member, Type[] paramTypes, string markerAttributeName )
		{
			var members = FindMethodInfos( table, member, paramTypes );

			return members
				.SelectMany( mi => CustomAttributeData.GetCustomAttributes( mi ) )
				.Where( ca => ca.AttributeType.Name == markerAttributeName )
				.ToList()
			;
		}

		bool MemberExists( ITable table, string member )
		{
			return FindMemberInfos( table, member ).Any();
		}

		bool MethodExists( ITable table, string member, params Type[] paramTypes )
		{
			return FindMethodInfos( table, member, paramTypes ).Any();
		}

		bool MemberHasAttribute( ITable table, string member, string markerAttributeName )
		{
			return MemberCustomAttributes( table, member, markerAttributeName ).Any();
		}

		bool MethodHasAttribute( ITable table, string member, Type[] paramTypes, string markerAttributeName )
		{
			return MethodCustomAttributes( table, member, paramTypes, markerAttributeName ).Any();
		}

		bool MemberOverriddenInDOG( ITable table, string member ) { return MemberExists( table, member ) && !MemberHasAttribute( table, member, MarkerAttributeClass_DOG ); }
		bool MemberOverriddenInDAL( ITable table, string member ) { return MemberExists( table, member ) && !MemberHasAttribute( table, member, MarkerAttributeClass_DAL ); }
		bool MemberOverriddenInREP( ITable table, string member ) { return MemberExists( table, member ) && !MemberHasAttribute( table, member, MarkerAttributeClass_REP ); }

		bool MethodOverriddenInDOG( ITable table, string member, params Type[] paramTypes ) { return MethodExists( table, member, paramTypes ) && !MethodHasAttribute( table, member, paramTypes, MarkerAttributeClass_DOG ); }
		bool MethodOverriddenInDAL( ITable table, string member, params Type[] paramTypes ) { return MethodExists( table, member, paramTypes ) && !MethodHasAttribute( table, member, paramTypes, MarkerAttributeClass_DAL ); }
		bool MethodOverriddenInREP( ITable table, string member, params Type[] paramTypes ) { return MethodExists( table, member, paramTypes ) && !MethodHasAttribute( table, member, paramTypes, MarkerAttributeClass_REP ); }

		//-----------------------------------------------------------------------------------------
		// Eval

		static string Eval( string template, object data, int tabs = 0 )
		{
			return Common.Eval( template, data, tabs );
		}

		//-----------------------------------------------------------------------------------------

	}
}
