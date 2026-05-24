using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vertex.Views.MainWindow;

public partial class TitleBarView : UserControl
{
    public TitleBarView()
    {
        InitializeComponent();
    }

    private void TitleBarDrag(object sender, MouseButtonEventArgs e)
    {
        var parentWindow = Window.GetWindow(this);
        if (e.ButtonState == MouseButtonState.Pressed) parentWindow?.DragMove();
    }

    private void MinimizeButton(object sender, RoutedEventArgs e) => Window.GetWindow(this)?.WindowState = WindowState.Minimized;
    
    private void CloseButton(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    
}