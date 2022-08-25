using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NBootstrap.Global.DOG
{
	public static class TimeZones
	{

		//-----------------------------------------------------------------------------------------
		// data object graph

		static readonly ConcurrentDictionary<Type, List<PropertyInfo>> s_propertyInfos = new();

		static List<PropertyInfo> FindPropertyInfos( Type type )
		{
			if ( !s_propertyInfos.TryGetValue( type, out var pis ) )
			{
				pis = type.GetProperties( BindingFlags.Instance | BindingFlags.Public )
					.Where( o => o.PropertyType != typeof( string ) )
					.Where( o => o.PropertyType != typeof( byte[] ) )
					.Where( o => o.PropertyType != typeof( PropertyInfo[] ) )
					.Where( o => o.PropertyType.IsClass )
					.Where( o => o.Name != "RowIdentityObject" )
					.Where( o => o.GetIndexParameters().Length == 0 )
					.Where( o => !o.CustomAttributes.Any( o => o.AttributeType.Name == "NotMappedAttribute" ) )
					.ToList()
				;

				s_propertyInfos.TryAdd( type, pis );
			}

			return pis;
		}

		public static void GmtToLocal_ObjectGraph( TimeZoneInfo tzi, object root )
		{
			var seen = new Dictionary<object, object>( ReferenceComparer<object>.Instance );

			Visit( root, dog => dog.All_GmtToLocal( tzi ), seen );
		}

		public static void LocalToGmt_ObjectGraph( TimeZoneInfo tzi, object root )
		{
			var seen = new Dictionary<object, object>( ReferenceComparer<object>.Instance );

			Visit( root, dog => dog.All_LocalToGmt( tzi ), seen );
		}

		static void Visit( object o, Action<IDog> fn, Dictionary<object, object> seen )
		{
			// null
			if ( o == null ) return;

			// filter
			var type = o.GetType();
			if ( type == typeof( string ) ) return;
			if ( type == typeof( byte[] ) ) return;
			if ( !type.IsClass ) return;
			if ( type.Name == "RowIdentityObject" ) return;

			// seen
			if ( seen.ContainsKey( o ) ) return;

			seen.Add( o, null );

			// enumerable
			if ( o is IEnumerable iEnumerable )
			{
				foreach ( var element in iEnumerable ) Visit( element, fn, seen );
			}

			// dog
			if ( o is IDog dog ) fn( dog );

			// properties
			foreach ( var property in FindPropertyInfos( o.GetType() ) )
			{
				object value = null;

				try
				{
					value = property.GetValue( o );
				}
				catch ( Exception x )
				{
					Log.Error( "Failed to read property value: '" + property.Name + "'", new LogParams( "o", o ), x );
				}

				Visit( value, fn, seen );
			}
		}

		//-----------------------------------------------------------------------------------------
		// single property

		public static DateTime GmtToLocal_Property( TimeZoneInfo tzi, DateTime gmt )
		{
			return tzi == null ? gmt : TimeZoneInfo.ConvertTimeFromUtc( gmt, tzi );
		}

		public static DateTime? GmtToLocal_Property( TimeZoneInfo tzi, DateTime? gmt )
		{
			return tzi == null ? gmt : gmt == null ? null : TimeZoneInfo.ConvertTimeFromUtc( gmt.Value, tzi );
		}

		public static DateTime LocalToGmt_Property( TimeZoneInfo tzi, DateTime local )
		{
			return tzi == null ? local : TimeZoneInfo.ConvertTimeToUtc( local, tzi );
		}

		public static DateTime? LocalToGmt_Property( TimeZoneInfo tzi, DateTime? local )
		{
			return tzi == null ? local : local == null ? null : TimeZoneInfo.ConvertTimeToUtc( local.Value, tzi );
		}

		//-----------------------------------------------------------------------------------------

	}
}
