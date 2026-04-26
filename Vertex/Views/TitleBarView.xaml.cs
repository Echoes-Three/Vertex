using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Vertex.ViewModels;

namespace Vertex.Views;

public partial class TitleBarView : Window
{
    public TitleBarView()
    {
        InitializeComponent();
        DataContext = this;
    }
}