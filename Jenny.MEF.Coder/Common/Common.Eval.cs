using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Jenny.MEF.Coder
{
	public static class Common
	{
		static string CSharpName( Type type )
		{
			var isGeneric = type.IsGenericType;

			var name = type.Name;
			var iGenericTypeParameters = name.IndexOf( "`" );
			if ( iGenericTypeParameters > 0 ) name = name.Substring( 0, iGenericTypeParameters );

			var genericTypeParameters = !isGeneric ? null : "<" + String.Join( ", ", type.GenericTypeArguments.Select( t => CSharpName( t ) ) ) + ">";

			return type.Namespace + "." + name + genericTypeParameters;
		}

		public static string Eval( string template, object data, int tabs = 0 )
		{
			var props = data.GetType().GetProperties();

			var parameters = props.Select(
				prop => new
				{
					TypeName = CSharpName( prop.PropertyType ),
					prop.Name,
					Value = prop.GetValue( data ),
				} )
				.ToList()
			;

			var fragments = template.Split( new[] { "#" }, StringSplitOptions.None );

			var text = true;

			var cs = String.Empty;

			foreach ( var fragment in fragments )
			{
				if ( text )
				{
					cs += @"sb.Append( @""" + fragment.Replace( "\"", "\"\"" ) + "\" );\r\n";

					text = false;
				}
				else
				{
					if ( fragment.Length == 0 )
					{
						cs += "sb.Append( \"#\" );";
					}
					else
					{
						cs += fragment[0] switch
						{
							'=' => @"sb.Append( " + fragment.Substring( 1 ).Trim() + " );\r\n",
							_ => fragment + ";\r\n",
						};
					}

					text = true;
				}
			}

			var main =
@"
using System;
using System.Linq;
using System.Text;

namespace Eval
{
 public class Main
 {
  public static string Eval( " + String.Join( ", ", parameters.Select( parameter => parameter.TypeName + " " + parameter.Name ) ) + @" )
  {
   var sb = new StringBuilder();

" + cs + @"

   return sb.ToString();
  }
 }
}
";

			var s = ExecuteCode( main, "Eval", "Main", "Eval", parameters.Select( o => o.Value ).ToArray() ).ToString();

			if ( tabs <= 0 ) return s;

			var t = new string( '\t', tabs );

			return String.Join( "\r\n", s
				.Split( new[] { "\r\n" }, StringSplitOptions.None )
				.Select( line => String.IsNullOrWhiteSpace( line ) ? "" : $"{t}{line}" )
			);
		}

		static string CacheFilepath( string code )
		{
			var dir = Glob.AssemblyCacheDirpath;
			if ( String.IsNullOrWhiteSpace( dir ) ) return null;
			if ( String.IsNullOrWhiteSpace( code ) ) return null;

			var hash = String.Concat( System.Security.Cryptography.MD5
				.Create()
				.ComputeHash( Encoding.UTF8.GetBytes( code ) )
				.Take( 4 )
				.Select( o => o.ToString( "X2" ) )
			);

			return Path.Combine( dir, $"x{hash}.cache" );
		}

		static Assembly LoadAssemby( string code )
		{
			var cache = CacheFilepath( code );
			if ( String.IsNullOrWhiteSpace( cache ) ) return null;
			if ( !File.Exists( cache ) )
			{
				//Glob.ProgressMessage?.Invoke( $"Cache missed: '{cache}'" );
				return null;
			}

			try
			{
#if !NETSTANDARD
				return Assembly.LoadFile( cache );
#else
				return AssemblyLoadContext.Default.LoadFromAssemblyPath( cache );
#endif
			}
			catch ( Exception x )
			{
				Glob.ProgressMessage?.Invoke( "Failed to load cached assembly: " + cache );
				Glob.ProgressMessage?.Invoke( "Failed to load cached assembly: " + x );
			}

			try
			{
				File.Delete( cache );
			}
			catch ( Exception x )
			{
				Glob.ProgressMessage?.Invoke( "Failed to delete bad assembly: " + x );
			}

			return null;
		}

		static Assembly BuildAssembly( string code )
		{
			System.Diagnostics.Debug.WriteLine( "BuildAssembly: " + s_cache.Count );

			var loaded = LoadAssemby( code );
			if ( loaded != null )
			{
				//Glob.ProgressMessage?.Invoke( $"Loaded cache: '{loaded.Location}'" );
				return loaded;
			}

			var cache = CacheFilepath( code );
			var save = !String.IsNullOrWhiteSpace( cache );

#if !NETSTANDARD
			var provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider( "CSharp" );

			var cp = new System.CodeDom.Compiler.CompilerParameters();

			cp.ReferencedAssemblies.Add( "System.dll" );
			cp.ReferencedAssemblies.Add( "System.Core.dll" );
			cp.ReferencedAssemblies.Add( "Jenny.MEF.Config.dll" );
			cp.ReferencedAssemblies.Add( "Jenny.MEF.Coder.dll" );

			cp.GenerateExecutable = false;

			if ( save )
			{
				cp.GenerateInMemory = false;
				cp.OutputAssembly = cache;
			}
			else
			{
				cp.GenerateInMemory = true;
			}

			var results = provider.CompileAssemblyFromSource( cp, code );

			if ( results.Errors.HasErrors )
			{
				var errors = new StringBuilder( "Template Compiler Errors :\r\n\r\n" );

				var i = 1;
				foreach ( var line in code.Split( '\n' ) ) errors.AppendLine( ( i++ ).ToString( "D4" ) + " " + line );

				errors.AppendLine();

				foreach ( System.CodeDom.Compiler.CompilerError error in results.Errors )
				{
					errors.AppendFormat( "Line {0},{1}: {2}\r\n", error.Line, error.Column, error.ErrorText );
				}

				throw new Exception( errors.ToString() );
			}

			return results.CompiledAssembly;
#else
			var options = CSharpParseOptions.Default
				.WithLanguageVersion( LanguageVersion.Latest );

			var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree( code, options );

			var references = new MetadataReference[]
			{
				MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
				MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
				MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
				MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Collections")).Location),
				MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
				MetadataReference.CreateFromFile( "Jenny.MEF.Config.dll" ),
				MetadataReference.CreateFromFile( "Jenny.MEF.Coder.dll" ),
			};

			var compilation = CSharpCompilation.Create(
				Path.GetFileName( cache ),
				new[] { parsedSyntaxTree },
				references,
				new CSharpCompilationOptions(
					OutputKind.DynamicallyLinkedLibrary,
					optimizationLevel: OptimizationLevel.Debug,
					assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default
				)
			);

			using var peStream = new MemoryStream();
			var result = compilation.Emit( peStream );

			if ( !result.Success )
			{
				Glob.ProgressMessage?.Invoke( "Template Compiler Errors :\r\n\r\n" );
				//var errors = new StringBuilder( "Template Compiler Errors :\r\n\r\n" );

				var i = 0;
				var eol = code.Contains( '\r' ) ? 2 : 1;
				foreach ( var line in code.Replace( "\r", "" ).Split( '\n' ) )
				{
					var m = i.ToString( "D5" ) + " " + line;
					Glob.ProgressMessage?.Invoke( m );
					//errors.AppendLine( m );
					i += line.Length + eol;
				}

				Glob.ProgressMessage?.Invoke( "" );
				//errors.AppendLine();

				var failures = result.Diagnostics.Where( diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error );

				foreach ( var error in failures )
				{
					var m = $"{error.Location}: {error.Id}: {error.GetMessage()}\r\n";
					Glob.ProgressMessage?.Invoke( m );
					//errors.AppendFormat( m );
				}

				throw new ApplicationException( "Template Compiler Errors: bailing..." );
			}

			if ( save )
			{
				//Glob.ProgressMessage?.Invoke( $"Saving cache: '{cache}'" );
				File.WriteAllBytes( cache, peStream.ToArray() );
			}

			peStream.Position = 0;

			var assembly = AssemblyLoadContext.Default.LoadFromStream( peStream );

			return assembly;
#endif
		}

		static readonly Dictionary<string, Assembly> s_cache = new();

		static object ExecuteCode(
			string code,
			string @namespace,
			string @class,
			string @method,
			params object[] args
		)
		{
			if ( !s_cache.TryGetValue( code, out var assembly ) ) s_cache.Add( code, assembly = BuildAssembly( code ) );

			var type = assembly.GetType( @namespace + "." + @class );

			var mi = type.GetMethod( @method );

			return mi.Invoke( null, args );
		}
	}
}
