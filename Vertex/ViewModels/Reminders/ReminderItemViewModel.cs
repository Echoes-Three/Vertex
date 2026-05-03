using System.Collections.ObjectModel;
using Vertex.Models.DataServices.DataHandling;
using Vertex.Models.UserData.Entry;
using Vertex.MVVM;

namespace Vertex.ViewModels.Reminders;

public class ReminderItemViewModel : ViewModelBase
{
    public ReminderEntry? Data { get; }
    
    private int _currentRemindDayIndex;
    private readonly List<string> _sortRemindDays = ["1 Day", "2 Days", "3 Days", "4 Days", "5 Days", "6 Days", "7 Days"];

    public RelayCommand OnRemindDay { get; }

    public ReminderItemViewModel(ReminderEntry  entry)
    {
        Data = entry;
        OnRemindDay = new RelayCommand(_ => UpdateRemindDay());
    }
    
    private void UpdateRemindDay() => CurrentRemindDay = _sortRemindDays[_currentRemindDayIndex = (_currentRemindDayIndex + 1) % 7];

    private string _currentRemindDay = "1 Day";
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