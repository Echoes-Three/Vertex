using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Vertex.ViewModels;
using Vertex.ViewModels.DonutGraph;

namespace Vertex.Views.DonutGraph;

public partial class DonutGraphTabWindow : UserControl
{
    private DonutGraphViewModel? Vm => (DataContext as MainWindowViewModel)?.DonutGraphVM;
    
    public DonutGraphTabWindow()
    {
        InitializeComponent();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Vm.OnMouseDown(sender, e, DonutCanvas);
    
    private void OnMouseMove(object sender, MouseEventArgs e) => Vm.OnMouseMove(e, DonutCanvas);
    
    private void OnMouseUp(object sender, MouseButtonEventArgs e) => Vm.OnMouseUp();


    private async void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Path path)
        {
            path.Focus();
            e.Handled = true;
            Vm.ShowActivityInfo = true;
            await Task.Delay(8000);
            Keyboard.ClearFocus();
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(path), null);
        }
    }

    private void GotFocus(object sender, RoutedEventArgs e)
    {
        
        if (sender is Path path)
        {
            Vm.PopulateActivityInfo(path.Tag);
        }
        
    }

    private void LostFocus(object sender, RoutedEventArgs e)
    {
        Vm.CleanActivityInfo();
        Vm.ShowActivityInfo = false;
    }
}