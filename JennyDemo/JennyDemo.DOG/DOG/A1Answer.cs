using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace JennyDemo.DOG
{
	[Table( "A1Answer" )]
	public partial class A1Answer
	{
		public int Id { get; set; }

		public int InterviewId { get; set; }
		public A1Interview Interview { get; set; }

		public int QuestionId { get; set; }
		public A1Question Question { get; set; }

		public int Stars { get; set; }
	}
}
