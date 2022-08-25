using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Coder;
using Jenny.MEF.Schema;

using Jenny.Core.Schema;

namespace Jenny.Coder.Internal
{
	partial class DbContextPerStatement
	{

		//-----------------------------------------------------------------------------------------
		// CommonREP

		bool CommonREP( CoderConfig config, IModel model, ref string s, Action<string> progress )
		{
			s += Eval(

@"
//------------------------------------------------------------------------------------------------------------------------------
// #= repoClassname #

public partial class #= repoClassname # : IDisposable
{
	public static Func<TimeZoneInfo, TimeZoneInfo> PopulateTimeZoneInfo { get; set; }

	//--------------------------------------------------------------------------------------------------------------------------

	Lazy<global::#= aspectNamespace #.Connection> _Connection = null;

	TimeZoneInfo __TimeZoneInfo = null;

	TimeZoneInfo TimeZoneInfo { get { return __TimeZoneInfo ?? ( __TimeZoneInfo = ( PopulateTimeZoneInfo == null ? null : PopulateTimeZoneInfo( null ) ) ); } }

	//--------------------------------------------------------------------------------------------------------------------------
	// ctors

	public #= repoClassname #( TimeZoneInfo timeZoneInfo = null )
	{
		_Connection = new( () => new global::#= aspectNamespace #.Connection() );

		__TimeZoneInfo = timeZoneInfo;
	}

	public #= repoClassname #( string connectionString, TimeZoneInfo timeZoneInfo = null )
	{
		_Connection = new( () => new global::#= aspectNamespace #.Connection( connectionString ) );

		__TimeZoneInfo = timeZoneInfo;
	}

	//--------------------------------------------------------------------------------------------------------------------------
	// dispose

	public void Dispose()
	{
		Dispose( true );

		GC.SuppressFinalize( this );
	}

	public virtual void Dispose( bool disposing )
	{
		if ( disposing )
		{
			if ( _OwnedDbContexts != null )
			{
				var xs = new List<Exception>();

				foreach ( var ctx in _OwnedDbContexts )
				{
					try { ctx.Dispose(); }
					catch ( Exception x ) { xs.Add( x ); }
				}

				_OwnedDbContexts = null;

				if ( xs.Count > 0 ) throw new AggregateException( xs );
			}

			if ( _Connection != null && _Connection.IsValueCreated )
			{
				_Connection.Value.Dispose();
				_Connection = null;
			}
		}
	}

	//--------------------------------------------------------------------------------------------------------------------------
	// DbContext

	List<DbContext> _OwnedDbContexts = new List<DbContext>();

	DbContext CreateDbContext( bool owned )
	{
		var ctx = new global::#= config.DbContextNamespaceName #( _Connection.Value, false );

		if ( owned ) _OwnedDbContexts.Add( ctx );

		return ctx;
	}

	public void RecreateReadContext() { }

	//--------------------------------------------------------------------------------------------------------------------------
	// Transactions

	public NBootstrap.Global.ITransaction BeginTransaction()
	{
		return _Connection.Value.BeginTransaction();
	}

	//--------------------------------------------------------------------------------------------------------------------------
	// IContext

	public interface IContext : IDisposable
	{
	}

	class Context : IContext
	{
		public DbContext DbContext { get; set; }

		public #= repoClassname # TrackRepo { get; set; }

		public void Dispose()
		{
			Dispose( true );

			GC.SuppressFinalize( this );
		}

		public void Dispose( bool disposing )
		{
			if ( disposing )
			{
				if ( DbContext != null )
				{
					DbContext.Dispose();
					DbContext = null;
				}
			}

			if ( TrackRepo != null )
			{
				TrackRepo._TrackContext = null;
				TrackRepo.UseTrackContext = false;
			}
		}
	}

	internal DbContext DbContext( IContext ctx )
	{
		var context = ctx as Context;

		if ( context == null ) throw new UnexpectedException( ""Illegal IContext object"" );

		return context.DbContext;
	}

	public IContext CreateContext()
	{
		return new Context { DbContext = CreateDbContext( false ) };
	}

	public bool UseTrackContext { get; set; } = false;
	Context _TrackContext = null;
	public IContext TrackContext => UseTrackContext ? _TrackContext : null;

	public IContext StartTrackContext()
	{
		UseTrackContext = true;

		return _TrackContext = new Context { DbContext = CreateDbContext( false ), TrackRepo = this };
	}
", new
	{
		config,
		model,
		repoClassname = config.CoderSettings[ Settings.OUTPUT_CLASSNAME_REP ],
		aspectNamespace = config.CoderSettings[ Settings.OUTPUT_NAMESPACE_ASPECT ],
	},
	tabs: 1
);

			return true;
		}

		//-----------------------------------------------------------------------------------------
		// CodeREP

		bool CodeREP( CoderConfig config, IModel model, ITable table, ref string s, Action<string> progress )
		{
			var loginUserTokenType = "" + config.CoderSettings[ Settings.OUTPUT_CLASSFULLNAME_LUT ];
			var loginUserTokenSet = !String.IsNullOrWhiteSpace( loginUserTokenType );

			s += TableHeader( table, 2 );

			// class
			s += Eval(

@"public #= tableClassname # #= table.Name # { get { return new #= tableClassname #( this ); } }

public partial class #= tableClassname #
{
	readonly #= repoClassname # _Repo = null;

	internal #= tableClassname #( #= repoClassname # repo ) { _Repo = repo; }

	//--------------------------------------------------------------------------------------------------------------------------
	// write hooks

	public static HookHandler<#= dog #> OnBeforeCreate = delegate { };
	public static HookHandler<#= dog #> OnBeforeUpdate = delegate { };
	public static HookHandler<#= dog #> OnBeforeDelete = delegate { };

	public static HookHandler<#= dog #> OnAfterCreate = delegate { };
	public static HookHandler<#= dog #> OnAfterUpdate = delegate { };
	public static HookHandler<#= dog #> OnAfterDelete = delegate { };

	//---------------------------------------------------------------------------------------------
	// Read

	public virtual IQueryable<#= dog #> Read( [CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0 )
	{
		var trackContext = _Repo.TrackContext;

		if ( trackContext != null ) return Read( trackContext, name, file, line );

		return new NBootstrap.EF.QueryTranslator<#= dog #>(
			_Repo.TimeZoneInfo,
			_Repo.CreateDbContext( true ).Set<#= dog #>().#= ( ( int ) config.DbContextBuild ) < 3 ? ""AsNoTracking"" : ""AsNoTrackingWithIdentityResolution"" #(),
			name, file, line
		);
	}

	public virtual IQueryable<#= dog #> Read( IContext ctx, [CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0 )
	{
		return new NBootstrap.EF.QueryTranslator<#= dog #>(
			_Repo.TimeZoneInfo,
			_Repo.DbContext( ctx ).Set<#= dog #>(),
			name, file, line
		);
	}

	//---------------------------------------------------------------------------------------------
	// CrudLog

	string MS( System.Diagnostics.Stopwatch sw ) { return ""( "" + sw.Elapsed.TotalMilliseconds.ToString( ""0"" ) + "" ms ) ""; }

	string Filename( string file ) { return String.IsNullOrWhiteSpace( file ) ? """" : System.IO.Path.GetFileName( file ); }

	string CrudLog( string name, string file, int line ) { return "" "" + name + ""() "" + Filename( file ) + ""["" + line + ""]""; }

	//---------------------------------------------------------------------------------------------
	// Upsert

	public virtual #= dog # Upsert(#= lutInput #
		#= dog # template,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		return template.RowIdentityObject.IsNew
			? Create(#= lutCall # template, suppressHooks, name, file, line )
			: UpdateAllColumns(#= lutCall # template, suppressHooks, name, file, line )
		;
	}

	//---------------------------------------------------------------------------------------------
	// Create

	public virtual #= dog # Create(#= lutInput #
		#= dog # template,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			return CreateCore( ctx,#= lutCall # template, suppressHooks, name, file, line );
		}
	}

	public virtual #= dog # Create(
		IContext ctx,#= lutInput #
		#= dog # template,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		return CreateCore( _Repo.DbContext( ctx ),#= lutCall # template, suppressHooks, name, file, line );
	}

	internal virtual #= dog # CreateCore(
		DbContext ctx,#= lutInput #
		#= dog # template,
		bool suppressHooks,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		var copy = new #= dog #().CopyAllColumnsFrom( template );

		ctx.Set<#= dog #>().Add( copy );

		if ( !suppressHooks ) OnBeforeCreate( this, _Repo, token, copy, null, null );

		var sw = System.Diagnostics.Stopwatch.StartNew();
		int rows;
		try
		{
			rows = ctx.SaveChanges();
		}
		finally
		{
			# if ( table.SuppressCrudLogging ) { #//# } #Log.Info( ""REPO CREATE"" + MS( sw ) + copy.ToLogString() + CrudLog( name, file, line ), raw: true );
		}

		if ( rows != 1 ) throw new NBootstrap.IncorrectRowCountException( ""#= table.Name #.CreateCore modified "" + rows + "" rows"" );

		copy.All_GmtToLocal( _Repo.TimeZoneInfo );

		if ( !suppressHooks ) OnAfterCreate( this, _Repo, token, copy, null, null );

		return copy;
	}

	//---------------------------------------------------------------------------------------------
	// DetachExistingEntity

	public virtual void DetachExistingEntity(
		IContext ctx,
		#= dog # template
	)
	{
		DetachExistingEntity( _Repo.DbContext( ctx ), template );
	}

	internal virtual void DetachExistingEntity(
		DbContext ctx,
		#= dog # template
	)
	{
		foreach ( var old in ctx.Set<#= dog #>().Local.Where( o => template.IsPrimaryKeyEqual( o ) ).ToList() )
		{
			ctx.Entry( old ).State = EntityState.Detached;
		}
	}

	//---------------------------------------------------------------------------------------------
	// UpdateAllColumns

	public virtual #= dog # UpdateAllColumns(#= lutInput #
		#= dog # template,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			return UpdateAllColumnsCore( ctx,#= lutCall # template, suppressHooks, name, file, line );
		}
	}

	public virtual #= dog # UpdateAllColumns(
		IContext ctx,#= lutInput #
		#= dog # template,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		return UpdateAllColumnsCore( _Repo.DbContext( ctx ),#= lutCall # template, suppressHooks, name, file, line );
	}

	internal virtual #= dog # UpdateAllColumnsCore(
		DbContext ctx,#= lutInput #
		#= dog # template,
		bool suppressHooks,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		DetachExistingEntity( ctx, template );

		var copy = new #= dog #().CopyAllColumnsFrom( template );

		ctx.Entry( copy ).State = EntityState.Modified;

		if ( !suppressHooks )
		{
			var builder = new UpdateMapEntryColumnBuilder<#= dog #>();

			OnBeforeUpdate( this, _Repo, token, copy, null, builder );

			builder.UpdateMap( copy );
		}

		var sw = System.Diagnostics.Stopwatch.StartNew();
		int rows;
		try
		{
			rows = ctx.SaveChanges();
		}
		finally
		{
			# if ( table.SuppressCrudLogging ) { #//# } #Log.Info( ""REPO UPDATE ALL"" + MS( sw ) + copy.ToLogString() + CrudLog( name, file, line ), raw: true );
		}

		if ( rows != 1 ) throw new NBootstrap.IncorrectRowCountException( ""#= table.Name #.UpdateAllColumnsCore modified "" + rows + "" rows"" );

		copy.All_GmtToLocal( _Repo.TimeZoneInfo );

		if ( !suppressHooks ) OnAfterUpdate( this, _Repo, token, copy, null, null );

		return copy;
	}

	//---------------------------------------------------------------------------------------------
	// UpdateMapColumns UpdateMapEntryColumnBuilder

	public #= dog # UpdateMapColumns(#= lutInput #
		#= dog # template,
		Action<UpdateMapEntryColumnBuilder<#= dog #>> map,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		var builder = new UpdateMapEntryColumnBuilder<#= dog #>();

		map( builder );

		return UpdateMapColumns(#= lutCall # template, builder.UpdateMap( template ), suppressHooks, name, file, line );
	}

	//---------------------------------------------------------------------------------------------
	// UpdateMapColumns

	public virtual #= dog # UpdateMapColumns(#= lutInput #
		#= dog # template,
		UpdateMap<#= dog #> map,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			return UpdateMapColumnsCore( ctx,#= lutCall # template, map, suppressHooks, name, file, line );
		}
	}

	public virtual #= dog # UpdateMapColumns(
		IContext ctx,#= lutInput #
		#= dog # template,
		UpdateMap<#= dog #> map,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		return UpdateMapColumnsCore( _Repo.DbContext( ctx ),#= lutCall # template, map, suppressHooks, name, file, line );
	}

	internal virtual #= dog # UpdateMapColumnsCore(
		DbContext ctx,#= lutInput #
		#= dog # template,
		UpdateMap<#= dog #> map,
		bool suppressHooks,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		DetachExistingEntity( ctx, template );

		var copy = new #= dog #().CopyAllColumnsFrom( template );

		var entry = ctx.Entry( copy );

		entry.State = EntityState.Unchanged;

		var mapPropertyNames = new List<string>();

		foreach ( var property in map.DataEntries )
		{
			mapPropertyNames.Add( property.PropertyName );

			entry.Property( property.PropertyName ).IsModified = true;
		}

		if ( !suppressHooks )
		{
			var builder = new UpdateMapEntryColumnBuilder<#= dog #>();

			OnBeforeUpdate( this, _Repo, token, copy, map, builder );

			foreach ( var property in builder.UpdateMap( copy ).DataEntries )
			{
				mapPropertyNames.Add( property.PropertyName );

				entry.Property( property.PropertyName ).IsModified = true;
			}
		}
# if( ( ( int ) config.DbContextBuild ) < 3 ) { #
		var oldValidateOnSaveEnabled = ctx.Configuration.ValidateOnSaveEnabled;
# } #
		try
		{
# if( ( ( int ) config.DbContextBuild ) < 3 ) { #
			ctx.Configuration.ValidateOnSaveEnabled = false;
# } #
			var sw = System.Diagnostics.Stopwatch.StartNew();
			int rows;
			try
			{
				rows = ctx.SaveChanges();
			}
			finally
			{
				# if ( table.SuppressCrudLogging ) { #//# } #Log.Info( ""REPO UPDATE MAP"" + MS( sw ) + copy.ToLogString( mapPropertyNames ) + CrudLog( name, file, line ), raw: true );
			}

			if ( rows != 1 ) throw new NBootstrap.IncorrectRowCountException( ""#= table.Name #.UpdateMapColumnsCore modified "" + rows + "" rows"" );
		}
		finally
		{
# if( ( ( int ) config.DbContextBuild ) < 3 ) { #
			ctx.Configuration.ValidateOnSaveEnabled = oldValidateOnSaveEnabled;
# } #
		}

		entry.Reload();

		copy.All_GmtToLocal( _Repo.TimeZoneInfo );

		if ( !suppressHooks ) OnAfterUpdate( this, _Repo, token, copy, map, null );

		return copy;
	}

	//---------------------------------------------------------------------------------------------
	// Delete identity

	public virtual void Delete(#= lutInput #
		#= dog #.RowIdentityClass identity,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			DeleteIdentityCore( ctx,#= lutCall # identity, suppressHooks, name, file, line );
		}
	}

	public virtual void Delete(
		IContext ctx,#= lutInput #
		#= dog #.RowIdentityClass identity,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		DeleteIdentityCore( _Repo.DbContext( ctx ),#= lutCall # identity, suppressHooks, name, file, line );
	}

	internal virtual void DeleteIdentityCore(
		DbContext ctx,#= lutInput #
		#= dog #.RowIdentityClass identity,
		bool suppressHooks,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		var copy = new #= dog # { RowIdentityObject = identity };

		DetachExistingEntity( ctx, copy );

		ctx.Entry( copy ).State = EntityState.Deleted;

		if ( !suppressHooks ) OnBeforeDelete( this, _Repo, token, copy, null, null );

		var sw = System.Diagnostics.Stopwatch.StartNew();
		int rows;
		try
		{
			rows = ctx.SaveChanges();
		}
		finally
		{
			# if ( table.SuppressCrudLogging ) { #//# } #Log.Info( ""REPO DELETE"" + MS( sw ) + identity.ToLogString() + CrudLog( name, file, line ), raw: true );
		}

		if ( rows != 1 ) throw new NBootstrap.IncorrectRowCountException( ""#= table.Name #.DeleteIdentityCore modified "" + rows + "" rows"" );

		if ( !suppressHooks ) OnAfterDelete( this, _Repo, token, copy, null, null );
	}

	//---------------------------------------------------------------------------------------------
	// Delete template

	public virtual void Delete(#= lutInput #
		#= dog # template,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			DeleteTemplateCore( ctx,#= lutCall # template, suppressHooks, name, file, line );
		}
	}

	public virtual void Delete(
		IContext ctx,#= lutInput #
		#= dog # template,
		bool suppressHooks = false,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		DeleteTemplateCore( _Repo.DbContext( ctx ),#= lutCall # template, suppressHooks, name, file, line );
	}

	internal virtual void DeleteTemplateCore(
		DbContext ctx,#= lutInput #
		#= dog # template,
		bool suppressHooks,
		[CallerMemberName] string name = null, [CallerFilePath] string file = null, [CallerLineNumber] int line = 0
	)
	{
		DetachExistingEntity( ctx, template );

		var copy = new #= dog #().CopyAllColumnsFrom( template );

		ctx.Entry( copy ).State = EntityState.Deleted;

		if ( !suppressHooks ) OnBeforeDelete( this, _Repo, token, copy, null, null );

		var sw = System.Diagnostics.Stopwatch.StartNew();
		int rows;
		try
		{
			rows = ctx.SaveChanges();
		}
		finally
		{
			# if ( table.SuppressCrudLogging ) { #//# } #Log.Info( ""REPO DELETE"" + MS( sw ) + copy.ToLogString() + CrudLog( name, file, line ), raw: true );
		}

		if ( rows != 1 ) throw new NBootstrap.IncorrectRowCountException( ""#= table.Name #.DeleteTemplateCore modified "" + rows + "" rows"" );

		if ( !suppressHooks ) OnAfterDelete( this, _Repo, token, copy, null, null );
	}

	//---------------------------------------------------------------------------------------------
", new
	{
		config,
		model,
		table,
		repoClassname = config.CoderSettings[ Settings.OUTPUT_CLASSNAME_REP ],
		tableClassname = table.Name + "Table",
		lutInput = ( !loginUserTokenSet ? null : "\r\n\t\tglobal::" + loginUserTokenType + " token," ),
		lutCall = ( !loginUserTokenSet ? null : " token," ),
		dog = "global::" + table.NamespaceName,
	},
	tabs: 2
);

			if ( !CodeLinks( config, model, table, ref s, progress ) ) return false;

			// end class
			//s = s.Substring( 0, s.Length - 2 );
			s += "\r\n\t\t}\r\n";

			return true;
		}

		//-----------------------------------------------------------------------------------------
		// CodeLinks

		bool CodeLinks( CoderConfig config, IModel model, ITable table, ref string s, Action<string> _ )
		{
			var loginUserTokenType = "" + config.CoderSettings[ Settings.OUTPUT_CLASSFULLNAME_LUT ];
			var loginUserTokenSet = !String.IsNullOrWhiteSpace( loginUserTokenType );

			var many2manys = table.NavigationProperties
				.Where( o =>
					o.FromRelationshipMultiplicity == RelationshipMultiplicity.Many
					&&
					o.ToRelationshipMultiplicity == RelationshipMultiplicity.Many
					&&
					!o.SuppressLinkMethods
				)
				.ToList()
			;

			if ( many2manys.Count > 0 )
			{
				s +=

@"			// Links
			//---------------------------------------------------------------------------------------------

			//---------------------------------------------------------------------------------------------
"
				;
			}

			foreach ( var many2many in many2manys )
			{
				var collection = many2many.PropertyInfo.Name;

				var other = model.Tables.Single( o => o.ClassType == many2many.OtherTableType );

				//TODO: link Overwrite methods for linked tables with > 1 pk

				if ( other.PrimaryKeys.Count == 1 )
				{
					var oPrimaryKey = other.PrimaryKeys.Single();

					s += Eval(

@"	// #= table.Name # -> #= other.Name #
	//---------------------------------------------------------------------------------------------
#
{
	var ensure = ""Ensure"" + other.Name + ""Link"";
	var overwrite = ""Overwrite"" + other.Name + ""Links"";
	var removeAll = ""RemoveAll"" + other.Name + ""Links"";
	var remove = ""Remove"" + other.Name + ""Links"";
	var linkedIdsType = oPrimaryKey.PropertyType.Name;
	var linkedId = oPrimaryKey.name;
	var linkedIds = oPrimaryKey.name + ""s"";
#
	//---------------------------------------------------------------------------------------------
	// #= ensure #

	public virtual void #= ensure #(#= lutInput #
		#= dog # template,
		#= linkedIdsType # #= linkedId #
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			#= ensure #Core( ctx,#= lutCall # template, #= linkedId # );
		}
	}

	public virtual void #= ensure #(
		IContext ctx,#= lutInput #
		#= dog # template,
		#= linkedIdsType # #= linkedId #
	)
	{
		#= ensure #Core( _Repo.DbContext( ctx ),#= lutCall # template, #= linkedId # );
	}

	internal virtual void #= ensure #Core(
		DbContext ctx,#= lutInput #
		#= dog # template,
		#= linkedIdsType # #= linkedId #
	)
	{
		DetachExistingEntity( ctx, template );

		var #= table.name #Set = ctx.Set<global::#= table.NamespaceName #>();
		var #= other.name #Set = ctx.Set<global::#= other.NamespaceName #>();

		var olds =
			#= table.name #Set
			.Where( o =>#
{
	int i = 0;
	foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) {
		if ( i++ > 0 ) { #&& # } #
				o.#= c.Name # == template.#= c.Name ##
	} 
} #
			)
			.SelectMany( o => o.#= collection #.Select( x => x.#= oPrimaryKey.Name # ) )
			.ToList()
		;

		if ( !olds.Any( id => id == #= linkedId # ) )
		{
			var insert = new global::#= other.NamespaceName # { #= oPrimaryKey # = #= linkedId # };

			var copy = new #= dog #().CopyKeyColumnsFrom( template );

			#= table.name #Set.Attach( copy );

			var oc = ( (System.Data.Entity.Infrastructure.IObjectContextAdapter) ctx ).ObjectContext;
			var osm = oc.ObjectStateManager;

			#= other.name #Set.Attach( insert ); osm.ChangeRelationshipState( copy, insert, o => o.#= collection #, EntityState.Added );

			var rows = ctx.SaveChanges();

			if ( rows != 1 ) throw new NBootstrap.IncorrectRowCountException( ""#= table.Name #.#= ensure #Core modified "" + rows + "" rows, not 1"" );
		}
	}

	//---------------------------------------------------------------------------------------------
	// #= overwrite #

	public virtual void #= overwrite #(#= lutInput #
		#= dog # template,
		IEnumerable<#= linkedIdsType #> #= linkedIds #
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			#= overwrite #Core( ctx,#= lutCall # template, #= linkedIds # );
		}
	}

	public virtual void #= overwrite #(
		IContext ctx,#= lutInput #
		#= dog # template,
		IEnumerable<#= linkedIdsType #> #= linkedIds #
	)
	{
		#= overwrite #Core( _Repo.DbContext( ctx ),#= lutCall # template, #= linkedIds # );
	}

	internal virtual void #= overwrite #Core(
		DbContext ctx,#= lutInput #
		#= dog # template,
		IEnumerable<#= linkedIdsType #> #= linkedIds # )
	{
		if ( #= linkedIds # == null ) throw new ArgumentNullException( ""#= linkedIds #"" );

		DetachExistingEntity( ctx, template );

		var #= table.name #Set = ctx.Set<global::#= table.NamespaceName #>();
		var #= other.name #Set = ctx.Set<global::#= other.NamespaceName #>();

		var olds =
			#= table.name #Set
			.Where( o =>#
{
	int i = 0;
	foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) {
		if ( i++ > 0 ) { #&& # } #
				o.#= c.Name # == template.#= c.Name ##
	} 
} #
			)
			.SelectMany( o => o.#= collection #.Select( x => x.#= oPrimaryKey.Name # ) )
			.ToList()
		;

		var deletes = olds.Distinct().Except( #= linkedIds # ).Select( o => new global::#= other.NamespaceName # { #= oPrimaryKey # = o } ).ToList();
		var inserts = #= linkedIds #.Distinct().Except( olds ).Select( o => new global::#= other.NamespaceName # { #= oPrimaryKey # = o } ).ToList();

		var copy = new #= dog #().CopyKeyColumnsFrom( template );

		#= table.name #Set.Attach( copy );

		var oc = ( (System.Data.Entity.Infrastructure.IObjectContextAdapter) ctx ).ObjectContext;
		var osm = oc.ObjectStateManager;

		foreach ( var other in deletes ) { #= other.name #Set.Attach( other ); osm.ChangeRelationshipState( copy, other, o => o.#= collection #, EntityState.Deleted ); }
		foreach ( var other in inserts ) { #= other.name #Set.Attach( other ); osm.ChangeRelationshipState( copy, other, o => o.#= collection #, EntityState.Added ); }

		var rows = ctx.SaveChanges();

		var changes = deletes.Count + inserts.Count;

		if ( rows != changes ) throw new NBootstrap.IncorrectRowCountException( ""#= table.Name #.#= overwrite #Core modified "" + rows + "" rows, not"" + changes );
	}

	//---------------------------------------------------------------------------------------------
	// #= removeAll #

	public virtual void #= removeAll #(#= lutInput #
		#= dog # template
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			#= removeAll #Core( ctx,#= lutCall # template );
		}
	}

	public virtual void #= removeAll #(
		IContext ctx,#= lutInput #
		#= dog # template
	)
	{
		#= removeAll #Core( _Repo.DbContext( ctx ),#= lutCall # template );
	}

	internal virtual void #= removeAll #Core(
		DbContext ctx,#= lutInput #
		#= dog # template
	)
	{
		var #= table.name #Set = ctx.Set<global::#= table.NamespaceName #>();

		var olds =
			#= table.name #Set
			.Where( o =>#
{
	int i = 0;
	foreach( var c in table.Columns.Where( o => o.IsPrimaryKey ) ) {
		if ( i++ > 0 ) { #&& # } #
				o.#= c.Name # == template.#= c.Name ##
	} 
} #
			)
			.SelectMany( o => o.#= collection #.Select( x => x.#= oPrimaryKey.Name # ) )
			.ToList()
		;

		#= remove #Core( ctx,#= lutCall # template, olds );
	}

	//---------------------------------------------------------------------------------------------
	// #= remove #

	public virtual void #= remove #(#= lutInput #
		#= dog # template,
		IEnumerable<#= linkedIdsType #> #= linkedIds #
	)
	{
		using ( var ctx = _Repo.CreateDbContext( false ) )
		{
			#= remove #Core( ctx,#= lutCall # template, #= linkedIds # );
		}
	}

	public virtual void #= remove #(
		IContext ctx,#= lutInput #
		#= dog # template,
		IEnumerable<#= linkedIdsType #> #= linkedIds #
	)
	{
		#= remove #Core( _Repo.DbContext( ctx ),#= lutCall # template, #= linkedIds # );
	}

	internal virtual void #= remove #Core(
		DbContext ctx,#= lutInput #
		#= dog # template,
		IEnumerable<#= linkedIdsType #> #= linkedIds #
	)
	{
		DetachExistingEntity( ctx, template );

		var #= table.name #Set = ctx.Set<global::#= table.NamespaceName #>();
		var #= other.name #Set = ctx.Set<global::#= other.NamespaceName #>();

		var deletes = #= linkedIds #.Distinct().Select( o => new global::#= other.NamespaceName # { #= oPrimaryKey # = o } ).ToList();

		var copy = new #= dog #().CopyKeyColumnsFrom( template );

		#= table.name #Set.Attach( copy );

		var oc = ( (System.Data.Entity.Infrastructure.IObjectContextAdapter) ctx ).ObjectContext;
		var osm = oc.ObjectStateManager;

		foreach ( var other in deletes ) { #= other.name #Set.Attach( other ); osm.ChangeRelationshipState( copy, other, o => o.#= collection #, EntityState.Deleted ); }

		var rows = ctx.SaveChanges();

		var changes = deletes.Count;

		if ( rows != changes ) throw new NBootstrap.IncorrectRowCountException( ""#= table.Name #.#= remove #Core modified "" + rows + "" rows, not"" + changes );
	}
#
}
#
	//---------------------------------------------------------------------------------------------
", new
	{
		config,
		model,
		table,
		repoClassname = config.CoderSettings[ Settings.OUTPUT_CLASSNAME_REP ],
		tableClassname = table.Name + "Table",
		lutInput = ( !loginUserTokenSet ? null : "\r\n\t\tglobal::" + loginUserTokenType + " token," ),
		lutCall = ( !loginUserTokenSet ? null : " token," ),
		dog = "global::" + table.NamespaceName,

		oPrimaryKey,
		other,
		collection,
	},
	tabs: 2
);
				}
			}

			return true;
		}

		//-----------------------------------------------------------------------------------------

	}
}
