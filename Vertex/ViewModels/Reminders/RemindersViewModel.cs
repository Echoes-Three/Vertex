using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Enums;
using Vertex.MVVM;
using Vertex.Views.Reminders;

namespace Vertex.ViewModels.Reminders;

public class RemindersViewModel : ViewModelBase
{
    private RemindersHandler RemindersData;
    
    private ObservableCollection<ReminderItemViewModel> _remindersNotDone;

    public ObservableCollection<ReminderItemViewModel> RemindersNotDone
    {
        get => _remindersNotDone;
        set
        {
            _remindersNotDone = value;
            OnPropertyChanged();
        }
    }

    private ObservableCollection<ReminderItemViewModel> _remindersDone;

    public ObservableCollection<ReminderItemViewModel> RemindersDone
    {
        get => _remindersDone;
        set
        {
            _remindersDone = value;
            OnPropertyChanged();
        }
    }

    private readonly List<string> _icons = ["Closest", "Created"];
    private int _currentIconIndex;

    private readonly List<string> _meridiem = ["AM", "PM"];
    private int _currentMeridiemIndex;
    private int _currentHourCount = 12;
    private int _currentMinuteCount = 59;

    private WindowMode _windowMode = WindowMode.Add;
    
    private Window? ReminderWindowView { get; set; }
    public RelayCommand OnSortIcon { get; }
    public RelayCommand OnAddNewReminder { get; }
    public RelayCommand OnSaveAction { get; }

public RemindersViewModel(RemindersHandler remindersHandler)
    {
        RemindersData = remindersHandler;
        
        RemindersNotDone = new ObservableCollection<ReminderItemViewModel>(
            remindersHandler.Reminders!.Where(x => !x.Done).Select(x => new ReminderItemViewModel(x)));

        RemindersDone = new ObservableCollection<ReminderItemViewModel>(
            remindersHandler.Reminders!.Where(x => x.Done).Select(x => new ReminderItemViewModel(x)));
        
        remindersHandler.Reminders!.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
                foreach (ReminderEntry entry in e.NewItems)
                    RemindersNotDone.Add(new ReminderItemViewModel(entry));

            if (e.OldItems != null)
                foreach (ReminderEntry entry in e.OldItems)
                {
                    var vm = RemindersNotDone.FirstOrDefault(x => x.EntryData!.Id == entry.Id);
                    if (vm != null)
                        RemindersNotDone.Remove(vm);
                }
        };
        
        WeakReferenceMessenger.Default.Register<DeleteReminderMessage>(this, (r, msg) =>
            DeleteReminder(msg.Value));

        WeakReferenceMessenger.Default.Register<MarkReminderAsDoneMessage>(this, (r, msg) =>
            MarkReminderAsDone(msg.Value));

        WeakReferenceMessenger.Default.Register<RestoreReminderMessage>(this, (r, msg) =>
            RestoreReminder(msg.Value));

        WeakReferenceMessenger.Default.Register<EditReminderMessage>(this, (r, msg) =>
            EditReminder(msg.Value));
        
