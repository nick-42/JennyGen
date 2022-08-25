using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace NBootstrap.Global
{

	//---------------------------------------------------------------------------------------------
	// enums

	public enum UpdateMapType
	{
		Data,
		UI,
	}

	public enum UpdateMapUIType
	{
		TableSeperatorRow,

		ExplicitHtml,
	}

	//---------------------------------------------------------------------------------------------
	// UpdateMap

#warning UpdateMap Navigation Properties

	public class UpdateMap<TModel>
	{
		public List<UpdateMapEntry<TModel>> Entries { get; private set; }

		public List<UpdateMapData<TModel>> AllDataEntries
		{
			get
			{
				return Entries.OfType<UpdateMapData<TModel>>().ToList();
			}
		}

		public List<UpdateMapData<TModel>> DataEntries
		{
			get
			{
				return AllDataEntries.Where( o => !o.ReadonlyValue && o.IsPropertyOf( x => x ) ).ToList();
			}
		}

		public UpdateMap()
		{
			Entries = new List<UpdateMapEntry<TModel>>();
		}

		public UpdateMap( IEnumerable<UpdateMapEntry<TModel>> entries )
		{
			Entries = new List<UpdateMapEntry<TModel>>( entries ?? new UpdateMapEntry<TModel>[ 0 ] );
		}

		public UpdateMap( Func<UpdateMapEntryBuilder<TModel>, IEnumerable<UpdateMapEntry<TModel>>> fEntries )
		{
			Entries = new List<UpdateMapEntry<TModel>>( fEntries( new UpdateMapEntryBuilder<TModel>() ) ?? new UpdateMapEntry<TModel>[ 0 ] );
		}

		public UpdateMap<TModel> Clone()
		{
			return new UpdateMap<TModel>( Entries );
		}

		public UpdateMap<TModel> Add( IEnumerable<UpdateMapEntry<TModel>> entries )
		{
			Entries.AddRange( entries ?? new UpdateMapEntry<TModel>[ 0 ] );

			return this;
		}

		public UpdateMap<TModel> Add( Func<UpdateMapEntryBuilder<TModel>, IEnumerable<UpdateMapEntry<TModel>>> fEntries )
		{
			Entries.AddRange( fEntries( new UpdateMapEntryBuilder<TModel>() ) ?? new UpdateMapEntry<TModel>[ 0 ] );

			return this;
		}

		public UpdateMap<TModel> AddIf( bool predicate, Func<UpdateMapEntryBuilder<TModel>, IEnumerable<UpdateMapEntry<TModel>>> fEntries )
		{
			return predicate ? Add( fEntries ) : this;
		}

		public TModel CopyFields( TModel template, TModel now )
		{
			foreach ( var entry in Entries ) entry.SetValue( template, now );

			return now;
		}

		public UpdateMap<PROP> GetMapFor<PROP>( Expression<Func<TModel, PROP>> selector )
		{
			var map = new UpdateMap<PROP>();

			var propEntries = AllDataEntries.Where( o => o.IsPropertyOf( selector ) ).ToList();

			foreach ( var entry in propEntries )
			{
				var now = UpdateMapData<PROP>.CreateDataEntry( entry.PropertyInfo );

				now.CopyValuesFrom( entry );

				map.Entries.Add( now );
			}

			return map;
		}

		public UpdateMapData<TModel> Find<TProperty>( Expression<Func<TModel, TProperty>> propertyExpression )
		{
			var propertyInfo = ( (MemberExpression) propertyExpression.Body ).Member as PropertyInfo;
			if ( propertyInfo == null ) throw new UnexpectedException( "Property Expression must be a property accessor" );

			var propertyName = propertyInfo.Name;

			return AllDataEntries.SingleOrDefault( o => o.PropertyInfo.Name == propertyName );
		}
	}

	//---------------------------------------------------------------------------------------------
	// UpdateMapEntry

	public abstract class UpdateMapEntry<TModel>
	{
		public abstract UpdateMapType MapType { get; }

		public abstract void SetValue( TModel template, TModel now );

		public string LabelString { get; private set; }
		public bool ReadonlyValue { get; private set; }
		public string ReadonlyHiddenIdValue { get; private set; }
		public bool ReadonlyHiddenValueIsSet { get; private set; }
		public object ReadonlyHiddenValueValue { get; private set; }
		public bool VisibleValue { get; private set; } = true;
		public bool RawHtmlValue { get; private set; }
		public bool? RequiredValue { get; private set; }
		public bool NoInputValue { get; private set; }
		public string EditorHtmlValue { get; private set; }
		public string DisplayHtmlValue { get; private set; }
		public string TranslateTargetIsoCode { get; private set; }
		public Expression<Func<TModel, object>> TranslateSourceExpression { get; private set; }

		public IDictionary<string, object> RowHtmlAttributeDictionary { get; private set; } = new Dictionary<string, object>();
		public IDictionary<string, object> LabelHtmlAttributeDictionary { get; private set; } = new Dictionary<string, object>();
		public IDictionary<string, object> EditorHtmlAttributeDictionary { get; private set; } = new Dictionary<string, object>();
		public IDictionary<string, object> DisplayHtmlAttributeDictionary { get; private set; } = new Dictionary<string, object>();

		public Func<TModel, object> DisplayHtmlAttributeFunc { get; private set; }

		protected UpdateMapEntry() { }

		protected UpdateMapEntry( UpdateMapEntry<TModel> old )
		{
			// these require TModel

			TranslateSourceExpression = old.TranslateSourceExpression;

			DisplayHtmlAttributeFunc = old.DisplayHtmlAttributeFunc;

			CopyValuesFrom( old );
		}

		public void CopyValuesFrom<TParent>( UpdateMapEntry<TParent> old )
		{
			LabelString = old.LabelString;
			ReadonlyValue = old.ReadonlyValue;
			ReadonlyHiddenIdValue = old.ReadonlyHiddenIdValue;
			ReadonlyHiddenValueIsSet = old.ReadonlyHiddenValueIsSet;
			ReadonlyHiddenValueValue = old.ReadonlyHiddenValueValue;
			VisibleValue = old.VisibleValue;
			RawHtmlValue = old.RawHtmlValue;
			RequiredValue = old.RequiredValue;
			NoInputValue = old.NoInputValue;
			EditorHtmlValue = old.EditorHtmlValue;
			DisplayHtmlValue = old.DisplayHtmlValue;
			TranslateTargetIsoCode = old.TranslateTargetIsoCode;
			//TranslateSourceExpression = old.TranslateSourceExpression;

			RowHtmlAttributeDictionary = old.RowHtmlAttributeDictionary;
			LabelHtmlAttributeDictionary = old.LabelHtmlAttributeDictionary;
			EditorHtmlAttributeDictionary = old.EditorHtmlAttributeDictionary;
			DisplayHtmlAttributeDictionary = old.DisplayHtmlAttributeDictionary;

			//DisplayHtmlAttributeFunc = old.DisplayHtmlAttributeFunc;
		}

		//---------------------------------------------------------------------------------------------
		// fluent

		public UpdateMapEntry<TModel> Label( string label ) { LabelString = label; return this; }
		public UpdateMapEntry<TModel> Readonly( bool @readonly = true ) { ReadonlyValue = @readonly; return this; }
		public UpdateMapEntry<TModel> ReadonlyHiddenId( string readonlyHiddenId ) { ReadonlyValue = true; ReadonlyHiddenIdValue = readonlyHiddenId; return this; }
		public UpdateMapEntry<TModel> ReadonlyHiddenValue( object readonlyHiddenValue ) { ReadonlyValue = true; ReadonlyHiddenValueIsSet = true; ReadonlyHiddenValueValue = readonlyHiddenValue; return this; }
		public UpdateMapEntry<TModel> Visible( bool visible = true ) { VisibleValue = visible; return this; }
		public UpdateMapEntry<TModel> RawHtml( bool rawHtml = true ) { RawHtmlValue = rawHtml; return this; }
		public UpdateMapEntry<TModel> Required( bool required = true ) { RequiredValue = required; return this; }
		public UpdateMapEntry<TModel> NoInput( bool noinput = true ) { NoInputValue = noinput; return this; }
		public UpdateMapEntry<TModel> EditorHtml( string html ) { EditorHtmlValue = html; return this; }
		public UpdateMapEntry<TModel> DisplayHtml( string html ) { DisplayHtmlValue = html; return this; }
		public UpdateMapEntry<TModel> Translate( string targetIsoCode, Expression<Func<TModel, object>> fnSource ) { TranslateTargetIsoCode = targetIsoCode; TranslateSourceExpression = fnSource; return this; }

		public UpdateMapEntry<TModel> RowHtmlAttributes( object htmlAttributes ) { DictionaryOfStringObject.Merge( RowHtmlAttributeDictionary, htmlAttributes ); return this; }
		public UpdateMapEntry<TModel> LabelHtmlAttributes( object htmlAttributes ) { DictionaryOfStringObject.Merge( LabelHtmlAttributeDictionary, htmlAttributes ); return this; }
		public UpdateMapEntry<TModel> EditorHtmlAttributes( object htmlAttributes ) { DictionaryOfStringObject.Merge( EditorHtmlAttributeDictionary, htmlAttributes ); return this; }
		public UpdateMapEntry<TModel> DisplayHtmlAttributes( object htmlAttributes ) { DictionaryOfStringObject.Merge( DisplayHtmlAttributeDictionary, htmlAttributes ); return this; }
		public UpdateMapEntry<TModel> EditorAndDisplayHtmlAttributes( object htmlAttributes ) { DictionaryOfStringObject.Merge( EditorHtmlAttributeDictionary, htmlAttributes ); DictionaryOfStringObject.Merge( DisplayHtmlAttributeDictionary, htmlAttributes ); return this; }

		public UpdateMapEntry<TModel> DisplayHtmlAttributes( Func<TModel, object> fnHtmlAttributes ) { DisplayHtmlAttributeFunc = fnHtmlAttributes; return this; }
	}

	//---------------------------------------------------------------------------------------------
	// UpdateMapData<TModel>

	public abstract class UpdateMapData<TModel> : UpdateMapEntry<TModel>
	{
		//---------------------------------------------------------------------------------------------
		// static

		internal static UpdateMapData<TModel, TProperty, TProperty> Data<TProperty>(
			Expression<Func<TModel, TProperty>> property
		)
		{
			return new UpdateMapData<TModel, TProperty, TProperty>( property, property );
		}

		internal static UpdateMapData<TModel> CreateDataEntry( PropertyInfo propertyInfo )
		{
			if ( propertyInfo.DeclaringType != typeof( TModel ) ) throw new UnexpectedException( "Property not on correct type" );

			var open = typeof( UpdateMapData<,> );
			var closed = open.MakeGenericType( typeof( TModel ), propertyInfo.PropertyType );

			var createDataEntry = closed.GetMethod( "CreateDataEntryTyped", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );

			return (UpdateMapData<TModel>) createDataEntry.Invoke( null, new[] { propertyInfo } );
		}

		//---------------------------------------------------------------------------------------------
		// instance

		public override UpdateMapType MapType { get { return UpdateMapType.Data; } }

		public abstract Type PropertyType { get; }
		public abstract Type DisplayType { get; }
		public abstract PropertyInfo PropertyInfo { get; }
		public abstract PropertyInfo DisplayInfo { get; }

		public abstract bool IsPropertyOf<PROP>( Expression<Func<TModel, PROP>> selector );

		// replace '.' with illegal char
		public string PropertyName { get { return PropertyInfo.Name.Replace( '.', '$' ); } }

		public override void SetValue( TModel template, TModel now )
		{
			var tModel = typeof( TModel );
			var prop = PropertyInfo;

			if ( prop.DeclaringType != tModel )
			{
				throw new UnexpectedException( "!!! UpdateMapData<" + tModel.FullName + ">.SetValue contains property " + prop.DeclaringType.FullName + "." + prop.Name );
			}

			var value = prop.GetValue( template, null );

			prop.SetValue( now, value, null );
		}

		protected UpdateMapData() : base() { }

		protected UpdateMapData( UpdateMapData<TModel> old ) : base( old ) { }
	}

	//---------------------------------------------------------------------------------------------
	// UpdateMapData<TModel, TProperty>

	public abstract class UpdateMapData<TModel, TProperty> : UpdateMapData<TModel>
	{

		//---------------------------------------------------------------------------------------------
		// static

		// called by reflection - do not change this method signature
		internal static UpdateMapData<TModel, TProperty, TProperty> CreateDataEntryTyped( PropertyInfo propertyInfo )
		{
			if ( propertyInfo.DeclaringType != typeof( TModel ) ) throw new UnexpectedException( "Property not on correct type" );

			var parameter = Expression.Parameter( typeof( TModel ), "o" );
			var property = Expression.MakeMemberAccess( parameter, propertyInfo );
			var lambda = Expression.Lambda<Func<TModel, TProperty>>( property, parameter );

			return new UpdateMapData<TModel, TProperty, TProperty>( lambda, lambda );
		}

		//---------------------------------------------------------------------------------------------
		// instance

		public override Type PropertyType { get { return typeof( TProperty ); } }

		PropertyInfo _PropertyInfo = null;
		public override PropertyInfo PropertyInfo { get { return _PropertyInfo; } }
		public Expression<Func<TModel, TProperty>> PropertyExpression { get; private set; }

		public UpdateMapData(
			Expression<Func<TModel, TProperty>> property
		)
			: base()
		{
			PropertyExpression = property;

			_PropertyInfo = ( (MemberExpression) PropertyExpression.Body ).Member as PropertyInfo;
			if ( PropertyInfo == null ) throw new UnexpectedException( "Property Expression must be a property accessor" );
		}

		protected UpdateMapData(
			UpdateMapData<TModel, TProperty> old
		)
			: base( old )
		{
			PropertyExpression = old.PropertyExpression;

			_PropertyInfo = old._PropertyInfo;
		}

		public override bool IsPropertyOf<PROP>( Expression<Func<TModel, PROP>> selector )
		{
			var ps = selector.Body;

			var property = PropertyExpression.Body as MemberExpression;
			if ( property == null ) return false;

			var px = property.Expression;

			while ( ps != null && px != null )
			{
				if ( ps is ParameterExpression ps1 && px is ParameterExpression px1 && ps1.Type == px1.Type ) return true;

				var ps2 = ps as MemberExpression;
				var px2 = px as MemberExpression;
				if ( ps2 == null || px2 == null || ps2.Member != px2.Member ) return false;

				ps = ps2.Expression;
				px = px2.Expression;
			}

			return false;
		}
	}

	//---------------------------------------------------------------------------------------------
	// UpdateMapData<TModel, TProperty, TDisplayProp>

	public class UpdateMapData<TModel, TProperty, TDisplayProp> : UpdateMapData<TModel, TProperty>
	{
		public override Type DisplayType { get { return typeof( TDisplayProp ); } }

		PropertyInfo _DisplayInfo = null;
		public override PropertyInfo DisplayInfo { get { return _DisplayInfo; } }
		public Expression<Func<TModel, TDisplayProp>> DisplayExpression { get; private set; }

		public UpdateMapData(
			Expression<Func<TModel, TProperty>> property,
			Expression<Func<TModel, TDisplayProp>> display
		)
			: base( property )
		{
			SetDisplay( display );
		}

		protected UpdateMapData(
			UpdateMapData<TModel, TProperty> old,
			Expression<Func<TModel, TDisplayProp>> display
		)
			: base( old )
		{
			SetDisplay( display );
		}

		protected UpdateMapData(
			UpdateMapData<TModel, TProperty, TDisplayProp> old
		)
			: base( old )
		{
			_DisplayInfo = old._DisplayInfo;
			DisplayExpression = old.DisplayExpression;
		}

		void SetDisplay( Expression<Func<TModel, TDisplayProp>> display )
		{
			DisplayExpression = display;

			if ( DisplayExpression != null )
			{
				// can be a method call, except for DropDownLists at the moment
				_DisplayInfo = ( DisplayExpression.Body as MemberExpression )?.Member as PropertyInfo;
				//if ( DisplayInfo == null ) throw new UnexpectedException( "Display Expression must be a property accessor" );
			}
		}

		//---------------------------------------------------------------------------------------------
		// fluent

		public UpdateMapData<TModel, TProperty, TNewDisplayProp> Display<TNewDisplayProp>( Expression<Func<TModel, TNewDisplayProp>> display ) { return new UpdateMapData<TModel, TProperty, TNewDisplayProp>( this, display ); }
	}

	//---------------------------------------------------------------------------------------------
	// UI

	public class UpdateMapUI<TModel> : UpdateMapEntry<TModel>
	{
		internal static UpdateMapUI<TModel> TableSeperatorRow { get; private set; }

		static UpdateMapUI()
		{
			TableSeperatorRow = new UpdateMapUI<TModel>( UpdateMapUIType.TableSeperatorRow, null );
		}

		public static UpdateMapUI<TModel> CreateExplicitHtml( string html )
		{
			return new UpdateMapUI<TModel>( UpdateMapUIType.ExplicitHtml, html );
		}

		public override UpdateMapType MapType { get { return UpdateMapType.UI; } }

		public UpdateMapUIType UIType { get; private set; }

		public string ExplicitHtml { get; private set; }

		public UpdateMapUI( UpdateMapUIType uiType, string explicitHtml )
			: base()
		{
			UIType = uiType;

			ExplicitHtml = explicitHtml;
		}

		public override void SetValue( TModel template, TModel now )
		{
			throw new UnexpectedException( "UpdateMapUI<" + typeof( TModel ).FullName + ">.SetValue was called!" );
		}
	}

	//---------------------------------------------------------------------------------------------

}
