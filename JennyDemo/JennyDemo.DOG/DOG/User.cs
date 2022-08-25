using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace JennyDemo.DOG
{
	public partial class User
	{
		public int UserId { get; set; }

		[Required, StringLength( 250 )] public string Email { get; set; }
		[Required, StringLength( 250 )] public string TimeZone { get; set; }

		[StringLength( 100 )] public string FirstName { get; set; }
		[StringLength( 100 )] public string LastName { get; set; }

		public string FullName => String.Join( " ", FirstName, LastName );
	}
}
