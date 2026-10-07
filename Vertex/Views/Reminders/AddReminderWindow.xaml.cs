using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vertex.ViewModels.Reminders;

namespace Vertex.Views.Reminders;

public partial class AddReminderWindow : UserControl
{
    private ReminderFormViewModel? Vm => DataContext as ReminderFormViewModel;
    
    public AddReminderWindow()
    {
        InitializeComponent();
        
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
        Vm.CleanFields();
        
    }

}