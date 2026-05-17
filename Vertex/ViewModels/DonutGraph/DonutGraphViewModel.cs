using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Models.Entities.Entry;
using Vertex.MVVM;
using Vertex.ViewModels.Activities;

namespace Vertex.ViewModels.DonutGraph;

public class DonutGraphViewModel : ViewModelBase
{
    private ActivitiesHandler ActivitiesData { get; set; }
    
    private ObservableCollection<SliceViewModel> _slices;

    public ObservableCollection<SliceViewModel> Slices
    {
        get => _slices;
        set
        {
            _slices = value;
            OnPropertyChanged();
        }
    }
    
    public DonutGraphViewModel(ActivitiesHandler activitiesHandler)
    {
        ActivitiesData = activitiesHandler;
        
        RebuildSlices();
        
        activitiesHandler.Activities!.CollectionChanged += (s, e) =>
        {
            var collectionChanged = false;
            
            if (e.OldItems != null)
            {
                foreach (ActivityEntry entry in e.OldItems)
                {
                    var vm = Slices.FirstOrDefault(x => x.EntryData!.Id == entry.Id);
                    if (vm == null) continue;
                    Slices.Remove(vm);
                    collectionChanged = true;
                }
            }
            
            if (e.NewItems != null)
            {
                foreach (ActivityEntry entry in e.NewItems)
                {
                    if (!entry.RepeatOn!.Contains(DateTime.Today.DayOfWeek)) continue;
                    Slices.Add(new SliceViewModel(entry, Slices.Count, this));
                    collectionChanged = true;
                }
            }

            if (!collectionChanged) return;
            for (var i = 0; i < Slices.Count; i++)
            {
                Slices[i].SliceOrder = i;
                Slices[i].RecalculateAngles();
            }
        };

        WeakReferenceMessenger.Default.Register<RebuildSlicesMessage> (this, (r, m) => 
            { RebuildSlices();});
    }
    
    public void RebuildSlices()
    {
        var filteredActivities = ActivitiesData.Activities!
            .Where(x => x!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
            .ToList();

        var newList = filteredActivities.Select((t, i) => new SliceViewModel(t, i, this)).ToList();
        
        Slices = new ObservableCollection<SliceViewModel>(newList);

        foreach (var t in Slices)
        {
            t.RecalculateAngles();
        }
    }

    private double _lastClockDegree;

    public double LastClockDegree
    {
        get => _lastClockDegree;
        set
        {
            _lastClockDegree = value;
            OnPropertyChanged();
        }
    }

    private SliceViewModel? _dragSlice;

    public SliceViewModel? DragSlice
    {
        get => _dragSlice;
        set
        {
            _dragSlice = value;
            OnPropertyChanged();
        }
    }

    private bool _isDragging;

    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            _isDragging = value;
            OnPropertyChanged();
        }
    }

    
    
    
}