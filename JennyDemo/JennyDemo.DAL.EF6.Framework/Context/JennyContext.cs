using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using NBootstrap.EF;

namespace JennyDemo.DAL.Context
{
	public partial class JennyContext : DbContext
	{
		static JennyContext()
		{
			Database.SetInitializer<JennyContext>( null );

			Connection.Init( new() { DefaultConnectionString = DOG.Glob.ConnectionString } );
		}

		public JennyContext() : this( null ) { }

		public JennyContext( string nameOrConnectionString ) : this( nameOrConnectionString, false ) { }

		public JennyContext( string nameOrConnectionString, bool lazyLoadingEnabled )
			: base( nameOrConnectionString ?? DOG.Glob.ConnectionString )
		{
			Init( lazyLoadingEnabled );
		}

		public JennyContext( NConnection connection, bool contextOwnsConnection, bool lazyLoadingEnabled = false )
			: base( connection.DbConnection, contextOwnsConnection )
		{
			Init( lazyLoadingEnabled );
		}

		void Init( bool lazyLoadingEnabled )
		{
			Configuration.LazyLoadingEnabled = lazyLoadingEnabled;
			Configuration.ProxyCreationEnabled = lazyLoadingEnabled;

			Database.Log = s =>
			{
				if ( s.Contains( "connection" ) ) return;

				s = s.TrimEnd( '\r', '\n' );

				if ( s.Length > 0 && s[ 0 ] != '-' ) s = "\r\n" + s;

				if ( !String.IsNullOrWhiteSpace( s ) ) Log.Info( s, category: "sql" );
			};
		}

		protected override void OnModelCreating( DbModelBuilder mb )
		{
			base.OnModelCreating( mb );

			mb.Conventions.Remove<PluralizingTableNameConvention>();
			mb.Conventions.Remove<OneToManyCascadeDeleteConvention>();
			mb.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

			OnModelCreatingEx( mb );
		}

		partial void OnModelCreatingEx( DbModelBuilder mb );

		public void Reset()
		{
			Database.ExecuteSqlCommand( "DELETE FROM A1Answer" );
			Database.ExecuteSqlCommand( "DELETE FROM A1Question" );
			Database.ExecuteSqlCommand( "DELETE FROM A1Interview" );
			Database.ExecuteSqlCommand( "DELETE FROM A1Candidate" );

			var r = new Random();

			foreach ( var candidate in DOG.A1Candidate.Defaults )
			{
				var c = Candidate.Add( candidate );

				var i = Interview.Add( new() { Candidate = c, DateTime = DateTime.UtcNow } );

				foreach ( var q in DOG.A1Question.Defaults )
				{
					Question.Add( q );

					Answer.Add( new()
					{
						Interview = i,
						Question = q,
						Stars = r.Next( 1, 6 )
					} ); ;
				}
			}

			SaveChanges();
		}

		public DbSet<DOG.A1Answer> Answer { get; set; }
		public DbSet<DOG.A1Candidate> Candidate { get; set; }
		public DbSet<DOG.A1Interview> Interview { get; set; }
		public DbSet<DOG.A1Question> Question { get; set; }
	}
}
