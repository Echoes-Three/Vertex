using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Vertex.Views.Reminders;

public partial class AddReminder : UserControl
{
    public AddReminder()
    {
        InitializeComponent();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
    }

    private void OnDrag(object sender, MouseButtonEventArgs e)
    {
        Window.GetWindow(this)?.DragMove();
    }
}