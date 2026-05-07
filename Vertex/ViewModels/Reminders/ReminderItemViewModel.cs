using System.Collections.ObjectModel;
using System.Windows.Media;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.UserData.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels.Reminders;

public class ReminderItemViewModel : ViewModelBase
{
    public ReminderEntry? Data { get; }
    
    private int _currentRemindDayCount = 1;
    
    public ReminderItemViewModel(ReminderEntry  entry)
    {
        Data = entry;
    }

    public void RemindDayUp()
    {
        if(_currentRemindDayCount == 7)
            return;
        _currentRemindDayCount++;
        CurrentRemindDay = $"{_currentRemindDayCount} day(s)";
    }

    public void RemindDayDown()
    {
        if(_currentRemindDayCount == 1)
            return;
        _currentRemindDayCount--;
        CurrentRemindDay = $"{_currentRemindDayCount} day(s)";
    }

    private string _currentRemindDay = "1 day(s)";
    public string CurrentRemindDay
    {
        get => _currentRemindDay;
        set
        {
            _currentRemindDay = value;
            OnPropertyChanged();
        }
    }
}