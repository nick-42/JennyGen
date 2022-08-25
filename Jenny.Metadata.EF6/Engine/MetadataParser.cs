using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Jenny.Core.Schema;

namespace Jenny.Core.Engine.EF6
{
	public static class MetadataParser
	{
		public static Model Run( ConfigEx config, SchemaEx schema, IProgress<RunnerProgress> progress )
		{
			progress ??= new Progress<RunnerProgress>();

			progress.Message();

			progress.Message( "Loading DbContext Assembly: " + config.DbContextAssemblyFilepath );
#if !NETSTANDARD
			var aDbContext = Assembly.LoadFrom( config.DbContextAssemblyFilepath );
#else
			Assembly load( string path )
			{
				progress.Message( $"Loading: {path}" );
				return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath( path );
			}
			//var dirConfig = Path.GetDirectoryName( config.ConfigExeFilepath );
			//Directory.GetFiles( dirConfig, "*.dll", SearchOption.TopDirectoryOnly ).Select( load ).ToList();
			var aDbContext = load( config.DbContextAssemblyFilepath );
#endif
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

			progress.Message( "Getting ObjectContext" );
			var tIObjectContextAdapter = tDbContext.GetInterface( "IObjectContextAdapter" );
			var pObjectContext = tIObjectContextAdapter.GetProperty( "ObjectContext" );
			dynamic oObjectContext = pObjectContext.GetValue( oDbContext );

			progress.Message( "Getting MetadataWorkspace" );
			dynamic oMetadataWorkspace = oObjectContext.MetadataWorkspace;

			progress.Message( "Finding EntityFramework Assembly" );
			var aEntityFramework = AppDomain.CurrentDomain.GetAssemblies().Single( o => o.GetName().Name == "EntityFramework" );
			var tAllEntityFramework = aEntityFramework.GetTypes();

			progress.Message( "Reflecting on EntityFramework Assembly" );

			var tMetadataWorkspace = (Type) oMetadataWorkspace.GetType();
			var miMetadataWorkspace_GetMap = tMetadataWorkspace.GetMethods( BindingFlags.Instance | BindingFlags.NonPublic ).Single( o => o.Name == "GetMap" && o.GetParameters().First().ParameterType.Name == "GlobalItem" );

			var tPrimitivePropertyConfiguration = tAllEntityFramework.Single( o => !o.Name.StartsWith( "<>" ) && o.FullName.Contains( "ModelConfiguration.Configuration.Properties.Primitive.PrimitivePropertyConfiguration" ) );

			// enums

			var tDataSpace = tAllEntityFramework.Single( o => o.Name == "DataSpace" );
			dynamic eDataSpace_OSpace = Enum.Parse( tDataSpace, "OSpace" );
			dynamic eDataSpace_CSpace = Enum.Parse( tDataSpace, "CSpace" );
			dynamic eDataSpace_OCSpace = Enum.Parse( tDataSpace, "OCSpace" );

			var tBuiltInTypeKind = tAllEntityFramework.Single( o => o.Name == "BuiltInTypeKind" );
			dynamic eBuiltInTypeKind_EntityType = Enum.Parse( tBuiltInTypeKind, "EntityType" );
			dynamic eBuiltInTypeKind_EntitySet = Enum.Parse( tBuiltInTypeKind, "EntitySet" );
			dynamic eBuiltInTypeKind_AssociationSet = Enum.Parse( tBuiltInTypeKind, "AssociationSet" );

			var tConcurrencyMode = tAllEntityFramework.Single( o => o.Name == "ConcurrencyMode" );
			dynamic eConcurrencyMode_Fixed = Enum.Parse( tConcurrencyMode, "Fixed" );

			var tRelationshipMultiplicity = tAllEntityFramework.Single( o => o.Name == "RelationshipMultiplicity" );
			dynamic eRelationshipMultiplicity_Many = Enum.Parse( tRelationshipMultiplicity, "Many" );
			dynamic eRelationshipMultiplicity_One = Enum.Parse( tRelationshipMultiplicity, "One" );
			dynamic eRelationshipMultiplicity_Zoo = Enum.Parse( tRelationshipMultiplicity, "ZeroOrOne" );
			Jenny.MEF.Schema.RelationshipMultiplicity mapRelationshipMultiplicity( dynamic value )
			{
				if ( eRelationshipMultiplicity_Many.Equals( value ) ) return Jenny.MEF.Schema.RelationshipMultiplicity.Many;
				if ( eRelationshipMultiplicity_One.Equals( value ) ) return Jenny.MEF.Schema.RelationshipMultiplicity.One;
				if ( eRelationshipMultiplicity_Zoo.Equals( value ) ) return Jenny.MEF.Schema.RelationshipMultiplicity.Zoo;

				throw new InvalidCastException( "Unknown " + tRelationshipMultiplicity.FullName + ": " + value );
			}

			progress.Message( "Getting Object Model" );
			dynamic objectItemCollection = oMetadataWorkspace.GetItemCollection( eDataSpace_OSpace );
			var oItems = (IEnumerable<dynamic>) oMetadataWorkspace.GetItems( eDataSpace_OSpace );
			var oEntityTypes = (IEnumerable<dynamic>) oItems.Where( o => eBuiltInTypeKind_EntityType.Equals( o.BuiltInTypeKind ) ).ToList();

			progress.Message( "Getting Conceptual Model" );
			var cItems = (IEnumerable<object>) oMetadataWorkspace.GetItems( eDataSpace_CSpace );
			dynamic cEntityContainer = cItems.Single( o => o.GetType().Name == "EntityContainer" );

			progress.Message( "Getting Object <-> Conceptual Mapping" );
			var ocItems = (IEnumerable<object>) oMetadataWorkspace.GetItems( eDataSpace_OCSpace );

			progress.Message( "Getting Entity Sets" );
			var cBaseEntitySets = (IEnumerable<object>) cEntityContainer.BaseEntitySets;

			var cEntitySets = cBaseEntitySets.Where( ( dynamic o ) => eBuiltInTypeKind_EntitySet.Equals( o.BuiltInTypeKind ) ).ToList();
			var cAssociationSets = cBaseEntitySets.Where( ( dynamic o ) => eBuiltInTypeKind_AssociationSet.Equals( o.BuiltInTypeKind ) ).ToList();

			foreach ( var oEntityType in oEntityTypes )
			{
				var oClrType = (Type) objectItemCollection.GetClrType( oEntityType );

				var table = new Table
				{
					ClassType = oClrType,
					ClassMembers = oClrType.GetMembers().ToList(),

					Namespace = oClrType.Namespace,
					Name = oClrType.Name,
				};

				// columns

				var oColumns = (IEnumerable<dynamic>) oEntityType.Properties;

				var ocMapping = (object) miMetadataWorkspace_GetMap.Invoke( oMetadataWorkspace, new object[] { oEntityType, eDataSpace_OCSpace } );

				var pEdmType = ocMapping.GetType().GetProperty( "EdmType", BindingFlags.Instance | BindingFlags.NonPublic );

				dynamic cEntityType = pEdmType.GetValue( ocMapping );

				var cKeys = (IEnumerable<object>) cEntityType.KeyMembers;
				var keyCount = cKeys.Count();

				var cColumns = (IEnumerable<dynamic>) cEntityType.Properties;

				foreach ( var oColumn in oColumns )
				{
					var cColumn = cColumns.Single( c => c.Name == oColumn.Name );

					var isPrimaryKey = cKeys.Contains( (object) cColumn );
					var isTimestamp = eConcurrencyMode_Fixed.Equals( cColumn.ConcurrencyMode );

					var configurationMetadataProperties =
						(IEnumerable<object>)
						( (IEnumerable<dynamic>) cColumn.MetadataProperties )
							.Where( o => o.Name == "Configuration" )
							.Select( o => (object) o.Value )
							.Where( o => o.GetType() == tPrimitivePropertyConfiguration || o.GetType().IsSubclassOf( tPrimitivePropertyConfiguration ) )
							.ToList()
					;
					var fluentOrder = configurationMetadataProperties.Max( o =>
						{
							var pColumnOrder = o.GetType().GetProperties().SingleOrDefault( x => x.Name == "ColumnOrder" );
							if ( pColumnOrder == null ) return null;
							return ( pColumnOrder.GetValue( o ) as int? );
						}
					);

					var pi = oClrType.GetProperty( oColumn.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );

					var column = new Column
					{
						PropertyInfo = pi,

						//PropertyType =
						//	oColumn.IsEnumType
						//	? pi_EdmProperty_PropertyInfo.GetValue( oColumn ).PropertyType
						//	: oColumn.PrimitiveType.ClrEquivalentType
						//,

						PropertyType = pi.PropertyType,

						Name = oColumn.Name,
						Order = fluentOrder ?? Int32.MaxValue,

						NotMapped = false,

						IsPrimaryKey = isPrimaryKey,
						IsTimestamp = isTimestamp,
						//ForeignKey = null,

						Required = !oColumn.Nullable,
						StringLengthMax = oColumn.MaxLength,
						StringLengthMin = null,

						IncludeInToString = false,
					};

					if ( schema.CallIgnoreColumn( table, column ) ) continue;

					schema.CallInspectColumn( table, column );

					table.Columns.Add( column );
				}

				// navigation properties

				foreach ( var oNavProp in oEntityType.NavigationProperties )
				{
					var from = oNavProp.FromEndMember;
					var to = oNavProp.ToEndMember;

					var navProp = new NavigationProperty
					{
						PropertyInfo = table.ClassType.GetProperty( oNavProp.Name ),

						FromRelationshipMultiplicity = mapRelationshipMultiplicity( from.RelationshipMultiplicity ),
						ToRelationshipMultiplicity = mapRelationshipMultiplicity( to.RelationshipMultiplicity ),
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
		}
	}
}
