using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NBootstrap.Global
{
	public interface IRowIdentityObject
	{
		string PrimaryKeyAsString { get; }
	}

	public interface IRowIdentityObject<T> : IRowIdentityObject
	{
		Expression<Func<T, bool>> Predicate();
	}

	public interface IDog
	{
		IRowIdentityObject IRowIdentityObject { get; }

		IRowIdentityObject IRowIdentityObjectFromPrimaryKeyAsString( string primaryKeyAsString );

		void SetPrimaryKeyFromString( string primaryKeyAsString );

		void All_GmtToLocal( TimeZoneInfo tzi );
		void All_LocalToGmt( TimeZoneInfo tzi );
	}

	public interface IColumnProperties
	{
		//string[] ColumnPropertyNames { get; }

		//System.Reflection.PropertyInfo[] ColumnPropertyInfos { get; }
	}

	public interface IDog<T> : IDog
	{
		IRowIdentityObject<T> ITRowIdentityObject { get; }

		IRowIdentityObject<T> ITRowIdentityObjectFromPrimaryKeyAsString( string primaryKeyAsString );
	}
}
