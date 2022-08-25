using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Core.Engine
{
	static class TypeToSource
	{
		public static string Run( Type type, bool efCore, bool efCoreIncludes )
		{
			var sb = new StringBuilder();

			sb.AppendLine( $"// {type.Name}" );
			sb.AppendLine( $"namespace Jenny" );
			sb.AppendLine( $"{{" );

			if ( efCoreIncludes ) sb.AppendLine(
@"public interface IJennyIncludableQueryable<out TEntity, out TProperty> : IQueryable<TEntity> { }

public class JennyIncludableQueryable<TEntity, TProperty> :
	IJennyIncludableQueryable<TEntity, TProperty>,
	IAsyncEnumerable<TEntity>
{
	internal System.Linq.IQueryable<TEntity> EF { get; init; }

	public JennyIncludableQueryable( System.Linq.IQueryable<TEntity> ef )
	{
		EF = ef;
	}

	public Type ElementType => EF.ElementType;
	public System.Linq.Expressions.Expression Expression => EF.Expression;
	public IQueryProvider Provider => EF.Provider;

	public IEnumerator<TEntity> GetEnumerator() => EF.GetEnumerator();
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => EF.GetEnumerator();

	public IAsyncEnumerator<TEntity> GetAsyncEnumerator( CancellationToken cancellationToken = default )
		=> ( (IAsyncEnumerable<TEntity>) EF ).GetAsyncEnumerator( cancellationToken );
}
" );

			sb.AppendLine( $"public static partial class Jenny_{type.Name}" );
			sb.AppendLine( $"{{" );

			if ( efCoreIncludes ) sb.AppendLine(
@"public static IJennyIncludableQueryable<T0, T1> Include<T0,T1>( 
	this System.Linq.IQueryable<T0> p0, 
	System.Linq.Expressions.Expression<System.Func<T0, T1>> p1 ) where T0 : class
{
	return new JennyIncludableQueryable<T0, T1>(
		Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include<T0,T1>( p0, p1 ) );
}

public static IJennyIncludableQueryable<T0, T2> ThenInclude<T0,T1,T2>( 
	this IJennyIncludableQueryable<T0, System.Collections.Generic.IEnumerable<T1>> p0, 
	System.Linq.Expressions.Expression<System.Func<T1, T2>> p1 ) where T0 : class
{ 
	dynamic o0 = p0;
	return new JennyIncludableQueryable<T0, T2>(
		Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ThenInclude<T0,T1,T2>( o0.EF, p1 ) );
}

public static IJennyIncludableQueryable<T0, T2> ThenInclude<T0,T1,T2>( 
	this IJennyIncludableQueryable<T0, T1> p0, 
	System.Linq.Expressions.Expression<System.Func<T1, T2>> p1 ) where T0 : class
{
	dynamic o0 = p0;
	return new JennyIncludableQueryable<T0, T2>(
		Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ThenInclude<T0,T1,T2>( o0.EF, p1 ) );
}

public static System.Linq.IQueryable<T0> Include<T0>( 
	this System.Linq.IQueryable<T0> p0, 
	System.String p1 ) where T0 : class
{
	return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include<T0>( p0, p1 );
}
" );
			else if ( efCore ) sb.AppendLine(
@"public static System.Linq.IQueryable<T0> Include<T0, T1>(
this System.Linq.IQueryable<T0> p0,
System.Linq.Expressions.Expression<System.Func<T0, T1>> p1 ) where T0 : class
{
	return Includes.SaneIncludes.Split( p0, p1 );
}

public static object IncludeAll<T0>( 
this IEnumerable<T0> p0, 
params System.Linq.Expressions.Expression<System.Func<T0, object>>[] p1 )
where T0 : NBootstrap.Global.IDog<T0>
{
	throw new NotImplementedException( ""IncludeAll can only be used inside an Include( ... )"" );
}

public static object IncludeAll<T0>(
this T0 p0,
params System.Linq.Expressions.Expression<System.Func<T0, object>>[] p1 )
where T0 : NBootstrap.Global.IDog<T0>
{
	throw new NotImplementedException( ""IncludeAll can only be used inside an Include( ... )"" );
}
" );

			var efExcludes = new[] { "Include", "ThenInclude" };

			var methods = type
				.GetMethods( BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly )
				.Where( o => o.IsDefined( typeof( System.Runtime.CompilerServices.ExtensionAttribute ), false ) )
				.Where( o => !efCore || !efExcludes.Contains( o.Name ) )
				.ToList();
			;

			foreach ( var method in methods )
			{
				//var template = method.IsGenericMethod ? method.GetGenericMethodDefinition() : null;
				//var generics = template == null ? new Type[ 0 ] : template.GetGenericArguments();
				var generics = method.GetGenericArguments();

				var aObsolete = method.GetCustomAttribute( typeof( ObsoleteAttribute ), false ) as ObsoleteAttribute;
				var sObsolete = aObsolete == null ? null : $"[System.Obsolete(\"{aObsolete.Message}\")]";

				var gMap = generics.Select( ( o, i ) => new
				{
					o,
					s = $"T{i}",
					cs = o.GenericParameterAttributes,
				} ).ToList();

				var sStatic = method.IsStatic ? " static" : "";
				var sReturn = sType( method.ReturnType );

				var sGenerics = gMap.Count == 0 ? "" : $"<{String.Join( ",", gMap.Select( o => o.s ) )}>";

				var cs = gMap.Where( o => o.cs == GenericParameterAttributes.ReferenceTypeConstraint ).ToList();
				var sConstraints = cs.Count == 0 ? "" : $" where {String.Join( ", ", cs.Select( o => $"{o.s} : class" ) )}";

				var ps = method.GetParameters();
				var sPs = ps.Select( ( o, i ) => $"{sType( o.ParameterType )} p{i}" );
				var sThis = ps.Any() ? "this " : "";

				var sSig = $"public{sStatic} {sReturn} {method.Name}{sGenerics}";
				var sParams = $"( {sThis}{String.Join( ", ", sPs )} )";

				var sBody = sReturn == "void" ? "" : "return ";
				sBody += $"{sType( method.DeclaringType )}.{method.Name}{sGenerics}";
				sBody += $"( {String.Join( ", ", ps.Select( ( o, i ) => $"p{i}" ) )} );";

				if ( sObsolete != null ) sb.AppendLine( sObsolete );
				sb.AppendLine( $"{sSig}{sParams}{sConstraints}" );
				sb.AppendLine( $"{{ {sBody} }}" );

				string sGeneric( Type o )
				{
					if ( o.IsArray ) return $"{gMap.Single( x => o.Name == $"{x.o.Name}[]" ).s}[]";
					if ( o.DeclaringType == null ) return sType( o );
					return gMap.Single( x => x.o == o ).s;
				}

				string sType( Type o )
				{
					var s = sTypeSimple( o );
					var i = s.IndexOf( '`' );
					if ( i < 0 ) return s;
					var gs = o.GetGenericArguments();
					return s.Substring( 0, i ) + $"<{String.Join( ", ", gs.Select( o => sGeneric( o ) ) )}>";

				}
				string sTypeSimple( Type o )
				{
					if ( o == null || o.Name == "Void" ) return "void";
					var g = gMap.SingleOrDefault( x => x.o == o );
					if ( g != null ) return g.s;
					return $"{o.Namespace}.{( o.DeclaringType == null ? "" : $"{o.DeclaringType.Name}." )}{o.Name}";
				}
			}

			sb.AppendLine( $"}}" );
			sb.AppendLine( $"}}" );

			return sb.ToString();
		}
	}
}
