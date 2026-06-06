using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vertex.ViewModels.Activities;

namespace Vertex.Views.Activities;

public partial class AddActivityWindow : UserControl
{
    private ActivityFormViewModel? Vm => DataContext as ActivityFormViewModel;
    
    public AddActivityWindow()
    {
        InitializeComponent();
        
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
        Vm?.CleanFields();
    }

    private void OnDrag(object sender, MouseButtonEventArgs e) => Window.GetWindow(this)?.DragMove();

    private void OnHourScroll(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0) Vm?.DurationHourUp();
        else Vm?.DurationHourDown();
    }

    private void OnMinuteScroll(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0) Vm?.DurationMinuteUp();
        else Vm?.DurationMinuteDown();
    }

    private void OnColorsScroll(object sender, MouseWheelEventArgs e)
    {
        if (e.Delta > 0) Vm?.ColorIndexUp();
        else Vm?.ColorIndexDown();
    }
    
}