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

public partial class ActivityForm : UserControl
{
    private ActivityFormViewModel? Vm => DataContext as ActivityFormViewModel;
    
    public ActivityForm()
    {
        InitializeComponent();
        
    }

    private void MouseEnterSaveButton(object sender, MouseEventArgs e)
    {
        Vm.SetHover(true);
        Vm.StartAnimation();
    }

    private void MouseLeaveSaveButton(object sender, MouseEventArgs e)
    {
        Vm.SetHover(false);
        Vm.ResetAnimation();
    }
    
}