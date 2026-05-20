using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vertex.ViewModels.Reminders;

namespace Vertex.Views.Reminders;

public partial class ReminderViewWindow : UserControl
{
    private RemindersViewModel? Vm => DataContext as RemindersViewModel;
    
    public ReminderViewWindow()
    {
        InitializeComponent();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
        Vm.CleanReminderWindowFields();
        
    }

    private void OnDrag(object sender, MouseButtonEventArgs e) => Window.GetWindow(this)?.DragMove();
    

    private void OnHourScroll(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
            Vm.RemindHourUp();
        else
            Vm.RemindHourDown();
    }

    private void OnMinuteScroll(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0)
            Vm.RemindMinuteUp();
        else
            Vm.RemindMinuteDown();
    }

    private void OnMeridiemScroll(object sender, MouseWheelEventArgs e) => Vm.UpdateMeridiem();
        

}