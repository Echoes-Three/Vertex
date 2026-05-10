using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Entities.Helpers;
using Vertex.MVVM;

namespace Vertex.ViewModels.Reminders;

public class ReminderItemViewModel : ViewModelBase
{
    public ReminderEntry? Data { get; }
    
    public RelayCommand OnDeleteReminder { get; }
    public RelayCommand OnReminderDone { get; }
    public RelayCommand OnRestoreReminder { get; }
    public ReminderItemViewModel(ReminderEntry  entry)
    {
        Data = entry;
        OnDeleteReminder = new RelayCommand(_ => DeleteReminder());
        OnReminderDone = new RelayCommand(_ => MarkReminderAsDone());
        OnRestoreReminder = new RelayCommand(_ => RestoreReminder());
        
        CreatedAt = Data.CreatedAt.ToString("yyyy-MM-dd HH:mm tt");
        SetFor = Data.Setfor.ToString("yyyy-MM-dd HH:mm tt");
        DoneAt = Data.DonedAt.ToString("yyyy-MM-dd HH:mm tt");
    }
    
    private void DeleteReminder()
        => WeakReferenceMessenger.Default.Send(new DeleteReminderMessage(Data!.Id));

    private void MarkReminderAsDone()
        => WeakReferenceMessenger.Default.Send(new MarkReminderAsDoneMessage(Data!.Id));
    
    private void RestoreReminder()
        => WeakReferenceMessenger.Default.Send(new RestoreReminderMessage(Data!.Id));

    private string _setFor;

    public string SetFor
    {
        get => _setFor;
        set
        {
            _setFor = value;
            OnPropertyChanged(nameof(Data));
        }
    }

    
    private string _cretedAt;

    public string CreatedAt
    {
        get => _cretedAt;
        set
        {
            _cretedAt = value;
            OnPropertyChanged(nameof(Data));
        }
    }

    private string _doneAt;

    public string DoneAt
    {
        get => _doneAt;
        set
        {
            _doneAt = value;
            OnPropertyChanged(nameof(Data));
        }
    }

    
}