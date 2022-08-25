using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JennyDemo.DOG
{
	[Table( "A1Interview" )]
	public partial class A1Interview
	{
		public int Id { get; set; }

		public DateTime DateTime { get; set; }

		public int CandidateId { get; set; }
		public A1Candidate Candidate { get; set; }

		public ICollection<A1Answer> Answers { get; set; } = new List<A1Answer>();
	}
}
