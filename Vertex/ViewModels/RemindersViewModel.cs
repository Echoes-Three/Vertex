using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.UserData.DataHandling;
using Vertex.Models.UserData.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels;

public class RemindersViewModel : ViewModelBase
{
    private RemindersHandler _remindersHandler;
    
    public RemindersViewModel()
    {
        var reminderData = new RemindersHandler();
        _remindersHandler = reminderData;
    }
    
    public void OnAddReminder()
    {
        var reminders = new ReminderEntry(
            RemiderId,
            ReminderTitle,
            ReminderContent,
            ReminderCompleted,
            ReminderCreatedAt);
        
        _remindersHandler.Save(reminders);
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

    private string _reminderTitle;

    public string ReminderTitle
    {
        get => _reminderTitle;
        set
        {
            _reminderTitle = value;
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
    
    
}