using CommunityToolkit.Mvvm.Messaging;
using Vertex.Models.Entities;
using Vertex.MVVM;

namespace Vertex.ViewModels.Reminders;

public class ReminderItemViewModel : ViewModelBase
{
    public ReminderEntry? EntryData { get; }
    
    public RelayCommand OnDeleteReminder { get; }
    public RelayCommand OnEditReminder { get; }
    public ReminderItemViewModel(ReminderEntry  entry)
    {
        EntryData = entry;
        OnDeleteReminder = new RelayCommand(_ => DeleteReminder());
        OnEditReminder = new RelayCommand(_ => EditReminder());
        
        InitializeReminder();
    }

    private void InitializeReminder()
    {
        var dateSpan = (EntryData.SetFor - DateTime.Today).Days;

        SetFor = dateSpan switch
        {
            0 => $"Today at {EntryData.SetFor:hh:mm tt}",
            < 0 =>  $"{Math.Abs(dateSpan)} Day(s) ago",
            _=> $"In {Math.Abs(dateSpan)} Day(s)"
        };
    }
    private void DeleteReminder()
        => WeakReferenceMessenger.Default.Send(new DeleteReminderMessage(EntryData!.Id));
    private void EditReminder()
        => WeakReferenceMessenger.Default.Send(new EditReminderMessage(EntryData!.Id));


    public string SetFor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(EntryData));
        }
    }
}