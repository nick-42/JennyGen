namespace JennyDemo.Test
{
	[TestClass]
	public class DogTests
	{
		[TestMethod]
		public void Test_RowIdentityObject_PrimaryKeyAsString()
		{
			var dog = DogFactory.CreateInterview();

			var s = dog.RowIdentityObject.PrimaryKeyAsString;

			Assert.AreEqual( s, dog.Id.ToString() );
		}

		[TestMethod]
		public void Test_SetPrimaryKeyFromString()
		{
			var dog = DogFactory.CreateInterview();

			dog.SetPrimaryKeyFromString( "42" );

			Assert.AreEqual( 42, dog.Id );
		}

		[TestMethod]
		public void Test_PrimaryKeyComparer()
		{
			var dictionary = new Dictionary<A1Interview, object>( A1Interview.PrimaryKeyComparer );

			var dog1 = DogFactory.CreateInterview();
			var dog2 = DogFactory.CreateInterview();

			dictionary.Add( dog1, null );

			Assert.IsTrue( dictionary.ContainsKey( dog2 ) );
		}

		[TestMethod]
		public void Test_CopyConstructor()
		{
			var dog1 = DogFactory.CreateInterview();

			var dog2 = new A1Interview( dog1 );

			Assert.IsTrue( dog2.Id == dog1.Id );
			Assert.IsTrue( dog2.DateTime == dog1.DateTime );
			Assert.IsTrue( dog2.CandidateId == dog1.CandidateId );
			Assert.IsTrue( dog2.Candidate == default );
		}

		[TestMethod]
		public void Test_StaticColumnPropertyNames()
		{
			var names = A1Interview.StaticColumnPropertyNames;

			Assert.IsTrue( names.SequenceEqual( new[] { "Id", "DateTime", "CandidateId" } ) );
		}

		[TestMethod]
		public void Test_StaticColumnPropertyInfos()
		{
			var properties = A1Interview.StaticColumnPropertyInfos;

			Assert.IsTrue( properties.Select( o => o.Name ).SequenceEqual( new[] { "Id", "DateTime", "CandidateId" } ) );
		}

		[TestMethod]
		public void Test_CreateColumnCopy()
		{
			var dog1 = DogFactory.CreateInterview();

			var dog2 = dog1.CreateColumnCopy();

			Assert.IsTrue( dog2.Id == dog1.Id );
			Assert.IsTrue( dog2.DateTime == dog1.DateTime );
			Assert.IsTrue( dog2.CandidateId == dog1.CandidateId );
			Assert.IsTrue( dog2.Candidate == default );
		}

		[TestMethod]
		public void Test_CopyAllColumnsFrom()
		{
			var dog1 = DogFactory.CreateInterview();

			var dog2 = new A1Interview().CopyAllColumnsFrom( dog1 );

			Assert.IsTrue( dog2.Id == dog1.Id );
			Assert.IsTrue( dog2.DateTime == dog1.DateTime );
			Assert.IsTrue( dog2.CandidateId == dog1.CandidateId );
			Assert.IsTrue( dog2.Candidate == default );
		}

		[TestMethod]
		public void Test_CopyKeyColumnsFrom()
		{
			var dog1 = DogFactory.CreateInterview();

			var dog2 = new A1Interview().CopyKeyColumnsFrom( dog1 );

			Assert.IsTrue( dog2.Id == dog1.Id );
			Assert.IsTrue( dog2.DateTime == default );
			Assert.IsTrue( dog2.CandidateId == default );
			Assert.IsTrue( dog2.Candidate == default );
		}

		[TestMethod]
		public void Test_CopyDataColumnsFrom()
		{
			var dog1 = DogFactory.CreateInterview();

			var dog2 = new A1Interview().CopyDataColumnsFrom( dog1 );

			Assert.IsTrue( dog2.Id == default );
			Assert.IsTrue( dog2.DateTime == dog1.DateTime );
			Assert.IsTrue( dog2.CandidateId == dog1.CandidateId );
			Assert.IsTrue( dog2.Candidate == default );
		}

		[TestMethod]
		public void Test_AreColumnsEqual()
		{
			var dog1 = DogFactory.CreateInterview();
			var dog2 = DogFactory.CreateInterview();

			Assert.IsTrue( dog1.AreColumnsEqual( dog2 ) );
		}

		[TestMethod]
		public void Test_All_GmtToLocal()
		{
			var dog = new A1Interview { DateTime = new( 2000, 6, 1, 12, 0, 0 ) };

			dog.All_GmtToLocal( TimeZoneInfo.FindSystemTimeZoneById( "GMT Standard Time" ) );

			Assert.AreEqual( new( 2000, 6, 1, 13, 0, 0 ), dog.DateTime_Local );
		}

		[TestMethod]
		public void Test_All_LocalToGmt()
		{
			var dog = new A1Interview { DateTime_Local = new( 2000, 6, 1, 13, 0, 0 ) };

			dog.All_LocalToGmt( TimeZoneInfo.FindSystemTimeZoneById( "GMT Standard Time" ) );

			Assert.AreEqual( new( 2000, 6, 1, 12, 0, 0 ), dog.DateTime );
		}
	}
}
