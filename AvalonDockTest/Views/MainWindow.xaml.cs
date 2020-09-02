using System.Windows;
using Prism.Regions;
using Unity;

namespace AvalonDockTest.Views
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    public MainWindow(IUnityContainer container, IRegionManager regionManager) : this()
    {
      Container = container;
      RegionManager = regionManager;

      Loaded += MainWindow_Loaded;
    }

    public IUnityContainer Container { get; }
    public IRegionManager RegionManager { get; }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      var contentView = Container.Resolve<ContentView>();
      var region = RegionManager.Regions["ContentRegion"];
      region.Add(contentView);
    }
  }
}