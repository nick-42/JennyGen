using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Jenny;
using JennyDemo.DAL.EF6.Framework;

namespace JennyDemo.EF6.Framework
{
	class Program
	{
		static async Task Main( string[] args )
		{
			DAL.JennyDataAccessLayer.Reset();

			var logger = new NBootstrap.Global.Logging.LogToConsole();
			Log.Logger = s => logger;
			NBootstrap.EF.Glob.SqlExecute += ( s, e ) => Log.Info( "REPO " + e.Expression, raw: true );

			Console.WriteLine( "---" );
			Console.WriteLine( "DAL.JennyDataAccessLayer.FetchInterviews()" );
			var interviews = DAL.JennyDataAccessLayer.FetchInterviews();
			Console.WriteLine( "interviews" );
			foreach ( var o in interviews ) Console.WriteLine( o.ToLogString() );
			Console.WriteLine( "interviews.Candidate" );
			foreach ( var o in interviews ) Console.WriteLine( o.Candidate?.ToLogString() );
			Console.WriteLine( "interviews.Answers.Question" );
			foreach ( var o in interviews.SelectMany( o => o.Answers ) ) Console.WriteLine( o.Question?.ToLogString() );

			Console.WriteLine( "---" );
			Console.WriteLine( "new DAL.JennyRepo()" );
			using var repo = new JennyRepo();

			Console.WriteLine( "---" );
			Console.WriteLine( "repo.A1Answer.Read().AnyAsync()" );
			var average = await repo.A1Answer.Read().AnyAsync( o => o.Stars == 5, CancellationToken.None );
			Console.WriteLine( average );

			Console.WriteLine( "---" );
			Console.WriteLine( "repo.A1Interview.Read().Include( ... )" );
			interviews = repo.A1Interview.Read()
				.Include( o => o.Answers.Select( o => o.Question ) )
				.Include( o => o.Candidate )
				.ToList();
			Console.WriteLine( "interviews" );
			foreach ( var o in interviews ) Console.WriteLine( o.ToLogString() );
			Console.WriteLine( "interviews.Candidate" );
			foreach ( var o in interviews ) Console.WriteLine( o.Candidate?.ToLogString() );
			Console.WriteLine( "interviews.Answers.Question" );
			foreach ( var o in interviews.SelectMany( o => o.Answers ) ) Console.WriteLine( o.Question?.ToLogString() );

			Console.WriteLine( "---" );
			Console.WriteLine( "repo.A1Candidate.Read().Single( o => o.GivenName == \"fred\" )" );
			var id = repo.A1Candidate.Read().Single( o => o.GivenName == "fred" ).Id;

			Console.WriteLine( "---" );
			Console.WriteLine( "repo.A1Candidate.UpdateMapColumns" );
			var fred = repo.A1Candidate.UpdateMapColumns( null, new() { Id = id }, x => x
				.Column( o => o.GivenName, "Frederick" )
			);
			Console.WriteLine( fred.ToLogString() );

			Console.WriteLine( "---" );
			Console.WriteLine( "DAL.JennyDataAccessLayer.FetchCandidates()" );
			var candidates = DAL.JennyDataAccessLayer.FetchCandidates();
			foreach ( var o in candidates ) Console.WriteLine( o.ToLogString() );
		}
	}
}
