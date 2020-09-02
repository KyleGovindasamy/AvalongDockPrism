using AvalonDockTest.Views;
using Prism.Mvvm;
using Prism.Regions;
using Unity;

namespace AvalonDockTest.ViewModels
{
  public class MainWindowViewModel : BindableBase
  {
    private string _title = "Prism Application";

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(IUnityContainer container, IRegionManager regionManager)
    {
      regionManager.RegisterViewWithRegion("MenuRegion", typeof(MenuView));
            //regionManager.RegisterViewWithRegion("ContentRegion", typeof(ContentView));
            //var startView = container.Resolve<ContentView>();
            //var region = regionManager.Regions["ContentRegion"];
            //region.Add(startView);
            //regionManager.AddToRegion("ContentRegion", startView);
    }

    public string Title
    {
      get => _title;
      set => SetProperty(ref _title, value);
    }
  }
}