using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Common;

using NBootstrap.Global;

namespace NBootstrap.EF
{

	//-----------------------------------------------------------------------------------------
	// NTransaction

	public class NTransaction : ITransaction
	{
		static long s_transactionIndex = -1;
		static long s_liveTransactions = 0;
		public static long LiveTransactions { get { return s_liveTransactions; } }

		readonly long _connectionIndex = 0;
		readonly long _transactionIndex = 0;
		readonly long _transactionLevel = 0;

		public bool IsCommitted { get; private set; }
		public bool IsRolledback { get; private set; }

		public bool IsCompleted { get { return ( IsCommitted || IsRolledback ); } }
		public bool IsDisposed { get { return _disposed; } }

		public event EventHandler Committed = delegate { };
		public event EventHandler Rolledback = delegate { };
		public event EventHandler Disposed = delegate { };

		void Log( string message, bool error )
		{
			NConnection.Log( _connectionIndex, $"TRANSACTION: {_transactionIndex} level: {_transactionLevel} {message}", error );
		}

		public NTransaction( long connectionIndex, long transactionLevel )
		{
			_connectionIndex = connectionIndex;
			_transactionLevel = transactionLevel;
			_transactionIndex = Interlocked.Increment( ref s_transactionIndex );
			Interlocked.Increment( ref s_liveTransactions );

			Log( "CREATED", false );
		}

		void ThrowIfCompleted()
		{
			if ( IsCommitted ) throw new InvalidOperationException( "This transaction has already been committed" );
			if ( IsRolledback ) throw new InvalidOperationException( "This transaction has already been rolled back" );
		}

		public void Commit()
		{
			ThrowIfCompleted();

			IsCommitted = true;

			Log( "COMMITTED", false );

			Committed( this, EventArgs.Empty );
		}

		public void Rollback()
		{
			ThrowIfCompleted();

			IsRolledback = true;

			Log( "ROLLEDBACK", false );

			Rolledback( this, EventArgs.Empty );
		}

		//-----------------------------------------------------------------------------------------
		// IDisposable

		bool _disposed = false;

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		~NTransaction()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( !_disposed )
			{
				_disposed = true;

				if ( !IsCompleted ) Rollback();

				Interlocked.Decrement( ref s_liveTransactions );
				Log( "DISPOSE", !disposing );

				Disposed( this, EventArgs.Empty );
			}
		}
	}

	//-----------------------------------------------------------------------------------------
}
