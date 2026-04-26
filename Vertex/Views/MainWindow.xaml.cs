using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Vertex.ViewModels;

namespace Vertex.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
        
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        
        const double widthPercentage = 0.75;
        
        Width = screenWidth * widthPercentage;
        Height = Width * 0.625;

    }
}