using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.UserData.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels;

public class RemindersViewModel : ViewModelBase
{
    private RemindersHandler RemindersData {get; set;}
    
    public RemindersViewModel( RemindersHandler remindersHandler )
    {
        RemindersData = remindersHandler;
    }
    
    public void OnAddReminder()
    {
        var reminders = new ReminderEntry(
            RemiderId,
            ReminderContent,
            ReminderCompleted,
            ReminderCreatedAt,
            ReminderDoneAt,
            ReminderSetFor);
        
        RemindersData.Save(reminders);
    }
    
    public bool CanAddReminder()
    {
        return true;
    }

    private string _reminderId;

    public string RemiderId
    {
        get => _reminderId;
        set
        {
            _reminderId = value;
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

    private bool _reminderCompleted;

    public bool ReminderCompleted
    {
        get => _reminderCompleted;
        set
        {
            _reminderCompleted = value;
            OnPropertyChanged();
        }
    }

    private DateTime _reminderCreatedAt;

    public DateTime ReminderCreatedAt
    {
        get => _reminderCreatedAt;
        set
        {
            _reminderCreatedAt = value;
            OnPropertyChanged();
        }
    }

    private DateTime _reminderDoneAt;

    public DateTime ReminderDoneAt
    {
        get => _reminderDoneAt;
        set
        {
            _reminderDoneAt = value;
            OnPropertyChanged();
        }
    }

    private DateTime _reminderSetFor;

    public DateTime ReminderSetFor
    {
        get => _reminderSetFor;
        set
        {
            _reminderSetFor = value;
            OnPropertyChanged();
        }
    }

    
}