using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jenny.Core.Engine.EF6
{
	public static class FindQueryableExtensions
	{
		public static Type Run( ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
		{
			progress ??= new Progress<RunnerProgress>();

			progress.Message();

			progress.Message( "Finding EntityFramework Assembly" );

			//foreach ( var a in AppDomain.CurrentDomain.GetAssemblies() )
			//	progress.Message( a.GetName().Name );

			var aEntityFramework = AppDomain.CurrentDomain
				.GetAssemblies()
				.SingleOrDefault( o => o.GetName().Name == "EntityFramework" );

			if ( aEntityFramework == null )
			{
				progress.Message( "Failed to find EntityFramework Assembly" );
				return null;
			}

			progress.Message( "Found EntityFramework Assembly" );
			progress.Message( "Finding QueryableExtensions Class" );

			var extensions = aEntityFramework.GetType( "System.Data.Entity.QueryableExtensions" );

			progress.Message( $"{( extensions == null ? "Failed to find" : "Found" )} QueryableExtensions Class" );

			return extensions;
		}
	}
}
