using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Vertex.ViewModels;

namespace Vertex.Views.MainWindow;

public partial class MainWindow : Window
{
    
    private MainWindowViewModel? Vm => DataContext as MainWindowViewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
        
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        
        const double widthPercentage = 0.8;
        
        Width = screenWidth * widthPercentage;
        Height = Width * 0.5625;
    }

    private void EnterSetHour(object sender, MouseEventArgs e) => Vm.SetHourEnter();

    private void LeaveSetHour(object sender, MouseEventArgs mouseEventArgs) => _ = Vm.SetHourFocus();
    
}