        OnSortIcon = new RelayCommand(_ => UpdateSortIcon());
        OnAddNewReminder = new RelayCommand(_ => ReminderWindow());
        OnSaveAction = new RelayCommand(_ => SaveAction(), _ => CanSaveAction());
    }

    /*Add/Edit Window Actions*/
    private void ReminderWindow()
    {
        if (ReminderWindowView == null)
        {
            ReminderWindowView = new Window
            {
                Title = "ReminderViewWindow",
                DataContext = this,
                Owner = Application.Current.MainWindow,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 450,
                Height = 150,
                Content = new ReminderViewWindow()
            }; 
               
            ReminderWindowView.Closed += (_, _) =>  ReminderWindowView = null;
            ReminderWindowView.ShowDialog();
        }
    }

    private bool CanSaveAction() =>
        !string.IsNullOrEmpty(ReminderContent) && ReminderSetFor != null;
    private void SaveAction()
    {
        if (_windowMode == WindowMode.Add)
            SaveNewReminder();
        else
            SaveEditReminder();
    }
    
    private void SaveEditReminder()
    {
        var reminderEntry = RemindersData.Reminders!.FirstOrDefault(x => x.Id == EditReminderId);
        if (reminderEntry == null) return;
        
        reminderEntry.Content = ReminderContent;
        reminderEntry.SetFor = ConvertDateTime();
        reminderEntry.Done = false;
        reminderEntry.DoneAt = default;
      

        RemindersData.Serialize();
        ReminderWindowView!.Close();
        
        CleanReminderWindowFields();
        ReloadCollections();
    }
    private void SaveNewReminder()
    {
        var reminder = new ReminderEntry
        {
            Content = ReminderContent,
            SetFor = ConvertDateTime(),
            Done = false,
            CreatedAt = DateTime.Now,
            DoneAt = default,
            Id = Guid.NewGuid().ToString()
        };

        RemindersData.Save(reminder);
        ReminderWindowView!.Close();
        
        CleanReminderWindowFields();
    }
   
    
    /*Actions on RemindersNotDoneView*/
    private void MarkReminderAsDone(string reminderId)
    {
        var reminderEntry = RemindersData.Reminders!.FirstOrDefault(x => x.Id == reminderId);
        if (reminderEntry == null) return;
        
        reminderEntry!.Done = true;
        reminderEntry!.DoneAt = DateTime.Now;
        
        RemindersData.Serialize();

        ReloadCollections();
    }
    private void DeleteReminder(string reminderId)
    {
        var reminderEntry = RemindersData.Reminders!.FirstOrDefault(x => x.Id == reminderId);
        if (reminderEntry == null) return;
        RemindersData.Delete(reminderEntry!);
        
    }
    private void EditReminder(string reminderId)
    {
        _windowMode = WindowMode.Edit;
        
        var reminderEntry = RemindersData.Reminders!.FirstOrDefault(x => x.Id == reminderId);
        if (reminderEntry == null) return;

        var hour12 = reminderEntry.SetFor.Hour % 12 == 0 ? 12 :reminderEntry.SetFor.Hour % 12; ;
        var minute = reminderEntry.SetFor.Minute;
        var meridiemCount = reminderEntry.SetFor.ToString("tt") == "AM" ? 0 : 1;
        var meridiem = reminderEntry.SetFor.ToString("tt");
        
        EditReminderId = reminderId;
        ReminderContent = reminderEntry.Content;
        ReminderSetFor = new DateTime(reminderEntry.SetFor.Year, reminderEntry.SetFor.Month, reminderEntry.SetFor.Day);
        
        (_currentHourCount, CurrentRemindHour) = ( hour12, hour12.ToString());
        (_currentMinuteCount, CurrentRemindMinute) = (minute, minute.ToString("D2"));
        (_currentMeridiemIndex, CurrentRemindMeridiem) = (meridiemCount, meridiem);
        
        ReminderWindow();
        
        
    }
    
    
    /*Actions on RemindersDoneView*/
    private void RestoreReminder(string reminderId)
    {
        var reminderEntry = RemindersData.Reminders!.FirstOrDefault(x => x.Id == reminderId);
        if (reminderEntry == null) return;
        
        reminderEntry!.Done = false;
        reminderEntry!.DoneAt = default;
        
        RemindersData.Serialize();

        ReloadCollections();
    }
    private void SortReminders(string orderby)
    {
        RemindersDone = orderby switch
        {
            "Created" => new ObservableCollection<ReminderItemViewModel>(
                RemindersDone.OrderBy(x => x.EntryData!.CreatedAt)),

            "Closest" => new ObservableCollection<ReminderItemViewModel>(
                RemindersDone.OrderBy(x => x.EntryData!.SetFor))
        };
        
        RemindersNotDone = orderby switch
        {
            "Created" => new ObservableCollection<ReminderItemViewModel>(
                RemindersNotDone.OrderBy(x => x.EntryData!.CreatedAt)),

            "Closest" => new ObservableCollection<ReminderItemViewModel>(
                RemindersNotDone.OrderBy(x => x.EntryData!.SetFor))
        };
    }
    
    
    /*Helper methods*/
    public void CleanReminderWindowFields()
    {
        _windowMode = WindowMode.Add;
        EditReminderId = "";
        (ReminderSetFor, ReminderContent) = (null, "");
        (_currentHourCount, CurrentRemindHour) = (12, "12");
        (_currentMinuteCount, CurrentRemindMinute) = (59, "59");
        (_currentMeridiemIndex, CurrentRemindMeridiem) = (0, "AM");
    }
    private void ReloadCollections()
    {
        RemindersNotDone = new ObservableCollection<ReminderItemViewModel>(
            RemindersData.Reminders!.Where(x => !x.Done).Select(x => new ReminderItemViewModel(x)));

        RemindersDone = new ObservableCollection<ReminderItemViewModel>(
            RemindersData.Reminders!.Where(x => x.Done).Select(x => new ReminderItemViewModel(x)));
    }
    private DateTime ConvertDateTime()
    {
        switch (_currentRemindMeridiem)
        {
            case "PM" when _currentHourCount != 12:
                _currentHourCount += 12;
                break;
            case "AM" when _currentHourCount == 12:
                _currentHourCount = 0;
                break;
        }
        
        return new DateTime(
            ReminderSetFor!.Value.Year,
            ReminderSetFor!.Value.Month,
            ReminderSetFor!.Value.Day,
            _currentHourCount,
            _currentMinuteCount,
            0
        );
    }

    
    /*HourPicker scroll behavior on ReminderViewWindow*/
    private void UpdateSortIcon() => 
        CurrentIcon = _icons[_currentIconIndex = (_currentIconIndex + 1) % 2];
    public void UpdateMeridiem() => 
        CurrentRemindMeridiem = _meridiem[_currentMeridiemIndex = (_currentMeridiemIndex + 1) % 2];
    
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

    
    /*Full Properties*/
    private string _editReminderId;

    public string EditReminderId
    {
        get => _editReminderId;
        set
        {
            _editReminderId = value;
            OnPropertyChanged();
        }
    }

    
    private bool _isRemindersHistoryOn;

    public bool IsRemindersHistoryOn
    {
        get => _isRemindersHistoryOn;
        set
        {
            _isRemindersHistoryOn = value;
            OnPropertyChanged();
        }
    }

    
    private string _currentRemindMeridiem = "AM";

    public string CurrentRemindMeridiem
    {
        get => _currentRemindMeridiem;
        set
        {
            _currentRemindMeridiem = value;
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
            SortReminders(value);
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
            OnSaveAction.RaiseCanExecuteChanged();
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
            OnSaveAction.RaiseCanExecuteChanged();
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