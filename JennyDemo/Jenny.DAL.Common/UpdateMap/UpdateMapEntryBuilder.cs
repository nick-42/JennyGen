using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NBootstrap.Global
{
	public class UpdateMapEntryBuilderMapResult<TModel> : List<UpdateMapEntry<TModel>>
	{
		public UpdateMapEntryBuilderMapResult( IEnumerable<UpdateMapEntry<TModel>> entries )
		{
			if ( entries != null ) AddRange( entries );
		}

		public UpdateMapEntryBuilderMapResult<TModel> MapIf( bool predicate, Func<UpdateMapEntryBuilder<TModel>, IEnumerable<UpdateMapEntry<TModel>>> fEntries )
		{
			if ( predicate ) AddRange(
				fEntries( new UpdateMapEntryBuilder<TModel>() )?.Where( o => o != null ) ??
				Enumerable.Empty<UpdateMapEntry<TModel>>()
			);

			return this;
		}
	}

	public class UpdateMapEntryBuilder<TModel>
	{
		public UpdateMapEntryBuilderMapResult<TModel> Map( params UpdateMapEntry<TModel>[] entries )
		{
			return new( entries?.Where( o => o != null ) );
		}

		public UpdateMapData<TModel, TProperty, TProperty> Data<TProperty>
		(
			Expression<Func<TModel, TProperty>> property
		)
		{
			return UpdateMapData<TModel>.Data( property );
		}

		public UpdateMapUI<TModel> TableSeperatorRow
		{
			get
			{
				return UpdateMapUI<TModel>.TableSeperatorRow;
			}
		}

		public UpdateMapUI<TModel> ExplicitHtml( string html )
		{
			return UpdateMapUI<TModel>.CreateExplicitHtml( html );
		}
	}

	public class UpdateMapEntryColumnBuilder<TModel>
	{
		readonly List<PropertyAndValueBase> _properties = new();

		public int ColumnCount => _properties.Count;

		public UpdateMapEntryColumnBuilder<TModel> Column<TProperty>( Expression<Func<TModel, TProperty>> propertyExpression )
		{
			_properties.Add( new PropertyAndValueDerived<TProperty>( propertyExpression ) );
			return this;
		}

		public UpdateMapEntryColumnBuilder<TModel> Column<TProperty>( Expression<Func<TModel, TProperty>> propertyExpression, TProperty value )
		{
			_properties.Add( new PropertyAndValueDerived<TProperty>( propertyExpression, value ) );
			return this;
		}

		public UpdateMapEntryColumnBuilder<TModel> ColumnIf<TProperty>( bool yes, Expression<Func<TModel, TProperty>> propertyExpression )
		{
			if ( yes ) Column( propertyExpression );
			return this;
		}

		public UpdateMapEntryColumnBuilder<TModel> ColumnIf<TProperty>( bool yes, Expression<Func<TModel, TProperty>> propertyExpression, TProperty value )
		{
			if ( yes ) Column( propertyExpression, value );
			return this;
		}

		public UpdateMap<TModel> UpdateMap( TModel template )
		{
			return new UpdateMap<TModel>( _properties.Select( o => { o.SetValue( template ); return o.UpdateMapData; } ) );
		}

		abstract class PropertyAndValueBase
		{
			public abstract UpdateMapData<TModel> UpdateMapData { get; }

			public abstract void SetValue( TModel template );
		}

		class PropertyAndValueDerived<TProperty> : PropertyAndValueBase
		{
			readonly UpdateMapData<TModel> _updateMapData;
			readonly bool _isValueSet;
			readonly TProperty _value;

			public PropertyAndValueDerived( Expression<Func<TModel, TProperty>> propertyExpression )
			{
				_updateMapData = UpdateMapData<TModel>.Data( propertyExpression );
				_isValueSet = false;
			}

			public PropertyAndValueDerived( Expression<Func<TModel, TProperty>> propertyExpression, TProperty value )
				: this( propertyExpression )
			{
				_isValueSet = true;
				_value = value;
			}

			public override UpdateMapData<TModel> UpdateMapData { get { return _updateMapData; } }

			public override void SetValue( TModel template )
			{
				if ( !_isValueSet ) return;

				_updateMapData.PropertyInfo.SetValue( template, _value );
			}
		}
	}
}
