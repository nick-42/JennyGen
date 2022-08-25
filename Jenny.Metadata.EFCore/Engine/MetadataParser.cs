using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Jenny.Core.Schema;

namespace Jenny.Core.Engine.EFCore
{
	public static class MetadataParser
	{
		public static Model Run( ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
		{
			progress ??= new Progress<RunnerProgress>();

			progress.Message();

			progress.Message( "Loading DbContext Assembly: " + config.DbContextAssemblyFilepath );

			Assembly load( string path )
			{
				progress.Message( $"Loading: {path}" );
				return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath( path );
			}
			//var dirConfig = Path.GetDirectoryName( config.ConfigExeFilepath );
			//Directory.GetFiles( dirConfig, "*.dll", SearchOption.TopDirectoryOnly ).Select( load ).ToList();
			var aDbContext = load( config.DbContextAssemblyFilepath );

			progress.Message( $"Loading DbContext Assembly => {( aDbContext == null ? "NULL" : "OK" )}" );

			try
			{
				progress.Message( "Creating DbContext Object: " + config.DbContextName );
				var tDbContext = aDbContext.GetTypes().Single( o => o.Name == config.DbContextName );
				var oDbContext = Activator.CreateInstance( tDbContext, config.DbContextConnectionString );

				return Run( config, schema, progress, tDbContext, oDbContext );
			}
			catch ( ReflectionTypeLoadException x )
			{
				progress.Message( $"Failed to load type for DbContext: {config.DbContextName} {x}" );

				return null;
			}
		}

		static Model Run(
			ConfigEx config,
			SchemaEx schema,
			IProgress<RunnerProgress> progress,
			Type tDbContext,
			object oDbContext
		)
		{
			var database = new Model();

			config.DbContextNamespaceName = tDbContext.FullName;

			if ( oDbContext is not Microsoft.EntityFrameworkCore.DbContext dbContext )
			{
				progress.Message( $"DbContext is not a DbContext: {config.DbContextName}" );
				return null;
			}

			progress.Message( "Getting Model" );
			var model = dbContext.Model;

			foreach ( var oEntityType in model.GetEntityTypes() )
			{
				var oClrType = (Type) oEntityType.ClrType;

				var table = new Table
				{
					ClassType = oClrType,
					ClassMembers = oClrType.GetMembers().ToList(),

					Namespace = oClrType.Namespace,
					Name = oClrType.Name,
				};

				// columns

				var oColumns = oEntityType.GetProperties();

				var cPrimaryKey = oEntityType.FindPrimaryKey();
				var cPrimaryKeyProperties = cPrimaryKey?.Properties.ToList();
				var keyCount = cPrimaryKeyProperties.Count;

				var cColumns = oEntityType.GetProperties();

				foreach ( var cColumn in cColumns )
				{
					var isPrimaryKey = cColumn.IsPrimaryKey();
					//var isTimestamp = cColumn.IsConcurrencyToken;
					var isTimestamp = false;

					var fluentOrder = cPrimaryKeyProperties?.IndexOf( cColumn );

					var pi = oClrType.GetProperty( cColumn.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

					var column = new Column
					{
						PropertyInfo = pi,

						//PropertyType =
						//	oColumn.IsEnumType
						//	? pi_EdmProperty_PropertyInfo.GetValue( oColumn ).PropertyType
						//	: oColumn.PrimitiveType.ClrEquivalentType
						//,

						PropertyType = pi.PropertyType,

						Name = cColumn.Name,
						Order = fluentOrder ?? Int32.MaxValue,

						NotMapped = false,

						IsPrimaryKey = isPrimaryKey,
						IsTimestamp = isTimestamp,
						//ForeignKey = null,

						Required = !cColumn.IsNullable,
						StringLengthMax = cColumn.GetMaxLength(),
						StringLengthMin = null,

						IncludeInToString = false,
					};

					if ( schema.CallIgnoreColumn( table, column ) ) continue;

					schema.CallInspectColumn( table, column );

					table.Columns.Add( column );
				}

				// navigation properties

				foreach ( var oNavProp in oEntityType.GetNavigations() )
				{
					var fk = oNavProp.ForeignKey;

					var navProp = new NavigationProperty
					{
						PropertyInfo = oNavProp.PropertyInfo,

						FromRelationshipMultiplicity = thisRelationshipMultiplicity( oNavProp ),
						ToRelationshipMultiplicity = otherRelationshipMultiplicity( oNavProp ),
					};

					table.NavigationProperties.Add( navProp );
				}

				//table.FixupColumnOrders();

				if ( schema.CallIgnoreTable( table ) ) continue;

				schema.CallInspectTable( table );

				database.Tables.Add( table );
			}

			database.Tables = database.Tables.OrderBy( o => o.Name ).ToList();

			return database;

			Jenny.MEF.Schema.RelationshipMultiplicity thisRelationshipMultiplicity( Microsoft.EntityFrameworkCore.Metadata.INavigation navigation )
			{
				if ( navigation.IsCollection ) return Jenny.MEF.Schema.RelationshipMultiplicity.Many;
				if ( navigation.ForeignKey.IsRequired ) return Jenny.MEF.Schema.RelationshipMultiplicity.One;
				return Jenny.MEF.Schema.RelationshipMultiplicity.Zoo;
			}

			Jenny.MEF.Schema.RelationshipMultiplicity otherRelationshipMultiplicity( Microsoft.EntityFrameworkCore.Metadata.INavigation navigation )
			{
				if ( navigation.ForeignKey.IsRequiredDependent ) return Jenny.MEF.Schema.RelationshipMultiplicity.One;
				return Jenny.MEF.Schema.RelationshipMultiplicity.Zoo;
			}
		}
	}
}
