using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.DependencyInjection;
using Vertex.ViewModels;
using Vertex.Views.Reminders;

namespace Vertex.Views;

public partial class RemindersView : UserControl
{
    public Window currentAddReminder { get; }
    public RemindersView()
    {
        InitializeComponent();
    }

    private void OnAddReminder(object sender, RoutedEventArgs e)
    {
        var addReminderWindow = new Window
        {
            Title = "AddReminder",
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            AllowsTransparency = true,
            Background = Brushes.Transparent,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Width = 450,
            Height = 150,
            Content = new AddReminder()
        };
        
        if (addReminderWindow.IsActive)
            return;
        
        addReminderWindow.Show();
        
    }
}