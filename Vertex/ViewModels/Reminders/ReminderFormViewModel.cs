using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Enums;
using Vertex.MVVM;

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
    
    private bool CanSaveAction()
    {
        var isTitleNotEmpty = ValidateReminder.Content(ReminderContent);
        var isDateNotEmpty = ValidateReminder.Date(ReminderSetFor);
        (bool IsValid, string Message) isHourValid = (false, string.Empty);
        
        if (isDateNotEmpty.IsValid)
            isHourValid = ReminderSetFor == DateTime.Today 
                ? ValidateReminder.Hour(_currentHourCount, _currentMinuteCount, _currentMeridiem)
                : (true, "");
        
        var canAdd = isTitleNotEmpty.IsValid && isHourValid.IsValid && isDateNotEmpty.IsValid;
        
        var parts = new List<string> {isTitleNotEmpty.Message, isDateNotEmpty.Message, isHourValid.Message}
            .Where(e => !string.IsNullOrWhiteSpace(e));
        
        WarningMessages = parts.Any() ? string.Join("\n", parts) : "No warning." ;
        
        WarningColor = canAdd ? ActivityColors.GetBrush("#C3FE0C") : ActivityColors.GetBrush("#ea163b");
        
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
        reminderEntry.Done = false;
        
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
            Done = false,
            Id = Guid.NewGuid().ToString()
        };

        _remindersData.Save(reminder);
        _closeWindow?.Invoke();
        
        CleanFields();
    }
    
    
    /*Helper methods*/
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
        switch (_currentMeridiem)
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
    private void CountContentLimit(int length) => 
        ContentLimitCounter = (250 - length).ToString();
    
    
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
    
    
    /*Full Properties*/

    private string _warningMessages;

    public string WarningMessages
    {
        get => _warningMessages;
        set
        {
            _warningMessages = value;
            OnPropertyChanged();
        }
    }
    
    private Brush? _warningColor;

    public Brush? WarningColor
    {
        get => _warningColor;
        set
        {
            _warningColor = value;
            OnPropertyChanged();
        }
    }

    private string _contentLimitCounter = "250";

    public string ContentLimitCounter
    {
        get => _contentLimitCounter;
        set
        {
            _contentLimitCounter = value;
            OnPropertyChanged();
        }
    }

    
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
    private string _reminderContent = "";

    public string ReminderContent
    {
        get => _reminderContent;
        set
        {
            _reminderContent = CharacterLimiter.LimitReminderContent(value);
            OnPropertyChanged();
            CountContentLimit(_reminderContent.Length);
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
    
    
    private string _currentHour = "12";

    public string CurrentHour
    {
        get => _currentHour;
        set
        {
            _currentHour = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }
    
    
    private string _currentMinute = "59";

    public string CurrentMinute
    {
        get => _currentMinute;
        set
        {
            _currentMinute = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }
    
    private string _currentMeridiem = "AM";

    public string CurrentMeridiem
    {
        get => _currentMeridiem;
        set
        {
            _currentMeridiem = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    }
}