using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Prism.Regions;
using Prism.Regions.Behaviors;
using Xceed.Wpf.AvalonDock;

namespace PT.ALC.Infrastructure
{
  public class DockingManagerDocumentsSourceSyncBehavior : RegionBehavior, IHostAwareRegionBehavior
  {
    public static readonly string BehaviorKey = "DockingManagerDocumentsSourceSyncBehavior";
    private readonly ObservableCollection<object> _documents = new ObservableCollection<object>();

    private DockingManager _dockingManager;
    private ReadOnlyObservableCollection<object> _readonlyDocumentsList;
    private bool _updatingActiveViewsInManagerActiveContentChanged;
    private UserControl startView;

    public ReadOnlyObservableCollection<object> Documents
    {
      get
      {
        if (_readonlyDocumentsList == null)
          _readonlyDocumentsList = new ReadOnlyObservableCollection<object>(_documents);

        return _readonlyDocumentsList;
      }
    }

    public DependencyObject HostControl
    {
      get => _dockingManager;
      set => _dockingManager = value as DockingManager;
    }

    /// <summary>
    ///   Starts to monitor the <see cref="IRegion" /> to keep it in synch with the items of the <see cref="HostControl" />.
    /// </summary>
    protected override void OnAttach()
    {
      var itemsSourceIsSet = _dockingManager.DocumentsSource != null;

      if (itemsSourceIsSet) throw new InvalidOperationException();

      SynchronizeItems();

      _dockingManager.ActiveContentChanged += ManagerActiveContentChanged;
      Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
      Region.Views.CollectionChanged += Views_CollectionChanged;
    }

    private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (_updatingActiveViewsInManagerActiveContentChanged) return;

      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        if (_dockingManager.ActiveContent != null && _dockingManager.ActiveContent != e.NewItems[0] &&
            Region.ActiveViews.Contains(_dockingManager.ActiveContent))
          Region.Deactivate(_dockingManager.ActiveContent);

        _dockingManager.ActiveContent = e.NewItems[0];
      }
      else if (e.Action == NotifyCollectionChangedAction.Remove &&
               e.OldItems.Contains(_dockingManager.ActiveContent))
      {
        _dockingManager.ActiveContent = null;
      }
    }

    private void ManagerActiveContentChanged(object sender, EventArgs e)
    {
      try
      {
        _updatingActiveViewsInManagerActiveContentChanged = true;

        if (_dockingManager == sender)
        {
          var activeContent = _dockingManager.ActiveContent;
          if (activeContent != null)
          {
            if (activeContent != startView) _documents.Remove(startView);

            foreach (var item in Region.ActiveViews.Where(it => it != activeContent))
              if (Region.Views.Contains(item))
                Region.Deactivate(item);

            if (Region.Views.Contains(activeContent) && !Region.ActiveViews.Contains(activeContent))
              Region.Activate(activeContent);
          }
          else
          {
            if (!_documents.Contains(startView)) _documents.Add(startView);
          }
        }
      }
      finally
      {
        _updatingActiveViewsInManagerActiveContentChanged = false;
      }
    }

    private void SynchronizeItems()
    {
      BindingOperations.SetBinding(
        _dockingManager,
        DockingManager.DocumentsSourceProperty,
        new Binding("Documents") {Source = this});

      foreach (var view in Region.Views) _documents.Add(view);
    }

    private void Views_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.Action == NotifyCollectionChangedAction.Add)
      {
        var startIndex = _documents.Count;
        foreach (var newItem in e.NewItems)
        {
          _documents.Insert(startIndex++, newItem);
          if (startView == null) startView = (UserControl) e.NewItems[0];
        }
      }
      else if (e.Action == NotifyCollectionChangedAction.Remove)
      {
        foreach (var oldItem in e.OldItems)
          if (_documents.Contains(oldItem))
            _documents.Remove(oldItem);
      }
    }
  }
}