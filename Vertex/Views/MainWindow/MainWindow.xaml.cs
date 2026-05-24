using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Vertex.ViewModels;

namespace Vertex.Views.MainWindow;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
        
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        
        const double widthPercentage = 0.8;
        
        Width = screenWidth * widthPercentage;
        Height = Width * 0.5625;
    }
}