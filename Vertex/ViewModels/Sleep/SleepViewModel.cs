using Vertex.Models.DataServices.DataHandling;

namespace Vertex.ViewModels;

public class SleepViewModel
{
    private SleepHandler SleepData { get; set; }
    
    public SleepViewModel(SleepHandler sleepData)
    {
        SleepData = sleepData;
    }
}