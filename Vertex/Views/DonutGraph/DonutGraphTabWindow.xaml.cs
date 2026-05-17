using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Vertex.ViewModels;
using Vertex.ViewModels.DonutGraph;

namespace Vertex.Views.DonutGraph;

public partial class DonutGraphTabWindow : UserControl
{
    public DonutGraphTabWindow()
    {
        InitializeComponent();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel) return;
        var vm = viewModel.DonutGraphVM;
        
        var path = sender as Path;
        vm.DragSlice = path.DataContext as SliceViewModel;
        vm.IsDragging = true;

        var mouse = e.GetPosition(DonutCanvas);
        var dx = mouse.X - 510;
        var dy = mouse.Y - 320;
        var angle = Math.Atan2(dy, dx) * (180 / Math.PI);
        if (angle < 0) angle += 360;
        vm.LastClockDegree = (angle - 180 + 360) % 360;
    }
    
    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel) return;
        var vm = viewModel.DonutGraphVM;
        
        if (!vm.IsDragging || vm.DragSlice == null) return;

        var mouse = e.GetPosition(DonutCanvas);
        var dx = mouse.X - 510;
        var dy = mouse.Y - 320;
        var angle = Math.Atan2(dy, dx) * (180 / Math.PI);
        if (angle < 0) angle += 360;
        var clockDegrees = (angle - 180 + 360) % 360;

        var delta = clockDegrees - vm.LastClockDegree;
        if (delta > 180) delta -= 360;
        if (delta < -180) delta += 360;

        vm.DragSlice.StartAngle = (vm.DragSlice.StartAngle - delta + 360) % 360;
        vm.DragSlice.EndAngle = (vm.DragSlice.EndAngle - delta + 360) % 360;

        vm.LastClockDegree = clockDegrees;

    }
    
    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel) return;
        var vm = viewModel.DonutGraphVM;
        
        if (vm.DragSlice != null) 
            vm.DragSlice = null;
            
        vm.IsDragging = false;

    }

    
}