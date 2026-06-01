using Vertex.Models.Entities;
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
        var trimmed = EntryData.Content.Length > 50 ? EntryData.Content[..50] : EntryData.Content;
        ReminderContent = $"{trimmed}...";
        ReminderSetForHour = $"TODAY AT: " +
                             $"{EntryData.SetFor:hh}:{EntryData.SetFor:mm} {EntryData.SetFor:tt}";
    }
    private void HourToAngle()
    {
        var hour = EntryData.SetFor.Hour;
        var minute = EntryData.SetFor.Minute;

        var angle = 270 - (15 * hour + 0.25 * minute);

        angle = angle % 360;
        if (angle < 0) angle += 360;

        var radian = angle * (Math.PI / 180);

        var x1 = 377 * Math.Cos(radian);
        var y1 = -377 * Math.Sin(radian);

        CanvasLeft = Math.Truncate(x1) - 15;
        CanvasTop = Math.Truncate(y1) - 15;

    }

    public double CanvasLeft
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public double CanvasTop
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string ReminderSetForHour
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string ReminderContent
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
}