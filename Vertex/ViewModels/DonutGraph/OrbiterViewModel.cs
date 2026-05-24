using Vertex.Models.Entities.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels.DonutGraph;

public class OrbiterViewModel : ViewModelBase
{
    public ReminderEntry EntryData { get; set; }
    public OrbiterViewModel(ReminderEntry entry)
    {
        EntryData = entry;
        InitializeOrbiters();
    }

    private void InitializeOrbiters()
    {
        HourToAngle();
        ReminderContent = EntryData.Content;
        ReminderSetForHour = $"SET FOR TODAY AT : " +
                             $"{EntryData.SetFor:hh}:{EntryData.SetFor:mm} {EntryData.SetFor:tt}";
        var status = EntryData.Done ? "DONE" : "NOT DONE";
        ReminderStatus = $"STATUS ->> ->> {status}";
    }

    private void HourToAngle()
    {
        var hour = EntryData.SetFor.Hour;
        var minute = EntryData.SetFor.Minute;

        var angle = 270 - (15 * hour + 0.25 * minute);

        angle = angle % 360;
        if (angle < 0) angle += 360;

        var radian = angle * (Math.PI / 180);

        var x1 = 360 * Math.Cos(radian);
        var y1 = -360 * Math.Sin(radian);
        var x2 = 360 * Math.Cos(radian);
        var y2 = -360 * Math.Sin(radian);

        InnerCanvasLeft = Math.Truncate(x1) - 10;
        InnerCanvasTop = Math.Truncate(y1) - 10;
        OuterCanvasLeft = Math.Truncate(x2) - 15;
        OuterCanvasTop = Math.Truncate(y2) - 15;
    }
    private double _innerCanvasLeft;

    public double InnerCanvasLeft
    {
        get => _innerCanvasLeft;
        set
        {
            _innerCanvasLeft = value;
            OnPropertyChanged();
        }
    }

    private double _innerCanvasTop;

    public double InnerCanvasTop
    {
        get => _innerCanvasTop;
        set
        {
            _innerCanvasTop = value;
            OnPropertyChanged();
        }
    }

    private double _outerCanvasLeft;

    public double OuterCanvasLeft
    {
        get => _outerCanvasLeft;
        set
        {
            _outerCanvasLeft = value;
            OnPropertyChanged();
        }
    }

    private double _outerCanvasTop;

    public double OuterCanvasTop
    {
        get => _outerCanvasTop;
        set
        {
            _outerCanvasTop = value;
            OnPropertyChanged();
        }
    }

    private string _reminderSetForHour;

    public string ReminderSetForHour
    {
        get => _reminderSetForHour;
        set
        {
            _reminderSetForHour = value;
            OnPropertyChanged();
        }
    }

    private string _reminderContent;

    public string ReminderContent
    {
        get => _reminderContent;
        set
        {
            _reminderContent = value;
            OnPropertyChanged();
        }
    }

    private string _reminderStatus;

    public string ReminderStatus
    {
        get => _reminderStatus;
        set
        {
            _reminderStatus = value;
            OnPropertyChanged();
        }
    }

}