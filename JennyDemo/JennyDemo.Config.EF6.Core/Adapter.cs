using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using Jenny.MEF.Config;
using Jenny.Coder;

namespace JennyDemo.Config
{
	[Export( typeof( IAdapter ) )]
	[ExportMetadata( "Name", "JennyDemo_EF6_Core_Adapter" )]
	[PartCreationPolicy( CreationPolicy.Shared )]
	public class Adapter : IAdapter
	{
		public void Config( IConfig config )
		{
			// input
			config.DbContextBuild = DbContextBuild.EF6Core;
			config.DbContextProject = "JennyDemo.DAL.EF6.Core";
			config.DbContextName = "JennyContext";
			config.DbContextConnectionString = "Server=.;Database=x;";
			config.LoadAssemblies = new List<string>
			{
				@"JennyDemo.DOG\bin\Debug\netstandard2.1\JennyDemo.DOG.dll",
				@"JennyDemo.DAL.EF6.Core\bin\Debug\net6.0\Jenny.DAL.Common.dll",
				@"JennyDemo.DAL.EF6.Core\bin\Debug\net6.0\Jenny.DAL.EF6.dll",
				@"JennyDemo.EF6.Core\bin\Debug\net6.0\EntityFramework.dll",
				@"JennyDemo.EF6.Core\bin\Debug\net6.0\EntityFramework.SqlServer.dll",
				//@"JennyDemo.EF6.Core\bin\Debug\net6.0\System.Data.SqlClient.dll",
			};

			// output
			config.Namespace = "JennyDemo";
			config.CoderName = DbContextPerStatement.CoderName;
			config.QueryableExtensionsFilepath = "_QueryableExtensions.cs";

			// coder
			config
				.DbContextPerStatement()
				.Output_Filepath_DOG( @"JennyDemo.DOG\_DOG.cs" )
				.Output_Filepath_DAL( @"JennyDemo.DAL.EF6.Core\_DAL.cs" )
				.Output_Filepath_REP( @"JennyDemo.DAL.EF6.Core\_REPO.cs" )
				.Output_Filepath_CTX( @"JennyDemo.DAL.EF6.Core\_CTX.cs" )
				.Output_Filepath_DDL( @"JennyDemo.EF6.Core\_DDL.cs" )

				.Output_Namespace_Aspect( "NBootstrap.EF" )

				.Output_Namespace_REP( "JennyDemo.DAL.EF6.Core" )
				.Output_ClassName_REP( "JennyRepo" )
				.Output_ClassFullName_LoginUserToken( "JennyDemo.DOG.LoginUserToken" )
			;

			// runner
			config.KeepProgressWindowOpen = false;
		}

		public void Schema( ISchema schema )
		{
			schema.IgnoreTable += Common.Schema_IgnoreTable;
			schema.IgnoreColumn += Common.Schema_IgnoreColumn;
			schema.InspectTable += Common.Schema_InspectTable;
			schema.InspectColumn += Common.Schema_InspectColumn;

			schema.IgnoreTable += Schema_IgnoreTable;
		}

		public static void Schema_IgnoreTable( object sender, IgnoreTableEventArgs e )
		{
			//if ( e.Table.Name == "AuditRequest" ) e.Ignore = true;
			//if ( e.Table.Name == "Company" ) e.Ignore = true;
			//if ( e.Table.Name == "Culture" ) e.Ignore = true;
			//if ( e.Table.Name == "SystemConfig" ) e.Ignore = true;
			//if ( e.Table.Name == "User" ) e.Ignore = true;
			//if ( e.Table.Name == "UserOption" ) e.Ignore = true;
			//if ( e.Table.Name == "UserOptionGrid" ) e.Ignore = true;
		}
	}
}
