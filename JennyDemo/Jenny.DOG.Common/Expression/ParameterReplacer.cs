using System;
using System.Linq.Expressions;

namespace NBootstrap.Global
{
	public class ParameterReplacer : ExpressionVisitor
	{
		public static Expression Run(
			ParameterExpression originalParameter,
			Expression replacement,
			Expression expression
		)
		{
			var now = new ParameterReplacer( replacement, originalParameter ).Visit( expression );

			return now;
		}

		public static Expression<Func<SOURCE, RESULT>> Run<SOURCE, TARGET, RESULT>(
			Expression<Func<SOURCE, TARGET>> replacement,
			Expression<Func<TARGET, RESULT>> expression
		)
		{
			var now = new ParameterReplacer( replacement.Body, expression.Parameters[ 0 ] ).Visit( expression.Body );

			return Expression.Lambda<Func<SOURCE, RESULT>>( now, replacement.Parameters );
		}

		readonly ParameterExpression _originalParameter;
		readonly Expression _replacement;

		ParameterReplacer( Expression replacement, ParameterExpression parameter )
		{
			_replacement = replacement;
			_originalParameter = parameter;
		}

		protected override Expression VisitParameter( ParameterExpression p )
		{
			if ( p == _originalParameter ) return _replacement;

			return base.VisitParameter( p );
		}
	}
}
