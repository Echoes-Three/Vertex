using System.Windows.Controls;
using System.Windows.Input;

namespace Vertex.Views.Activities;

public partial class ActivitiesItem : UserControl
{
    public ActivitiesItem()
    {
        InitializeComponent();
    }

    private new void MouseLeave(object sender, MouseEventArgs e)
    {
        ActivityToolTip.IsOpen = false;
    }
}