using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBootstrap.Global
{
	public static class DictionaryOfStringObject
	{
		static readonly List<string> s_concat = new List<string>
			{
				"class",
				"style",
			}
		;

		public static Dictionary<string, object> ToDictionary( object values )
		{
			var dictionary = new Dictionary<string, object>();

			Merge( dictionary, values );

			return dictionary;
		}

		public static void Merge( IDictionary<string, object> dictionary, object values )
		{
			if ( values == null ) return;

			foreach ( var property in values.GetType().GetProperties() )
			{
				var name = property.Name.Replace( '_', '-' );

				var oValue = property.GetValue( values );
				var sValue = oValue?.ToString();

				if ( s_concat.Any( o => String.Equals( o, name, StringComparison.OrdinalIgnoreCase ) ) )
				{
					dictionary.TryGetValue( name, out var oOld );
					var sOld = oOld?.ToString();

					dictionary[ name ] = String.Join( " ", new[] { sOld, sValue }.Where( o => o != null ) );
				}
				else
				{
					dictionary[ name ] = sValue;
				}
			}
		}

		public static object ToAnonObject( IDictionary<string, object> dictionary )
		{
			var o = new ExpandoObject();

			var c = (ICollection<KeyValuePair<string, object>>) o;

			foreach ( var kvp in dictionary ) c.Add( new KeyValuePair<string, object>( kvp.Key, kvp.Value ) );

			return o;
		}

		public static string ToString( IDictionary<string, object> dictionary )
		{
			var s = String.Join( " ", dictionary.Select( kvp => kvp.Key + "=\"" + kvp.Value + "\"" ) );

			return s;
		}
	}
}
