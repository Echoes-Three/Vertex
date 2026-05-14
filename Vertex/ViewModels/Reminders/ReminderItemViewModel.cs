using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Models.Entities.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels.Reminders;

public class ReminderItemViewModel : ViewModelBase
{
    public ReminderEntry? EntryData { get; }
    
    public RelayCommand OnDeleteReminder { get; }
    public RelayCommand OnReminderDone { get; }
    public RelayCommand OnRestoreReminder { get; }
    public RelayCommand OnEditReminder { get; }
    public ReminderItemViewModel(ReminderEntry  entry)
    {
        EntryData = entry;
        OnDeleteReminder = new RelayCommand(_ => DeleteReminder());
        OnReminderDone = new RelayCommand(_ => MarkReminderAsDone());
        OnRestoreReminder = new RelayCommand(_ => RestoreReminder());
        OnEditReminder = new RelayCommand(_ => EditReminder());
        
        CreatedAt = EntryData.CreatedAt.ToString("yyyy-MM-dd hh:mm tt");
        SetFor = EntryData.SetFor.ToString("yyyy-MM-dd hh:mm tt");
        DoneAt = EntryData.DoneAt.ToString("yyyy-MM-dd hh:mm tt");
    }
    
    private void DeleteReminder()
        => WeakReferenceMessenger.Default.Send(new DeleteReminderMessage(EntryData!.Id));

    private void MarkReminderAsDone()
        => WeakReferenceMessenger.Default.Send(new MarkReminderAsDoneMessage(EntryData!.Id));
    
    private void RestoreReminder()
        => WeakReferenceMessenger.Default.Send(new RestoreReminderMessage(EntryData!.Id));

    private void EditReminder()
        => WeakReferenceMessenger.Default.Send(new EditReminderMessage(EntryData!.Id));
    
    
    private string _setFor;

    public string SetFor
    {
        get => _setFor;
        set
        {
            _setFor = value;
            OnPropertyChanged(nameof(EntryData));
        }
    }

    
    private string _cretedAt;

    public string CreatedAt
    {
        get => _cretedAt;
        set
        {
            _cretedAt = value;
            OnPropertyChanged(nameof(EntryData));
        }
    }

    private string _doneAt;

    public string DoneAt
    {
        get => _doneAt;
        set
        {
            _doneAt = value;
            OnPropertyChanged(nameof(EntryData));
        }
    }

    
}