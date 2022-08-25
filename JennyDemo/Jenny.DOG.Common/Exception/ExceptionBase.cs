using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
	public abstract class NBootstrapException : ApplicationException
	{
		public NBootstrapExceptionCode Code { get; set; }

		protected NBootstrapException( NBootstrapExceptionCode code ) : base() { Code = code; }

		protected NBootstrapException( NBootstrapExceptionCode code, string message ) : base( message ) { Code = code; }

		protected NBootstrapException( NBootstrapExceptionCode code, string message, Exception inner ) : base( message, inner ) { Code = code; }
	}

	public abstract class ExpectedException : NBootstrapException
	{
		protected ExpectedException() : base( NBootstrapExceptionCode.Application ) { }

		protected ExpectedException( NBootstrapExceptionCode code, string message ) : base( code, message ) { }

		protected ExpectedException( NBootstrapExceptionCode code, Exception inner ) : base( code, inner.Message, inner ) { }

		protected ExpectedException( NBootstrapExceptionCode code, string message, Exception inner ) : base( code, message, inner ) { }
	}

	public class UnexpectedException : NBootstrapException
	{
		public UnexpectedException() : base( NBootstrapExceptionCode.Unexpected, "Unexpected error." ) { }

		public UnexpectedException( string message ) : base( NBootstrapExceptionCode.Unexpected, message ) { }

		public UnexpectedException( Exception inner ) : base( NBootstrapExceptionCode.Unexpected, inner.Message, inner ) { }

		public UnexpectedException( string message, Exception inner ) : base( NBootstrapExceptionCode.Unexpected, message, inner ) { }
	}
}
