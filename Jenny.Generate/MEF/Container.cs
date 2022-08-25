using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Jenny.MEF.Coder;
using Jenny.MEF.Config;

namespace Jenny.Core.MEF
{
	public interface IAdapterData
	{
		string Name { get; }
	}

	public interface ICoderData
	{
		string Name { get; }
	}

	class Container
	{
		readonly CompositionContainer _Container = null;

		[ImportMany( typeof( IAdapter ) )]
		IEnumerable<Lazy<IAdapter, IAdapterData>> _Adapters = null;
		public IEnumerable<IAdapter> Adapters { get { return _Adapters.Select( o => o.Value ).ToList(); } }

		[ImportMany( typeof( ICoder ) )]
		IEnumerable<Lazy<ICoder, ICoderData>> _Coders = null;
		public IEnumerable<ICoder> Coders { get { return _Coders.Select( o => o.Value ).ToList(); } }
		public ICoder Coder( string name ) { return _Coders.Where( o => o.Metadata.Name == name ).Select( o => o.Value ).SingleOrDefault(); }

		public Container( string configExeFilepath )
		{
			var catalog = new AggregateCatalog();

			//catalog.Catalogs.Add( new AssemblyCatalog( GetType().Assembly ) );
			catalog.Catalogs.Add( new DirectoryCatalog( Path.GetDirectoryName( Path.GetFullPath( Assembly.GetEntryAssembly().Location ) ) ) );
			catalog.Catalogs.Add( new DirectoryCatalog( Path.GetDirectoryName( Path.GetFullPath( configExeFilepath ) ) ) );
			//catalog.Catalogs.Add( new System.ComponentModel.Composition.Hosting.AssemblyCatalog( configExeFilepath ) );

			_Container = new CompositionContainer( catalog );

			_Container.ComposeParts( this );
		}

		//public void Init( ControllerLocalSettings controllerLocalSettings )
		//{
		//	System.Diagnostics.Debug.WriteLine( "CompositionContainer.Adapters: " + _Adapters.Count() );

		//	foreach ( var adapter in _Adapters )
		//	{
		//		System.Diagnostics.Debug.WriteLine( "MEF.Container calling Init() on: " + adapter.Metadata.Name );

		//		adapter.Value.Init( controllerLocalSettings );
		//	}
		//}
	}
}
