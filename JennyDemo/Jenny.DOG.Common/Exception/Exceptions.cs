using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NBootstrap
{

	//---------------------------------------------------------------------------------------------
	// Debug

	public class DebugPassthroughException : ExpectedException
	{
		public DebugPassthroughException( Exception x ) : base( NBootstrapExceptionCode.DebugPassthrough, x + "\n\n" + new System.Diagnostics.StackTrace( true ), x ) { }
	}

	//---------------------------------------------------------------------------------------------
	// General

	public class BadArgumentException : ExpectedException
	{
		public BadArgumentException( string name, string message ) : base( NBootstrapExceptionCode.BadArgument, "BadArgument: '" + name + "' - " + message ) { }
	}

	public class UnknownLoginUserIdException : ExpectedException
	{
		public UnknownLoginUserIdException( int loginUserId, int userId ) : base( NBootstrapExceptionCode.UnknownLoginUserId, "Unknown Login UserId: " + loginUserId + " [ " + userId + " ]" ) { }
	}

	public class DuplicateException : ExpectedException
	{
		public DuplicateException() : base( NBootstrapExceptionCode.Duplicate, "Duplicate" ) { }
	}

	//---------------------------------------------------------------------------------------------
	// Database

	public class UpdateErrorException : ExpectedException
	{
		public UpdateErrorException( string message ) : base( NBootstrapExceptionCode.UpdateError, message ) { }
	}

	public class IncorrectRowCountException : ExpectedException
	{
		public IncorrectRowCountException( string message ) : base( NBootstrapExceptionCode.IncorrectRowCount, message ) { }
		public IncorrectRowCountException( InvalidOperationException x ) : base( NBootstrapExceptionCode.IncorrectRowCount, x ) { }
	}

	//---------------------------------------------------------------------------------------------
	// exceptions derived from NBootstrapDatabaseException may have their x.Message string shown in the UI

	public class NBootstrapDatabaseException : ExpectedException
	{
		protected NBootstrapDatabaseException() : base() { }
		protected NBootstrapDatabaseException( NBootstrapExceptionCode code, string message ) : base( code, message ) { }
		protected NBootstrapDatabaseException( NBootstrapExceptionCode code, Exception inner ) : base( code, inner ) { }
		protected NBootstrapDatabaseException( NBootstrapExceptionCode code, string message, Exception inner ) : base( code, message, inner ) { }
	}

	public class OptimisticConcurrencyErrorException : NBootstrapDatabaseException
	{
		public OptimisticConcurrencyErrorException() : base( NBootstrapExceptionCode.OptimisticConcurrencyError, "Optimistic Concurrency Error" ) { }
	}

	public class DataTruncationException : NBootstrapDatabaseException
	{
		public DataTruncationException() : base( NBootstrapExceptionCode.DataTruncation, "Data too long" ) { }
	}

	public class ForeignKeyException : NBootstrapDatabaseException
	{
		static Regex _Table = new Regex( @"table\s*""(?<table>[^""]*)""", RegexOptions.Compiled | RegexOptions.IgnoreCase );

		string _Message = null;

		public ForeignKeyException( string message )
			: base( NBootstrapExceptionCode.ForeignKey, "Foreign Key Conflict" )
		{
			var match = _Table.Match( message );
			if ( match == null ) return;

			var group = match.Groups[ "table" ];
			if ( group == null ) return;

			var table = group.Value;
			if ( String.IsNullOrWhiteSpace( table ) ) return;

			var i = table.LastIndexOf( '.' );
			if ( i >= 0 ) table = table.Substring( i + 1 );

			_Message = "This update conflicts with record(s) of type \"" + table + "\"";
		}

		public override string Message
		{
			get
			{
				if ( !String.IsNullOrWhiteSpace( _Message ) )
				{
					return _Message;
				}

				return base.Message;
			}
		}
	}

	//---------------------------------------------------------------------------------------------
	// WCF

	public class CommunicationErrorException : ExpectedException
	{
		public CommunicationErrorException( Exception inner ) : base( NBootstrapExceptionCode.CommunicationError, "Communication Error", inner ) { }
		protected CommunicationErrorException( NBootstrapExceptionCode code, string message, Exception inner ) : base( code, message, inner ) { }
	}

	public class CommunicationTimeoutException : CommunicationErrorException
	{
		public CommunicationTimeoutException( TimeoutException inner ) : base( NBootstrapExceptionCode.CommunicationTimeout, "Communication Timeout", inner ) { }
	}

	//---------------------------------------------------------------------------------------------
	// Authentication

	public class AuthenticationFailedException : ExpectedException
	{
		public AuthenticationFailedException() : base( NBootstrapExceptionCode.AuthenticationFailed, "Authentication Failed" ) { }
	}

	public class AccountLockedOutException : ExpectedException
	{
		public AccountLockedOutException() : base( NBootstrapExceptionCode.AccountLockedOut, "Account is Locked Out" ) { }
	}

	public class DuplicateUsernameException : ExpectedException
	{
		public DuplicateUsernameException() : base( NBootstrapExceptionCode.DuplicateUsername, "Duplicate Username" ) { }
	}

	public class UnknownUserException : ExpectedException
	{
		public UnknownUserException() : base( NBootstrapExceptionCode.UnknownUser, "Unknown User" ) { }
	}

	//---------------------------------------------------------------------------------------------
	// Authorization

	public class UnauthorizedException : ExpectedException
	{
		public UnauthorizedException() : base( NBootstrapExceptionCode.Unauthorized, "Authorization Failed" ) { }
	}

	//---------------------------------------------------------------------------------------------

}
