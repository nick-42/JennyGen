using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Coder;
using Jenny.MEF.Schema;

using Jenny.Core.Schema;

namespace Jenny.Coder.Internal
{
	partial class DbContextPerStatement
	{

		//-----------------------------------------------------------------------------------------
		// CommonCTX

		bool CommonCTX( CoderConfig config, IModel model, ref string s, Action<string> _ )
		{
			s += Eval(

@"
	//------------------------------------------------------------------------------------------------------------------------------
	// #= config.DbContextNamespaceName #

	partial class #= config.DbContextName #
	{
# if( ( ( int ) config.DbContextBuild ) < 3 ) { #
		//--------------------------------------------------------------------------------------------------------------------------
		// OnModelCreatingEx

		partial void OnModelCreatingEx( DbModelBuilder mb )
		{
# } else { #
		void Dummy()
		{
# } #
", new
{
	config,
	model,
	repoClassname = config.CoderSettings[ Settings.OUTPUT_CLASSNAME_REP ],
	aspectNamespace = config.CoderSettings[ Settings.OUTPUT_NAMESPACE_ASPECT ],
}
);

			return true;
		}

		//-----------------------------------------------------------------------------------------
		// CodeCTX

		bool CodeCTX( CoderConfig config, IModel model, ITable table, ref string s, Action<string> progress )
		{
			var indexes = table
				.Columns
				.SelectMany( column => MemberAttributes( table, column.Name, "IndexAttribute" )
					.Select( index =>
					(
						column,
						name: (string) index.GetType().GetProperty( "Name" ).GetValue( index ),
						isUnique: (bool) index.GetType().GetProperty( "IsUnique" ).GetValue( index ),
						order: (int) index.GetType().GetProperty( "Order" ).GetValue( index )
					) )
				)
				.GroupBy( o => o.name )
				.ToList()
			;

			if ( !indexes.Any() ) return true;

			s += TableHeader( table, 3, false );

			foreach ( var unnamed in indexes.Where( o => String.IsNullOrWhiteSpace( o.Key ) ) )
			{
				foreach ( var index in unnamed )
				{
					if ( index.order != 0 )
					{
						progress( $"unnamed [Index] cannot be composite" );
						progress( $"unnamed [Index] on {table.Name}.{index.column.Name} must have order: 0" );
						return false;
					}

					s += Eval(
@"			mb.Entity<#= dog #>()
				.HasIndex( o => o.#= index.Item1.Name # )# 
if( index.Item3 ) 
{#
				.IsUnique()#
}#;

", new { config, model, table, dog = "global::" + table.NamespaceName, index } );
				}
			}

			foreach ( var index in indexes.Where( o => !String.IsNullOrWhiteSpace( o.Key ) ) )
			{
				s += Eval(
@"			mb.Entity<#= dog #>()
				.HasIndex( o => new { #=
String.Join( "", "", index.OrderBy( o => o.Item4 ).Select( o => ""o."" + o.Item1.Name ) )
			# } )
				.HasName( ""#= index.Key #"" )# 
if( index.Any( o => o.Item3 ) ) 
{#
				.IsUnique()#
}#;

", new { config, model, table, dog = "global::" + table.NamespaceName, index } );
			}

			//----------------------------------------------------------------------------------------------

			return true;
		}

		//-----------------------------------------------------------------------------------------

	}
}
