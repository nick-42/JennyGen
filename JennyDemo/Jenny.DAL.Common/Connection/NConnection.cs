using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBootstrap.EF
{

	//-----------------------------------------------------------------------------------------
	// NConnection

	public class NConnection : IDisposable
	{
		static long s_connectionIndex = -1;
		static long s_liveConnections = 0;
		public static long LiveConnections => s_liveConnections;

		public static void Log( long connectionIndex, string message, bool error )
		{
			var m = $"CONN {connectionIndex:N0} {message} LiveConnections: {LiveConnections:N0} LiveTransactions: {NTransaction.LiveTransactions:N0}";

			if ( error )
				System.Log.Error( m, raw: true );
			else
				System.Log.Info( m, raw: true );
		}

		public long Index => _connectionIndex;
		readonly long _connectionIndex = 0;

		public DbConnection DbConnection { get; private set; }
		public DbTransaction DbTransaction { get; private set; }

		public NConnection( DbConnection dbConnection )
		{
			DbConnection = dbConnection;

			_connectionIndex = Interlocked.Increment( ref s_connectionIndex );
			Interlocked.Increment( ref s_liveConnections );

			Log( _connectionIndex, "CREATED", false );
		}

		NTransactionCounter NTransactionCounter { get; set; }

		public bool HasTransaction => NTransactionCounter != null && !NTransactionCounter.IsDisposed;

		public NTransaction BeginTransaction( System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted )
		{
			if ( NTransactionCounter == null || NTransactionCounter.IsDisposed )
			{
				if ( DbConnection.State == System.Data.ConnectionState.Closed ) DbConnection.Open();

				DbTransaction = DbConnection.BeginTransaction( isolationLevel );

				NTransactionCounter = new NTransactionCounter( _connectionIndex, DbTransaction );
			}

			return NTransactionCounter.BeginTransaction();
		}

		//-----------------------------------------------------------------------------------------
		// IDisposable

		bool _disposed = false;

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		~NConnection()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( !_disposed )
			{
				_disposed = true;

				if ( disposing )
				{
					if ( NTransactionCounter != null ) NTransactionCounter.Dispose();
					if ( DbConnection != null ) DbConnection.Dispose();
				}

				Interlocked.Decrement( ref s_liveConnections );
				Log( _connectionIndex, "DISPOSE", !disposing );
			}
		}
	}

	//-----------------------------------------------------------------------------------------

}
