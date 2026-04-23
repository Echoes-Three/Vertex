using Vertex.Models.UserData.DataHandling;
using Vertex.Models.UserDataHandling;
using Vertex.MVVM;

namespace Vertex.ViewModels;

public class BreaksViewModel : ViewModelBase
{
    private BreaksHandler _breaksHandler;
    
    private BreaksHandler BreaksData { get; set; }
    public BreaksViewModel( BreaksHandler breaksHandlerData)
    {
        BreaksData = breaksHandlerData;
    }
    
    public void OnAddBreak()
    {
        var breaks = new BreakEntry(
            BreakId,
            BreakDurationHour,
            BreakPlacementOrder);

        _breaksHandler.Save(breaks);
    }

    public bool CanAddBreak()
    {
        return true;
    }
    
    private string _breakId;
    public string BreakId
    {
        get => _breakId;
        set
        {
            _breakId = value;
            OnPropertyChanged();
        }
    }

    private TimeSpan _breakDurationHour;

    public TimeSpan BreakDurationHour
    {
        get => _breakDurationHour;
        set
        {
            _breakDurationHour = value;
            OnPropertyChanged();
        }
    }

    private int _breakPlacementOrder;

    public int BreakPlacementOrder
    {
        get => _breakPlacementOrder;
        set
        {
            _breakPlacementOrder = value;
            OnPropertyChanged();
        }
    }
}