using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
	public enum NBootstrapExceptionCode
	{
		Unexpected = -1,
		Zero = 0,

		// Application
		Application = 1,
		Unauthorized = 2,

		// Debug
		DebugPassthrough = 42,

		// General
		GeneralError = 100,
		BadArgument,
		UnknownLoginUserId,
		Duplicate,

		// Database
		DatabaseError = 200,
		IncorrectRowCount,
		UpdateError,
		OptimisticConcurrencyError,
		DataTruncation,
		ForeignKey,

		// WCF
		CommunicationError = 300,
		CommunicationTimeout,

		// User
		UserError = 1000,
		AuthenticationFailed,
		AccountLockedOut,
		DuplicateUsername,
		UnknownUser,
	}
}
