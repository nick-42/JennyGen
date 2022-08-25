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
		// CodeDDL

		bool CodeDDL( CoderConfig config, IModel model, ITable table, ref string s, Action<string> progress )
		{
			if ( String.IsNullOrEmpty( table.DropDownListText ) ) return true;

			s += TableHeader( table, 1 );

			// class
			s += Eval(

@"	public partial class #= table.Name #DDL : DropDownListBase
	{
		Expression<Func<#= dog #, bool>> _Where = null;

		public #= table.Name #DDL( Expression<Func<#= dog #, bool>> where = null )
		{
			_Where = where;
		}

		public override List<DropDownListItem> List
		{
			get
			{
				using ( var repo = new #= repoClassname #() )
				{
					var q = repo.#= table.Name #.Read();

					if ( _Where != null ) q = q.Where( _Where );

					return q.OrderBy( o => #= String.Format( table.DropDownListSort, ""o"" ) # )
						.ToList()
						.Select( o => new DropDownListItem
						{
							Id = o.RowIdentityObject.PrimaryKeyAsString,
							Text = #= String.Format( table.DropDownListText, ""o"" ) #,
						} )
						.ToList()
					;
				}
			}
		}
	}
", new
	{
		config,
		model,
		table,
		dog = "global::" + table.NamespaceName,
		repoClassname = "global::" + config.CoderSettings[ Settings.OUTPUT_NAMESPACE_REP ] + "." + config.CoderSettings[ Settings.OUTPUT_CLASSNAME_REP ],
	}
);

			return true;
		}

		//-----------------------------------------------------------------------------------------

	}
}
