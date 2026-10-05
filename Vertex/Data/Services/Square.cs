using Vertex.MVVM;

namespace Vertex.Data.Services;

public class Square : ViewModelBase
{
    public bool IsActive
    {
        get;
        set
        {
            if (Equals(field, value)) return;
            field = value;
            OnPropertyChanged();
        }
    }
}