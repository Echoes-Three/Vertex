using System.Collections.ObjectModel;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.UserData.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels.Reminders;

public class RemindersViewModel : ViewModelBase
{
    public ObservableCollection<ReminderItemViewModel> Reminders {get; set;}
    
    private readonly List<String> _sortIcons = ["Closest", "Created"];
    private int _currentIconIndex;
    
    public RelayCommand OnSortIcon { get; }
    
    public RemindersViewModel( RemindersHandler remindersHandler)
    {
        Reminders = new ObservableCollection<ReminderItemViewModel>(
            remindersHandler.Reminders.Select(x => new ReminderItemViewModel(x)));
        OnSortIcon = new RelayCommand(_ => UpdateSortIcon());
        
    }
    
    private void UpdateSortIcon() => CurrentIcon = _sortIcons[_currentIconIndex = (_currentIconIndex + 1) % 2];  
    
    private string _currentIcon;

    public string CurrentIcon
    {
        get => _currentIcon;
        set
        {
            _currentIcon = value;
            OnPropertyChanged();
        }
    }

    private string _canAppear = "True";

    public string CanAppear
    {
        get => _canAppear;
        set
        {
            _canAppear = value;
            OnPropertyChanged();
        }
    }
    
    private int _remindMeInDays;

    public int RemindMeInDays
    {
        get => _remindMeInDays;
        set
        {
            _remindMeInDays = value;
            OnPropertyChanged();
        }
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