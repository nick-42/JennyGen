using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Core.Engine.EFCore
{
	public static class FindQueryableExtensions
	{
		public static Type Run( ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
		{
			progress ??= new Progress<RunnerProgress>();

			progress.Message();

			progress.Message( "Finding EntityFrameworkCore Assembly" );

			//foreach ( var a in AppDomain.CurrentDomain.GetAssemblies() )
			//	progress.Message( a.GetName().Name );

			var aEntityFramework = AppDomain.CurrentDomain
				.GetAssemblies()
				.SingleOrDefault( o => o.GetName().Name == "Microsoft.EntityFrameworkCore" );

			if ( aEntityFramework == null )
			{
				progress.Message( "Failed to find EntityFrameworkCore Assembly" );
				return null;
			}

			progress.Message( "Found EntityFrameworkCore Assembly" );
			progress.Message( "Finding EntityFrameworkQueryableExtensions Class" );

			var extensions = aEntityFramework.GetType( "Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions" );

			progress.Message( $"{( extensions == null ? "Failed to find" : "Found" )} EntityFrameworkQueryableExtensions Class" );

			return extensions;
		}
	}
}
