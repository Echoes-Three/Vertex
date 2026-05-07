using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.Entities;
using Vertex.Models.Entities.Entry;
using Vertex.Models.UserData.Entry;
using Vertex.MVVM;
using Vertex.Views.Reminders;

namespace Vertex.ViewModels.Reminders;

public class RemindersViewModel : ViewModelBase
{
    public ObservableCollection<ReminderItemViewModel> Reminders {get; set;}
    public RemindersHandler RemindersData;
    
    private readonly List<string> _sortIcons = ["Closest", "Created"];
    private int _currentIconIndex;
    
    private readonly List<string> _meridiem = ["AM", "PM"];
    private int _currentMeridiemIndex;
    
    private int _currentHourCount = 12;
    private int  _currentMinuteCount = 59;

    private Window? CurrentAddReminder { get; set; }
    public RelayCommand OnSortIcon { get; }
    public RelayCommand OnSaveNewReminder { get; }
    public RelayCommand OnAddNewReminder { get; }


    public RemindersViewModel( RemindersHandler remindersHandler)
    {
        Reminders = new ObservableCollection<ReminderItemViewModel>(
            remindersHandler.Reminders!.Select(x => new ReminderItemViewModel(x)));

        remindersHandler.Reminders!.CollectionChanged += (s, e) =>
        {
            if (e.NewItems == null) return;
            foreach (ReminderEntry entry in e.NewItems)
                Reminders.Add(new ReminderItemViewModel(entry));
        };
        
        RemindersData = remindersHandler;
        
        OnSortIcon = new RelayCommand(_ => UpdateSortIcon());
        OnSaveNewReminder = new RelayCommand(_ => SaveNewReminder(), _ => CanSaveNewReminder());
        OnAddNewReminder = new RelayCommand(_ => AddReminder());

    }

    

    private void AddReminder()
    {
        if (CurrentAddReminder == null)
        {
            CurrentAddReminder = new Window
            {
                Title = "AddReminder",
                DataContext = this,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 450,
                Height = 150,
                Content = new AddReminder()
            }; 
               
            CurrentAddReminder.Closed += (_, _) =>  CurrentAddReminder = null;
            CurrentAddReminder.Show();
        }
        
        CurrentAddReminder.Activate();

    }

    private bool CanSaveNewReminder() => string.IsNullOrEmpty(ReminderContent)  && ReminderSetFor != null;
    public void SaveNewReminder()
    {
        
        switch (_currentMeridiem)
        {
            case "PM" when _currentHourCount != 12:
                _currentHourCount += 12;
                break;
            case "AM" when _currentHourCount == 12:
                _currentHourCount = 0;
                break;
        }

        var setFor = new DateTime(
            ReminderSetFor!.Value.Year,
            ReminderSetFor!.Value.Month,
            ReminderSetFor!.Value.Day,
            _currentHourCount,
            _currentMinuteCount,
            0
            );
        
        var reminder = new ReminderEntry
        {
            Content = ReminderContent,
            Setfor = setFor,
            Completed = false,
            CreatedAt = DateTime.Now,
            DonedAt = default,
            Id = new Guid().ToString()
        };

        RemindersData.Save(reminder);
        
        CurrentAddReminder!.Close();
    }
    
    private void UpdateSortIcon() => CurrentIcon = _sortIcons[_currentIconIndex = (_currentIconIndex + 1) % 2];
    public void UpdateMeridiem() => CurrentMeridiem = _meridiem[_currentMeridiemIndex = (_currentMeridiemIndex + 1) % 2];
    
    public void RemindHourUp()
    {
        if (_currentHourCount == 12)
            _currentHourCount = 1;
        else
            _currentHourCount++;
        
        CurrentRemindHour = $"{_currentHourCount}";
    }

    public void RemindHourDown()
    {
        if (_currentHourCount == 1)
            _currentHourCount = 12;
        else
            _currentHourCount--;
        
        CurrentRemindHour = $"{_currentHourCount}";
    }

    public void RemindMinuteUp()
    {
        if (_currentMinuteCount == 59)
            _currentMinuteCount = 0;
        else
            _currentMinuteCount++;
        
        CurrentRemindMinute = $"{_currentMinuteCount:D2}";
    }

    public void RemindMinuteDown()
    {
        if (_currentMinuteCount == 0)
            _currentMinuteCount = 59;
        else
            _currentMinuteCount--;
        
        CurrentRemindMinute = $"{_currentMinuteCount:D2}";
    }
    
    private string _currentMeridiem = "AM";

    public string CurrentMeridiem
    {
        get => _currentMeridiem;
        set
        {
            _currentMeridiem = value;
            OnPropertyChanged();
        }
    }

    private string _currentIcon = "Closest";

    public string CurrentIcon
    {
        get => _currentIcon;
        set
        {
            _currentIcon = value;
            OnPropertyChanged();
        }
    }
    
    private string _reminderContent = "";

    public string ReminderContent
    {
        get => _reminderContent;
        set
        {
            _reminderContent = value;
            OnPropertyChanged();
        }
    }
    
    private DateTime? _reminderSetFor;

    public DateTime? ReminderSetFor
    {
        get => _reminderSetFor;
        set
        {
            _reminderSetFor = value;
            OnPropertyChanged();
        }
    }
    
    private string _currentRemindHour = "12";

    public string CurrentRemindHour
    {
        get => _currentRemindHour;
        set
        {
            _currentRemindHour = value;
            OnPropertyChanged();
        }
    }
    
    private string _currentRemindMinute = "59";

    public string CurrentRemindMinute
    {
        get => _currentRemindMinute;
        set
        {
            _currentRemindMinute = value;
            OnPropertyChanged();
        }
    }

    
}