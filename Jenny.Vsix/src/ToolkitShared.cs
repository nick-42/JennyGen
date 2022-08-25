using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell.Interop;

namespace Jenny.Vsix
{
	public static class ToolkitShared
	{
		public static IEnumerable<SolutionItem> GetActiveItems()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var hierarchies = GetSelectedHierarchies();
			var items = new List<SolutionItem>();

			foreach ( var hierarchy in hierarchies )
			{
				var item = FromHierarchy( hierarchy.HierarchyIdentity.Hierarchy, hierarchy.HierarchyIdentity.ItemID );

				if ( item != null ) items.Add( item );
			}

			return items;
		}

		public static SolutionItem GetActiveItem()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var items = GetActiveItems();
			return items?.FirstOrDefault();
		}

		public static Project GetActiveProject()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var item = GetActiveItem();
			if ( item == null ) return null;

			if ( item.Type == SolutionItemType.Project ) return item as Project;

			return item.FindParent( SolutionItemType.Project ) as Project;
		}

		public static IEnumerable<IVsHierarchyItem> GetSelectedHierarchies()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var svc = GetMonitorSelection();
			var hierPtr = IntPtr.Zero;
			var containerPtr = IntPtr.Zero;

			List<IVsHierarchyItem> results = new();

			try
			{
				svc.GetCurrentSelection( out hierPtr, out var itemId, out var multiSelect, out containerPtr );

				AddHierarchiesFromSelection( hierPtr, itemId, multiSelect, results );
			}
			catch ( Exception )
			{
				// await ex.LogAsync();
			}
			finally
			{
				if ( hierPtr != IntPtr.Zero ) Marshal.Release( hierPtr );
				if ( containerPtr != IntPtr.Zero ) Marshal.Release( containerPtr );
			}

			return results;
		}

		public static IVsMonitorSelection GetMonitorSelection()
		{
			return GetRequiredService<SVsShellMonitorSelection, IVsMonitorSelection>();
		}

		public static IVsSolution GetSolution()
		{
			return GetRequiredService<SVsSolution, IVsSolution>();
		}

		public static TInterface GetRequiredService<TService, TInterface>() where TService : class where TInterface : class
		{
			ThreadHelper.ThrowIfNotOnUIThread();
			return (TInterface) ServiceProvider.GlobalProvider.GetService( typeof( TService ) );
		}

		internal static void AddHierarchiesFromSelection(
			IntPtr hierPtr,
			uint itemId,
			IVsMultiItemSelect multiSelect,
			List<IVsHierarchyItem> hierarchies
		)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if ( itemId == VSConstants.VSITEMID_SELECTION && multiSelect is not null )
			{
				multiSelect.GetSelectionInfo( out var itemCount, out var _ );

				var items = new VSITEMSELECTION[itemCount];
				multiSelect.GetSelectedItems( 0, itemCount, items );

				hierarchies.Capacity = (int) itemCount;

				foreach ( var item in items )
				{
					if ( item.pHier != null )
					{
						hierarchies.Add( item.pHier.ToHierarchyItem( item.itemid ) );
					}
					else
					{
						var solution = (IVsHierarchy) GetSolution();
						hierarchies.Add( solution.ToHierarchyItem( VSConstants.VSITEMID_ROOT ) );
					}
				}
			}
			else if ( itemId == VSConstants.VSITEMID_NIL )
			{
				// Empty Solution Explorer or nothing selected, so don't add anything.
			}
			else if ( hierPtr != IntPtr.Zero )
			{
				var hierarchy = (IVsHierarchy) Marshal.GetUniqueObjectForIUnknown( hierPtr );
				hierarchies.Add( hierarchy.ToHierarchyItem( itemId ) );
			}
			else if ( GetSolution() is IVsHierarchy solution )
			{
				hierarchies.Add( solution.ToHierarchyItem( VSConstants.VSITEMID_ROOT ) );
			}
		}

		public static IVsHierarchyItem ToHierarchyItem( this IVsHierarchy hierarchy, uint itemId )
		{
			if ( hierarchy == null ) throw new ArgumentNullException( nameof( hierarchy ) );

			var manager = GetMefService<IVsHierarchyItemManager>();
			return manager.GetHierarchyItem( hierarchy, itemId );
		}

		public static TInterface GetMefService<TInterface>() where TInterface : class
		{
			var compService = GetRequiredService<SComponentModel, IComponentModel2>();
			return compService.GetService<TInterface>();
		}

		public static SolutionItem FromHierarchy( IVsHierarchy hierarchy, uint itemId )
		{
			if ( hierarchy is null ) throw new ArgumentNullException( nameof( hierarchy ) );

			var item = hierarchy.ToHierarchyItem( itemId );

			return SolutionItem.FromHierarchyItem( item );
		}
	}
}
