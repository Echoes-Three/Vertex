using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Models.Entities;
using Vertex.Models.Entities.Entry;
using Vertex.MVVM;
using Vertex.Views.Activities;

namespace Vertex.ViewModels.Activities;

public class ActivityItemViewModel : ViewModelBase
{
    public ActivityEntry? EntryData { get; }
    
    public RelayCommand OnDeleteActivity { get; }
    public RelayCommand OnEditActivity { get; }
    
    public ActivityItemViewModel(ActivityEntry entry)
    {
        EntryData =  entry;
        ActivityColor = ActivityColors.Palette[EntryData.Color];
        
        OnDeleteActivity = new RelayCommand(_ => DeleteActivity());
        OnEditActivity = new RelayCommand(_ => EditActivity());

        _isDone = EntryData.Done;
    }

    private void DeleteActivity() =>
        WeakReferenceMessenger.Default.Send(new DeleteActivityMessage(EntryData!.Id));

    private void EditActivity() =>
        WeakReferenceMessenger.Default.Send(new EditActivityMessage(EntryData!.Id));

    private void MarkActivityAsDone() =>
        WeakReferenceMessenger.Default.Send(new ChangeActivityStateMessage((EntryData!.Id, IsDone)));
    
    
    private Brush? _activityColor;

    public Brush? ActivityColor
    {
        get => _activityColor;
        set
        {
            _activityColor = value;
            OnPropertyChanged();
        }
    }

    private bool _isDone;

    public bool IsDone
    {
        get => _isDone;
        set
        {
            _isDone = value;
            OnPropertyChanged();
            MarkActivityAsDone();
        }
    }

}

