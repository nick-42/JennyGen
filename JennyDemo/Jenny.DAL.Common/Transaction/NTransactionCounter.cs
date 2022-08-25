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
	// NTransactionCounter

	public class NTransactionCounter : IDisposable
	{
		readonly long _connectionIndex = 0;
		readonly DbTransaction _dbTransaction = null;
		readonly List<NTransaction> _transactions = new();
		int _nestedTransactions = 0;

		void Log( string message, bool error )
		{
			NConnection.Log( _connectionIndex, "SQL TRANSACTION: " + message, error );
		}

		public NTransactionCounter( long connectionIndex, DbTransaction dbTransaction )
		{
			_connectionIndex = connectionIndex;
			_dbTransaction = dbTransaction;
		}

		public NTransaction BeginTransaction()
		{
			var transaction = new NTransaction( _connectionIndex, _nestedTransactions );

			_nestedTransactions++;
			_transactions.Add( transaction );

			transaction.Committed += ( s, e ) => CheckAllComplete();
			transaction.Rolledback += ( s, e ) => CheckAllComplete();
			transaction.Disposed += ( s, e ) => _nestedTransactions--;

			return transaction;
		}

		//-----------------------------------------------------------------------------------------
		// CheckAllComplete

		void CheckAllComplete()
		{
			if ( _transactions.Count == 0 || _transactions.Any( o => !o.IsCompleted ) ) return;

			if ( _transactions.All( o => o.IsCommitted ) )
			{
				Log( $"COMMITTED {_transactions.Count} nested transaction(s)", false );

				try
				{
					_dbTransaction.Commit();
				}
				catch ( Exception x )
				{
					System.Log.Fatal( "_DbTransaction.Commit() failed", x: x );

					throw;
				}
			}
			else
			{
				Log( $"ROLLEDBACK: {_transactions.Count( o => !o.IsCommitted )} uncommitted of {_transactions.Count} nested transaction(s)", false );

				_dbTransaction.Rollback();
			}

			Dispose();
		}

		//-----------------------------------------------------------------------------------------
		// IDisposable

		bool _disposed = false;
		public bool IsDisposed { get { return _disposed; } }

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		~NTransactionCounter()
		{
			Dispose( false );
		}

		protected virtual void Dispose( bool disposing )
		{
			if ( !_disposed )
			{
				_disposed = true;

				foreach ( var tx in _transactions ) tx.Dispose();

				if ( disposing )
				{
					_dbTransaction.Dispose();
				}

				Log( "DISPOSE", !disposing );
			}
		}
	}

	//-----------------------------------------------------------------------------------------
}
