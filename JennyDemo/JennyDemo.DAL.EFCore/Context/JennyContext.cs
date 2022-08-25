using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using NBootstrap.EF;

namespace JennyDemo.DAL.Context
{
	public partial class JennyContext : DbContext
	{
		static JennyContext()
		{
			Connection.Init( new() { DefaultConnectionString = DOG.Glob.ConnectionString } );
		}

		static void RaiseLog( string s )
		{
			if ( s.Contains( "connection" ) ) return;

			s = s.TrimEnd( '\r', '\n' );

			if ( s.Length > 0 && s[ 0 ] != '-' ) s = "\r\n" + s;

			if ( !String.IsNullOrWhiteSpace( s ) ) Log.Info( s, category: "sql" );
		}

		public JennyContext() : this( null ) { }

		public JennyContext( string nameOrConnectionString ) : this( nameOrConnectionString, false ) { }

		bool LazyLoadingEnabled { get; }
		bool ContextOwnsConnection { get; } = true;
		string NameOrConnectionString { get; }

		public JennyContext( string nameOrConnectionString, bool lazyLoadingEnabled ) : base()
		{
			NameOrConnectionString = nameOrConnectionString;
			LazyLoadingEnabled = lazyLoadingEnabled;
		}

		public JennyContext( NConnection connection, bool contextOwnsConnection, bool lazyLoadingEnabled = false )
			: base( Configure( connection ) )
		{
			NameOrConnectionString = null;
			ContextOwnsConnection = contextOwnsConnection;
			LazyLoadingEnabled = lazyLoadingEnabled;
		}

		static DbContextOptions<JennyContext> Configure( NConnection connection )
		{
			return new DbContextOptionsBuilder<JennyContext>()
				.LogTo( RaiseLog )
				.EnableDetailedErrors()
				.EnableSensitiveDataLogging()
				//.ConfigureWarnings( o => o.Ignore( Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.NavigationBaseIncludeIgnored ) )
				.UseSqlServer( connection.DbConnection )
				.Options;
		}

		//static readonly ILoggerFactory consoleLoggerFactory = new LoggerFactory().AddConsole();
		protected override void OnConfiguring( DbContextOptionsBuilder optionsBuilder )
		{
			if ( !ContextOwnsConnection ) return;

			optionsBuilder
				.LogTo( RaiseLog )
				.EnableDetailedErrors()
				.EnableSensitiveDataLogging()
				//.UseLazyLoadingProxies()
				//.ConfigureWarnings( o => o.Ignore( Microsoft.EntityFrameworkCore.Diagnostics.CoreEventId.NavigationBaseIncludeIgnored ) )
				.UseSqlServer( NameOrConnectionString ?? DOG.Glob.ConnectionString );
		}

		//protected override void OnModelCreating( DbModelBuilder mb )
		//{
		//	base.OnModelCreating( mb );

		//	mb.Conventions.Remove<PluralizingTableNameConvention>();
		//	mb.Conventions.Remove<OneToManyCascadeDeleteConvention>();
		//	mb.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
		//}

		public void Reset()
		{
			Database.ExecuteSqlRaw( "DELETE FROM A1Answer" );
			Database.ExecuteSqlRaw( "DELETE FROM A1Question" );
			Database.ExecuteSqlRaw( "DELETE FROM A1Interview" );
			Database.ExecuteSqlRaw( "DELETE FROM A1Candidate" );

			var r = new Random();

			foreach ( var candidate in DOG.A1Candidate.Defaults )
			{
				var c = Candidate.Add( candidate );

				var i = Interview.Add( new() { Candidate = c.Entity, DateTime = DateTime.UtcNow } );

				foreach ( var q in DOG.A1Question.Defaults )
				{
					Question.Add( q );

					Answer.Add( new()
					{
						Interview = i.Entity,
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
