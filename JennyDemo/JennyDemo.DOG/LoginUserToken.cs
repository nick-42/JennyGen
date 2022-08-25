using System;
using System.Collections.Generic;
using System.Text;

namespace JennyDemo.DOG
{
	public partial class LoginUserToken
	{
		public User SuperUser { get; private set; }
		public User LoginUser { get; private set; }

		public LoginUserToken( User user )
		{
			SuperUser = LoginUser = user;

			Init();
		}

		public LoginUserToken( User superUser, User loginUser )
		{
			SuperUser = superUser;
			LoginUser = loginUser;

			Init();
		}

		public void Impersonate( User loginUser )
		{
			LoginUser = loginUser;

			Init();
		}

		public void RemoveImpersonation()
		{
			LoginUser = SuperUser;

			Init();
		}

		void Init()
		{
			if ( !String.IsNullOrWhiteSpace( LoginUser.TimeZone ) )
			{
				try
				{
					TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById( LoginUser.TimeZone );
				}
				catch ( Exception x )
				{
					Log.Error( "Failed to lookup TimeZone '" + LoginUser.TimeZone + "' for user '" + LoginUser + "'", x: x );
				}
			}
		}

		public bool IsLoggedIn => LoginUser?.UserId > 0;
		public bool IsSuperUserDeveloper { get; set; }
		public bool IsLoginUserDeveloper { get; set; }

		public bool HasImpersonate { get; set; }

		public bool ShowDeveloperMetrics { get; set; }

		public TimeZoneInfo TimeZoneInfo { get; set; }

		public bool IsImpersonating
		{
			get
			{
				if ( SuperUser == null || LoginUser == null ) return false;

				return ( SuperUser.UserId != LoginUser.UserId );
			}
		}

		public override string ToString()
		{
			return "LUT[ " + SuperUser?.FullName +
				( IsImpersonating ? " IMPERSONATING " + LoginUser?.FullName : "" ) +
				" ]"
			;
		}
	}
}
