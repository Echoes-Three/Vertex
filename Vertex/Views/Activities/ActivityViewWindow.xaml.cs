using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vertex.ViewModels.Activities;
using Vertex.ViewModels.Reminders;

namespace Vertex.Views.Activities;

public partial class ActivityViewWindow : UserControl
{
    private ActivitiesViewModel? Vm => DataContext as ActivitiesViewModel;
    
    public ActivityViewWindow()
    {
        InitializeComponent();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
        Vm.CleanActivityWindowFields();
    }

    private void OnDrag(object sender, MouseButtonEventArgs e) => Window.GetWindow(this)?.DragMove();
    
    private void OnHourScroll(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
            Vm.DurationHourUp();
        else
            Vm.DurationHourDown();
    }

    private void OnMinuteScroll(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
            Vm.DurationMinuteUp();
        else
            Vm.DurationMinuteDown();
    }
}