using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JennyDemo.DOG
{
	[Table( "A1Question" )]
	public partial class A1Question
	{
		public int Id { get; set; }

		[Required] [StringLength( 1024 )] public string Question { get; set; }

		public ICollection<A1Answer> Answers { get; set; } = new List<A1Answer>();

		public static IReadOnlyCollection<A1Question> Defaults { get; } = new List<A1Question>
		{
			new A1Question { Question = "favourite language?" },
			new A1Question { Question = "spaces or tabs?" },
			new A1Question { Question = "gates or jobs?" },
		};
	}
}
