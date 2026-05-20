using System.Windows;
using System.Windows.Controls;
using Vertex.ViewModels;

namespace Vertex.Views;

public partial class ConsistencyView : UserControl
{
    public ConsistencyView()
    {
        InitializeComponent();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var vm = (MainWindowViewModel)DataContext;
        vm.ConsistencyVM.CanvasWidth = e.NewSize.Width;
        vm.ConsistencyVM.CanvasHeight = e.NewSize.Height;
        vm.ConsistencyVM.OnCanvasChange();
    }
}