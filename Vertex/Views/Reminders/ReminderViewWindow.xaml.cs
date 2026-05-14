using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vertex.ViewModels.Reminders;

namespace Vertex.Views.Reminders;

public partial class ReminderViewWindow : UserControl
{
    public ReminderViewWindow()
    {
        InitializeComponent();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
        
        if (DataContext is RemindersViewModel vm)
        {
            vm.CleanReminderWindowFields();
        }
    }

    private void OnDrag(object sender, MouseButtonEventArgs e)
    {
        Window.GetWindow(this)?.DragMove();
    }

    private void OnHourScroll(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is RemindersViewModel vm)
        {
            if (e.Delta > 0)
                vm.RemindHourUp();
            else
                vm.RemindHourDown();
        }
    }

    private void OnMinuteScroll(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is RemindersViewModel vm)
        {
            if (e.Delta > 0)
                vm.RemindMinuteUp();
            else
                vm.RemindMinuteDown();
        }
    }

    private void OnMeridiemScroll(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is RemindersViewModel vm)
        {
            vm.UpdateMeridiem();
        }

    }

}