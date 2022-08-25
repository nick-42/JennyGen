using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Vsix
{
	static class VsConstants
	{
		// Project type guids
		internal const string WebApplicationProjectTypeGuid = "{349C5851-65DF-11DA-9384-00065B846F21}";
		internal const string WebSiteProjectTypeGuid = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
		internal const string CsharpProjectTypeGuid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
		internal const string VbProjectTypeGuid = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
		internal const string CppProjectTypeGuid = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
		internal const string FsharpProjectTypeGuid = "{F2A71F9B-5D33-465A-A702-920D77279786}";
		internal const string JsProjectTypeGuid = "{262852C6-CD72-467D-83FE-5EEB1973A190}";
		internal const string WixProjectTypeGuid = "{930C7802-8A8C-48F9-8165-68863BCCD9DD}";
		internal const string LightSwitchProjectTypeGuid = "{ECD6D718-D1CF-4119-97F3-97C25A0DFBF9}";
		internal const string NemerleProjectTypeGuid = "{edcc3b85-0bad-11db-bc1a-00112fde8b61}";
		internal const string InstallShieldLimitedEditionTypeGuid = "{FBB4BD86-BF63-432a-A6FB-6CF3A1288F83}";
		internal const string WindowsStoreProjectTypeGuid = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
		internal const string SynergexProjectTypeGuid = "{BBD0F5D1-1CC4-42fd-BA4C-A96779C64378}";
		internal const string NomadForVisualStudioProjectTypeGuid = "{4B160523-D178-4405-B438-79FB67C8D499}";

		// Copied from EnvDTE.Constants since that type can't be embedded
		internal const string VsProjectItemKindPhysicalFile = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
		internal const string VsProjectItemKindPhysicalFolder = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
		internal const string VsProjectItemKindSolutionFolder = "{66A26720-8FB5-11D2-AA7E-00C04F688DDE}";
		internal const string VsProjectItemKindSolutionItem = "{66A26722-8FB5-11D2-AA7E-00C04F688DDE}";
		internal const string VsWindowKindSolutionExplorer = "{3AE79031-E1BC-11D0-8F78-00A0C9110057}";

		// All unloaded projects have this Kind value
		internal const string UnloadedProjectTypeGuid = "{67294A52-A4F0-11D2-AA88-00C04F688DDE}";

		internal const string NuGetSolutionSettingsFolder = ".nuget";

		// HResults
		internal const int OK = 0;
	}

	public static class Jenny
	{

		//-----------------------------------------------------------------------------------------
		// from NuGet

		const string BinFolder = "Bin";

		public static string GetOutputPath( EnvDTE.Project project )
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			// For Websites the output path is the bin folder
			var outputPath = IsWebSite( project )
				? BinFolder
				: project.ConfigurationManager.ActiveConfiguration.Properties.Item( "OutputPath" ).Value.ToString();

			return Path.Combine( GetFullPath( project ), outputPath );
		}

		public static bool IsWebSite( EnvDTE.Project project )
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			return
				project.Kind != null &&
				project.Kind.Equals( VsConstants.WebSiteProjectTypeGuid, StringComparison.OrdinalIgnoreCase );
		}

		public static string GetFullPath( EnvDTE.Project project )
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if ( IsUnloaded( project ) )
			{
				// To get the directory of an unloaded project, we use the UniqueName property,
				// which is the path of the project file relative to the solution directory.
				var solutionDirectory = Path.GetDirectoryName( project.DTE.Solution.FullName );
				var projectFileFullPath = Path.Combine( solutionDirectory, project.UniqueName );
				return Path.GetDirectoryName( projectFileFullPath );
			}

			var fullPath = GetPropertyValue<string>( project, "FullPath" );
			if ( !String.IsNullOrEmpty( fullPath ) )
			{
				// Some Project System implementations (JS metro app) return the project 
				// file as FullPath. We only need the parent directory
				if ( File.Exists( fullPath ) )
				{
					fullPath = Path.GetDirectoryName( fullPath );
				}
			}
			else
			{
				// C++ projects do not have FullPath property, but do have ProjectDirectory one.
				fullPath = GetPropertyValue<string>( project, "ProjectDirectory" );
			}

			return fullPath;
		}

		public static bool IsUnloaded( EnvDTE.Project project )
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			return VsConstants.UnloadedProjectTypeGuid.Equals( project.Kind, StringComparison.OrdinalIgnoreCase );
		}

		public static T GetPropertyValue<T>( EnvDTE.Project project, string propertyName )
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if ( project.Properties == null ) return default;

			try
			{
				var property = project.Properties.Item( propertyName );
				if ( property != null ) return (T) property.Value;
			}
			catch ( ArgumentException ) { }
			return default;
		}

		//-----------------------------------------------------------------------------------------
		// using Community

		public static bool IsConfigProject( Project project )
		{
			//if ( project == null || project.IsUnloaded() || !project.IsSupported() ) return false;
			if ( project == null ) return false;

			var jennyMefCoder = false;
			var jennyMefConfig = false;

			foreach ( var r in project.References )
			{
				if ( r.Name == "Jenny.MEF.Coder" ) jennyMefCoder = true;
				if ( r.Name == "Jenny.MEF.Config" ) jennyMefConfig = true;
			}

			return ( !jennyMefCoder && jennyMefConfig );
		}

		//-----------------------------------------------------------------------------------------

	}
}
