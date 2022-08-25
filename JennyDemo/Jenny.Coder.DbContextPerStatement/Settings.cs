using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Config;

namespace Jenny.Coder
{
	public static class DbContextPerStatement
	{
		public const string CoderName = "DbContextPerStatement";
	}

	public static class Settings
	{
		public const string OUTPUT_FILEPATH_DOG = "OUTPUT_FILEPATH_DOG";
		public const string OUTPUT_FILEPATH_DAL = "OUTPUT_FILEPATH_DAL";
		public const string OUTPUT_FILEPATH_REP = "OUTPUT_FILEPATH_REP";
		public const string OUTPUT_FILEPATH_CTX = "OUTPUT_FILEPATH_CTX";
		public const string OUTPUT_FILEPATH_DDL = "OUTPUT_FILEPATH_DDL";

		public const string OUTPUT_NAMESPACE_REP = "OUTPUT_NAMESPACE_REP";
		public const string OUTPUT_CLASSNAME_REP = "OUTPUT_CLASSNAME_REP";
		public const string OUTPUT_CLASSFULLNAME_LUT = "OUTPUT_CLASSFULLNAME_LUT";

		public const string OUTPUT_NAMESPACE_ASPECT = "OUTPUT_NAMESPACE_ASPECT";

		public static Fluent DbContextPerStatement( this IConfig config )
		{
			return new Fluent( config.CoderSettings );
		}

		public class Fluent
		{
			readonly ICoderSettings _settings = null;

			public Fluent( ICoderSettings settings )
			{
				_settings = settings;
			}

			public Fluent Output_Filepath_DOG( string filepath )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_FILEPATH_DOG ), filepath );

				return this;
			}

			public Fluent Output_Filepath_DAL( string filepath )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_FILEPATH_DAL ), filepath );

				return this;
			}

			public Fluent Output_Filepath_REP( string filepath )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_FILEPATH_REP ), filepath );

				return this;
			}

			public Fluent Output_Filepath_CTX( string filepath )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_FILEPATH_CTX ), filepath );

				return this;
			}

			public Fluent Output_Filepath_DDL( string filepath )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_FILEPATH_DDL ), filepath );

				return this;
			}

			public Fluent Output_Namespace_REP( string @namespace )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_NAMESPACE_REP ), @namespace );

				return this;
			}

			public Fluent Output_ClassName_REP( string className )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_CLASSNAME_REP ), className );

				return this;
			}

			public Fluent Output_ClassFullName_LoginUserToken( string classFullName )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_CLASSFULLNAME_LUT ), classFullName );

				return this;
			}

			public Fluent Output_Namespace_Aspect( string @namespace )
			{
				_settings.Add( new CoderConfigKey( Coder.DbContextPerStatement.CoderName, OUTPUT_NAMESPACE_ASPECT ), @namespace );

				return this;
			}
		}
	}
}
