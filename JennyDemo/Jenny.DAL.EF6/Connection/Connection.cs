using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBootstrap.EF
{
	public class ConnectionConfig
	{
		public string DefaultConnectionString { get; set; }
	}

	public class Connection : NConnection
	{
		static ConnectionConfig s_config = null;

		public static void Init( ConnectionConfig config )
		{
			s_config = config;
		}

		public Connection()
			: base( new SqlConnection( s_config.DefaultConnectionString ) )
		{
		}

		public Connection( string connectionString )
			: base( new SqlConnection( connectionString ) )
		{
		}
	}
}
