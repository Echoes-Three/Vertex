using System.Windows;
using Vertex.ViewModels;

namespace Vertex;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var viewmodel = new MainWindowViewModel();
        DataContext = viewmodel;
    }
}