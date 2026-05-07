using System.Windows.Controls;
using System.Windows.Input;
using Vertex.ViewModels;
using Vertex.ViewModels.Reminders;

namespace Vertex.Views.Reminders;

public partial class ReminderItemView : UserControl
{
    public ReminderItemView()
    {
        InitializeComponent();
    }

    private void OnRemindScroll(object sender, MouseWheelEventArgs e)
    {
        if (DataContext is ReminderItemViewModel vm)
        {
            if (e.Delta > 0)
                vm.RemindDayUp();
            else
                vm.RemindDayDown();
            
            e.Handled = true;
        }
    }
}