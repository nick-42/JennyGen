using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JennyDemo.DOG
{
	[Table( "A1Candidate" )]
	public partial class A1Candidate
	{
		public int Id { get; set; }

		[Required] [StringLength( 128 )] public string GivenName { get; set; }
		[Required] [StringLength( 128 )] public string FamilyName { get; set; }

		public ICollection<A1Interview> Interviews { get; set; } = new List<A1Interview>();

		public static IReadOnlyCollection<A1Candidate> Defaults => new List<A1Candidate>
		{
			new A1Candidate { GivenName = "Fred", FamilyName = "Brooks" },
			new A1Candidate { GivenName = "Anders", FamilyName = "Hejlsberg" },
			new A1Candidate { GivenName = "Evan", FamilyName = "You" },
		};
	}
}
