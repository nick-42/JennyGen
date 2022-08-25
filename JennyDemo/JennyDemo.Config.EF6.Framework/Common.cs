using System;
using System.Linq;

using Jenny.MEF.Config;

namespace JennyDemo.Config
{
	public static class Common
	{
		public static void Schema_IgnoreTable( object _, IgnoreTableEventArgs e )
		{
			if ( e.Table.Name == "AspNetUser" ) e.Ignore = true;
			if ( e.Table.Name == "AspNetUserClaim" ) e.Ignore = true;
			if ( e.Table.Name == "AspNetUserLogin" ) e.Ignore = true;
			if ( e.Table.Name == "AspNetRole" ) e.Ignore = true;
			if ( e.Table.Name == "AspNetUserRoles" ) e.Ignore = true;
		}

		public static void Schema_IgnoreColumn( object _, IgnoreColumnEventArgs e )
		{
			if ( e.Column.Name == "Fred" ) e.Ignore = true;
		}

		public static void Schema_InspectTable( object _, TableEventArgs e )
		{
			switch ( e.Table.Name )
			{
				//case "AuditRequest":
				//	e.Table.SuppressCrudLogging = true;
				//	break;

				//case "Culture":
				//	e.Table.DropDownListSort = "new {{ {0}.EnglishName }}";
				//	e.Table.DropDownListText = "{0}.EnglishName";
				//	break;

				//case "EventType":
				//	e.Table.DropDownListSort = "new {{ {0}.Code, {0}.Name }}";
				//	e.Table.DropDownListText = "{0}.Code + \" - \" + {0}.Name";
				//	break;

				//case "User":
				//	e.Table.DropDownListSort = "new {{ {0}.FirstName, {0}.LastName }}";
				//	e.Table.DropDownListText = "{0}.FullName";
				//	break;

				default:
					if ( e.Table.Columns.Any( o => o.Name == "Name" ) )
					{
						e.Table.DropDownListSort = e.Table.Columns.Any( o => o.Name == "DisplayOrder" )
							? "{0}.DisplayOrder"
							: "{0}.Name"
						;

						e.Table.DropDownListText = "{0}.Name";
					}
					break;
			}
		}

		public static void Schema_InspectColumn( object _, ColumnEventArgs e )
		{
			if ( e.Column.Name == "Name" ) e.Column.IncludeInToString = true;

			if ( e.Column.Name == "RowVersion" && !e.Column.IsTimestamp ) throw new ApplicationException( e.Table.Name + "." + e.Column.Name + " is not marked as a Timestamp" );
		}
	}
}
