using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.ViewModels;
using Vertex.ViewModels.DonutGraph;
using Colors = Vertex.Data.Services.Colors;

namespace Vertex.Views.DonutGraph;

public partial class DonutGraphTabWindow : UserControl
{
    private DonutGraphViewModel? Vm => (DataContext as MainWindowViewModel)?.DonutGraphVM;
    
    public DonutGraphTabWindow()
    {
        InitializeComponent();
        GenerateClockTicks(DonutCanvas, new Point(510, 377), 320, 15);
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => Vm.OnMouseDown(sender, e, DonutCanvas);
    
    private void OnMouseMove(object sender, MouseEventArgs e) => Vm.OnMouseMove(e, DonutCanvas);
    
    private void OnMouseUp(object sender, MouseButtonEventArgs e) => Vm.OnMouseUp();
    
    private static void GenerateClockTicks(Canvas canvas, Point center, double faceRadius, double tickLength)
    {
        for (var hour = 0; hour < 24; hour++)
        {
            var angle = hour / 24.0 * 2 * Math.PI - Math.PI / 2;

            var outerX = center.X + faceRadius * Math.Cos(angle);
            var outerY = center.Y + faceRadius * Math.Sin(angle);
            var innerX = center.X + (faceRadius - tickLength) * Math.Cos(angle);
            var innerY = center.Y + (faceRadius - tickLength) * Math.Sin(angle);

            var line = new Line
            {
                X1 = outerX, Y1 = outerY,
                X2 = innerX, Y2 = innerY,
                StrokeThickness = 6,
                Stroke = Colors.GetBrush("#26282E")
            };

            canvas.Children.Add(line);
        }
    }

    private void EnterSlice(object sender, MouseEventArgs e)
    {
        if (sender is Path path)
            Vm.PopulateActivityInfo(path.Tag);
    }

    private void LeaveSlice(object sender, MouseEventArgs e) => Vm.CleanActivityInfo();
    
}