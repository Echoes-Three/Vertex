using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vertex.ViewModels.Activities;
using Vertex.ViewModels.Reminders;

namespace Vertex.Views.Activities;

public partial class ActivityViewWindow : UserControl
{
    public ActivityViewWindow()
    {
        InitializeComponent();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
        
        if (DataContext is ActivitiesViewModel vm)
        {
            vm.CleanActivityWindowFields();
        }
    }

    private void OnDrag(object sender, MouseButtonEventArgs e)
    {
        Window.GetWindow(this)?.DragMove();
    }


    private void OnHourScroll(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is ActivitiesViewModel vm)
        {
            if (e.Delta > 0)
                vm.DurationHourUp();
            else
                vm.DurationHourDown();
        }
    }

    private void OnMinuteScroll(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is ActivitiesViewModel vm)
        {
            if (e.Delta > 0)
                vm.DurationMinuteUp();
            else
                vm.DurationMinuteDown();
        }
    }
}