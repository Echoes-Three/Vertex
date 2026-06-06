using System.Windows.Documents;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Colors = Vertex.Data.Services.Colors;

namespace Vertex.ViewModels.Reminders;

public class ReminderFormViewModel : ViewModelBase
{
    private readonly RemindersHandler _remindersData;
    
    private readonly List<string> _meridiem = ["AM", "PM"];
    private int _currentMeridiemIndex;
    private int _currentHourCount = 12;
    private int _currentMinuteCount = 59;

    private WindowMode _windowMode = WindowMode.Add;
    
    private Action? _closeWindow;
    public void SetCloseAction(Action close) => _closeWindow = close;
    
    public RelayCommand OnSaveAction { get; }
    
    public ReminderFormViewModel(RemindersHandler remindersHandler)
    {
        _remindersData = remindersHandler;
        
        OnSaveAction = new RelayCommand(_ => SaveAction(), _ => CanSaveAction());
    }
    
    /*Saving Reminder*/
    private bool CanSaveAction()
    {
        var isTitleNotEmpty = ValidateReminder.Content(ReminderContent);
        var isDateNotEmpty = ValidateReminder.Date(ReminderSetFor);
        (bool IsValid, string Message) isHourValid = (false, string.Empty);
        
        if (isDateNotEmpty.IsValid)
            isHourValid = ReminderSetFor == DateTime.Today 
                ? ValidateReminder.Hour(_currentHourCount, _currentMinuteCount, CurrentMeridiem)
                : (true, "");
        
        var canAdd = isTitleNotEmpty.IsValid && isHourValid.IsValid && isDateNotEmpty.IsValid;
        
        var parts = new List<string> {isTitleNotEmpty.Message, isDateNotEmpty.Message, isHourValid.Message}
            .Where(e => !string.IsNullOrWhiteSpace(e));
        
        WarningMessages = parts.Any() ? string.Join("\n", parts) : "No warning." ;
        WarningColor = canAdd ? Colors.GetBrush("#C3FE0C") : Colors.GetBrush("#ea163b");
        ShowWarning = canAdd;
        return canAdd;
    }
    private void SaveAction()
    {
        if (_windowMode == WindowMode.Add)
            SaveNewReminder();
        else
            SaveEditReminder();
    }
    
    public void LoadForEdit(string reminderId)
    {
        _windowMode = WindowMode.Edit;
        
        var reminderEntry = _remindersData.Reminders!.FirstOrDefault(x => x.Id == reminderId);
        if (reminderEntry == null) return;

        var hour12 = reminderEntry.SetFor.Hour % 12 == 0 ? 12 :reminderEntry.SetFor.Hour % 12; ;
        var minute = reminderEntry.SetFor.Minute;
        var meridiemCount = reminderEntry.SetFor.ToString("tt") == "AM" ? 0 : 1;
        var meridiem = reminderEntry.SetFor.ToString("tt");
        
        EditReminderId = reminderId;
        ReminderContent = reminderEntry.Content;
        ReminderSetFor = new DateTime(reminderEntry.SetFor.Year, reminderEntry.SetFor.Month, reminderEntry.SetFor.Day);
        
        (_currentHourCount, CurrentHour) = ( hour12, hour12.ToString("D2"));
        (_currentMinuteCount, CurrentMinute) = (minute, minute.ToString("D2"));
        (_currentMeridiemIndex, CurrentMeridiem) = (meridiemCount, meridiem);
    }
    
    private void SaveEditReminder()
    {
        var reminderEntry = _remindersData.Reminders!.FirstOrDefault(x => x.Id == EditReminderId);
        if (reminderEntry == null) return;
        
        reminderEntry.Content = ReminderContent;
        reminderEntry.SetFor = ConvertDateTime();
        
        _remindersData.Serialize();
        _closeWindow?.Invoke();
        
        CleanFields();
        WeakReferenceMessenger.Default.Send(new ReminderEditedMessage());
        WeakReferenceMessenger.Default.Send(new RelaunchOrbitersMessage());
    }
    private void SaveNewReminder()
    {
        var reminder = new ReminderEntry
        {
            Content = ReminderContent,
            SetFor = ConvertDateTime(),
            Id = Guid.NewGuid().ToString()
        };

        _remindersData.Save(reminder);
        _closeWindow?.Invoke();
        
        CleanFields();
    }
    
    private static SolidColorBrush LimitColor(int length, int limit) =>
        (limit, length) switch
        {
            var (lim, len) when lim - len > lim * 0.2 => Colors.GetBrush("#C3FE0C"),
            var (lim, len) when len == lim => Colors.GetBrush("#ea163b"),
            _ => Colors.GetBrush("#0c4af7")
        };
    
    public string WarningMessages
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public Brush? WarningColor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public bool ShowWarning
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public Brush? ContentLimitIndicator
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    
    /*Helper Methods*/
    public void CleanFields()
    {
        _windowMode = WindowMode.Add;
        EditReminderId = "";
        (ReminderSetFor, ReminderContent) = (null, "");
        (_currentHourCount, CurrentHour) = (12, "12");
        (_currentMinuteCount, CurrentMinute) = (59, "59");
        (_currentMeridiemIndex, CurrentMeridiem) = (0, "AM");
    }
    private DateTime ConvertDateTime()
    {
        switch (CurrentMeridiem)
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
    
    
    /*HourPicker scroll behavior on AddReminderWindow*/
    public void UpdateMeridiem() => 
        CurrentMeridiem = _meridiem[_currentMeridiemIndex = (_currentMeridiemIndex + 1) % 2];
    public void RemindHourUp()
    {
        if (_currentHourCount == 12)
            _currentHourCount = 1;
        else
            _currentHourCount++;
        
        CurrentHour = $"{_currentHourCount:D2}";
    }
    public void RemindHourDown()
    {
        if (_currentHourCount == 1)
            _currentHourCount = 12;
        else
            _currentHourCount--;
        
        CurrentHour = $"{_currentHourCount:D2}";
    }
    public void RemindMinuteUp()
    {
        if (_currentMinuteCount == 59)
            _currentMinuteCount = 0;
        else
            _currentMinuteCount++;
        
        CurrentMinute = $"{_currentMinuteCount:D2}";
    }
    public void RemindMinuteDown()
    {
        if (_currentMinuteCount == 0)
            _currentMinuteCount = 59;
        else
            _currentMinuteCount--;
        
        CurrentMinute = $"{_currentMinuteCount:D2}";
    }
    
    public string CurrentHour
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "12";
    public string CurrentMinute
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "59";
    public string CurrentMeridiem
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "AM";
    
    
    /*Remaining Properties*/
    private string EditReminderId
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public string ReminderContent
    {
        get;
        set
        {
            field = CharacterLimiter.LimitReminderContent(ref value);
            OnPropertyChanged();
            ContentLimitIndicator = LimitColor(field.Length, 250);
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "";
    public DateTime? ReminderSetFor
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }
 
    
}