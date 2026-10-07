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
    private int _meridiemIndex;
    private int _hour = 12;
    private int _minute = 59;

    private WindowMode _windowMode = WindowMode.Add;
    
    private Action? _closeWindow;
    public void SetCloseAction(Action close) => _closeWindow = close;
    
    public RelayCommand OnSaveAction { get; }
    public RelayCommand OnPickHourAction { get; }
    
    public ReminderFormViewModel(RemindersHandler remindersHandler)
    {
        _remindersData = remindersHandler;
        
        OnSaveAction = new RelayCommand(_ => SaveAction(), _ => CanSaveAction());
        OnPickHourAction = new RelayCommand(param => PickHourAction(param));
    }
    
    /*Saving Reminder*/
    private bool CanSaveAction()
    {
        var isTitleNotEmpty = ValidateReminder.Content(ReminderContent);
        var isDateNotEmpty = ValidateReminder.Date(ReminderSetFor);
        (bool IsValid, string Message) isHourValid = (false, string.Empty);
        
        if (isDateNotEmpty.IsValid)
            isHourValid = ReminderSetFor == DateTime.Today 
                ? ValidateReminder.Hour(_hour, _minute, Meridiem)
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
        
        (_hour, Hour) = ( hour12, hour12.ToString("D2"));
        (_minute, Minute) = (minute, minute.ToString("D2"));
        (_meridiemIndex, Meridiem) = (meridiemCount, meridiem);
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

    public string ContentLimitIndicator
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    } = "250";
    
    
    /*Helper Methods*/
    public void CleanFields()
    {
        _windowMode = WindowMode.Add;
        EditReminderId = "";
        (ReminderSetFor, ReminderContent) = (null, "");
        (_hour, Hour) = (12, "12");
        (_minute, Minute) = (59, "59");
        (_meridiemIndex, Meridiem) = (0, "AM");
    }
    private DateTime ConvertDateTime()
    {
        switch (Meridiem)
        {
            case "PM" when _hour != 12:
                _hour += 12;
                break;
            case "AM" when _hour == 12:
                _hour = 0;
                break;
        }
        
        return new DateTime(
            ReminderSetFor!.Value.Year,
            ReminderSetFor!.Value.Month,
            ReminderSetFor!.Value.Day,
            _hour,
            _minute,
            0
        );
    }
    
    
    /*HourPicker scroll behavior on AddReminderWindow*/
    
    private void PickHourAction(object identifier)
    {
        var id = identifier.ToString();
        
        if (id!.StartsWith('H'))
        {
            if (id.EndsWith('U'))
                _hour = _hour == 12 ? 0 : _hour + 1;
            else
                _hour = _hour == 0 ? 12 : _hour - 1;
        }
        else if (id.StartsWith('M'))
        {
            if (id.EndsWith('U'))
                _minute = _minute == 59 ? 0 : _minute + 1;
            else
                _minute = _minute == 0 ? 59 : _minute - 1; 
        }
        else if (id.StartsWith('T'))
        {
           _meridiemIndex = (_meridiemIndex + 1) % 2;
        }
        
        Hour = $"{_hour:D2}";
        Minute = $"{_minute:D2}";
        Meridiem =  _meridiem[_meridiemIndex];
    }
    
    public string Hour
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "12";
    public string Minute
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
            OnSaveAction.RaiseCanExecuteChanged();
        }
    } = "59";
    public string Meridiem
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
            ContentLimitIndicator = (250 - field.Length).ToString();
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