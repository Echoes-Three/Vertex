using Vertex.Models.DataServices.DataHandling;
using Vertex.MVVM;

namespace Vertex.ViewModels;

public class ConsistencyViewModel : ViewModelBase
{
    private ConsistencyHandler ConsistencyData {get; set;}
    public ConsistencyViewModel( ConsistencyHandler consistencyData)
    {
        ConsistencyData = consistencyData;
    }
}