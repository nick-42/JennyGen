#if NET48
using JennyDemo.DAL.EF6.Framework;
#elif EF6
using JennyDemo.DAL.EF6.Core;
#else
using JennyDemo.DAL.EFCore;
#endif

namespace JennyDemo.Test
{
	[TestClass]
	public class RepoTests
	{
		[AssemblyInitialize]
		public static void AssemblyInit( TestContext context )
		{
			// run cctor
			new JennyDemo.DAL.Context.JennyContext();
		}

		JennyRepo _repo = null;

		[TestInitialize]
		public void Initialize()
		{
			_repo = new( TimeZoneInfo.FindSystemTimeZoneById( "GMT Standard Time" ) );
		}

		[TestCleanup]
		public void Cleanup()
		{
			_repo.Dispose();
		}

		[TestMethod]
		public void Test_Read_Simple()
		{
			var found = _repo.A1Interview.Read().Any();

			Assert.IsTrue( found );
		}

		[TestMethod]
		public void Test_Read_Include()
		{
			var dog = _repo.A1Interview.Read()
				.Include( o => o.Candidate )
				.First();

			Assert.IsNotNull( dog.Candidate );
		}

		[TestMethod]
		public void Test_Read_Context()
		{
			using var ctx = _repo.CreateContext();

			var interview = _repo.A1Interview.Read( ctx ).First();

			_repo.A1Candidate.Read( ctx ).Single( o => o.Id == interview.CandidateId );

			Assert.IsNotNull( interview.Candidate );
		}

		[TestMethod]
		public void Test_Read_TimeZones()
		{
			var dog = _repo.A1Interview.Read().First();

			Assert.IsTrue( dog.DateTime_Local != default );
		}

		[TestMethod]
		public void Test_Create()
		{
			var template = DogFactory.CreateCandidate();
			template.Id = 0;

			var dog = _repo.A1Candidate.Create( null, template );

			Assert.AreNotEqual( dog.Id, template.Id );
			Assert.IsTrue( dog.AreColumnsEqual( template ) );
		}

		[TestMethod]
		public void Test_Update_All()
		{
			var template = DogFactory.CreateCandidate();
			template.Id = 0;

			var dog1 = _repo.A1Candidate.Create( null, template );

			dog1.GivenName = "Alan";
			dog1.FamilyName = "Turing";

			var dog2 = _repo.A1Candidate.UpdateAllColumns( null, dog1 );

			Assert.AreEqual( dog2.GivenName, "Alan" );
			Assert.AreEqual( dog2.FamilyName, "Turing" );
		}

		[TestMethod]
		public void Test_Update_Map()
		{
			var template = DogFactory.CreateCandidate();
			template.Id = 0;

			var dog1 = _repo.A1Candidate.Create( null, template );

			var dog2 = _repo.A1Candidate.UpdateMapColumns( null, dog1, x => x
				.Column( o => o.GivenName, "Alan" )
			);

			Assert.AreEqual( dog2.GivenName, "Alan" );
			Assert.AreEqual( dog2.FamilyName, dog1.FamilyName );
		}

		[TestMethod]
		public void Test_Delete_Identity()
		{
			var template = DogFactory.CreateCandidate();
			template.Id = 0;

			var dog1 = _repo.A1Candidate.Create( null, template );

			_repo.A1Candidate.Delete( null, dog1.RowIdentityObject );

			var dog2 = _repo.A1Candidate.Read().SingleOrDefault( o => o.Id == dog1.Id );

			Assert.IsNull( dog2 );
		}

		[TestMethod]
		public void Test_Delete_Template()
		{
			var template = DogFactory.CreateCandidate();
			template.Id = 0;

			var dog1 = _repo.A1Candidate.Create( null, template );

			_repo.A1Candidate.Delete( null, dog1 );

			var dog2 = _repo.A1Candidate.Read().SingleOrDefault( o => o.Id == dog1.Id );

			Assert.IsNull( dog2 );
		}

		[TestMethod]
		public void Test_Write_Hooks()
		{
			var called = false;

			void handler(
				object sender,
				JennyRepo repo,
				LoginUserToken token,
				DOG.A1Candidate dog,
				UpdateMap<DOG.A1Candidate> map,
				UpdateMapEntryColumnBuilder<DOG.A1Candidate> map2
			)
			{
				called = true;
			}

			JennyRepo.A1CandidateTable.OnBeforeCreate += handler;

			var template = DogFactory.CreateCandidate();
			template.Id = 0;

			_repo.A1Candidate.Create( null, template );

			JennyRepo.A1CandidateTable.OnBeforeCreate -= handler;

			Assert.IsTrue( called );
		}
	}
}
