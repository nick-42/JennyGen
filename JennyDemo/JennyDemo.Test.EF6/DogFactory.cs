using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JennyDemo.Test
{
	class DogFactory
	{
		public static A1Candidate CreateCandidate()
		{
			return new A1Candidate
			{
				Id = 1,
				FamilyName = "Steve",
				GivenName = "McConnell",
			};
		}

		public static A1Interview CreateInterview()
		{
			var candidate = CreateCandidate();

			var interview = new A1Interview
			{
				Id = 2,
				DateTime = new( 1999, 12, 31, 23, 59, 59 ),
				CandidateId = candidate.Id,
				Candidate = candidate,
			};

			candidate.Interviews.Add( interview );

			return interview;
		}
	}
}
