using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Colors = Vertex.Data.Services.Colors;

namespace Vertex.ViewModels.Activities;

public class ActivityItemViewModel : ViewModelBase
{
    public ActivityEntry? EntryData { get; }
    public RelayCommand OnDeleteActivity { get; }
    public RelayCommand OnEditActivity { get; }
    
    public ActivityItemViewModel(ActivityEntry entry)
    {
        EntryData =  entry;
        ActivityColor = Colors.Palette[EntryData.Color];
        OnDeleteActivity = new RelayCommand(_ => DeleteActivity());
        OnEditActivity = new RelayCommand(_ => EditActivity());
        
        var d = EntryData.RepeatOn.ToBoolList();
        (Sun, Mon, Tue, Wed, Thu, Fri, Sat) = (d.Sun, d.Mon, d.Tue, d.Wed, d.Thu, d.Fri, d.Sat);
    }

    private void DeleteActivity() =>
        WeakReferenceMessenger.Default.Send(new DeleteActivityMessage(EntryData!.Id));
    private void EditActivity() =>
        WeakReferenceMessenger.Default.Send(new EditActivityMessage(EntryData!.Id));
   
    public Brush? ActivityColor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public bool Sun
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public bool Mon
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public bool Tue
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public bool Wed
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public bool Thu
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public bool Fri
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    public bool Sat
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
}

