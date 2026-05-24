using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.Models.Entities.Entry;
using Vertex.Models.Enums;
using Vertex.MVVM;
using Vertex.Views.Activities;

namespace Vertex.ViewModels.Activities;

public class ActivitiesViewModel : ViewModelBase
{
    private ActivitiesHandler ActivitiesData { get; set; }
    
    private readonly ActivityFormViewModel _form;
    
    private ObservableCollection<ActivityItemViewModel> _currentDayActivities;
    public ObservableCollection<ActivityItemViewModel> ActivitiesForToday
    {
        get => _currentDayActivities;
        set
        {
            _currentDayActivities = value;
            OnPropertyChanged();
        }
    }
    
    private ObservableCollection<ActivityItemViewModel> _allActivities;
    public ObservableCollection<ActivityItemViewModel> ActivitiesNotForToday
    {
        get => _allActivities;
        set
        {
            _allActivities = value;
            OnPropertyChanged();
        }
    }
    
    public RelayCommand OnAddActivityView { get; }
    
    public ActivitiesViewModel(ActivitiesHandler activitiesHandler)
    {
        ActivitiesData = activitiesHandler;
        
        _form = new ActivityFormViewModel(activitiesHandler);

        ActivitiesForToday = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!.Where(a => a!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(a => new ActivityItemViewModel(a)));

        ActivitiesNotForToday = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!.Where(a => !a!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(a => new ActivityItemViewModel(a)));

        activitiesHandler.Activities!.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
                foreach (ActivityEntry entry in e.NewItems)
                {
                    ActivitiesNotForToday.Add(new ActivityItemViewModel(entry));
                    if (entry.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                        ActivitiesForToday.Add(new ActivityItemViewModel(entry));
                }
            
            if (e.OldItems != null)
                foreach (ActivityEntry entry in e.OldItems)
                {
                    var vmOne = ActivitiesNotForToday.FirstOrDefault(a => a.EntryData!.Id == entry.Id);
                    if (vmOne != null)
                        ActivitiesNotForToday.Remove(vmOne);
                    var vmTwo = ActivitiesForToday.FirstOrDefault(a => a.EntryData!.Id == entry.Id);
                    if (vmTwo != null)
                        ActivitiesForToday.Remove(vmTwo);
                }
        };
        
        WeakReferenceMessenger.Default.Register<DeleteActivityMessage>(this, (r, msg) =>
            DeleteActivity(msg.Value));
        
        WeakReferenceMessenger.Default.Register<ChangeActivityStateMessage>(this, (r, msg) =>
            MarkActivityAsDone(msg.Value.Item1, msg.Value.Item2));
        
        WeakReferenceMessenger.Default.Register<ActivityEditedMessage>(this, (r, msg) =>
            ReloadCollection());
        
        WeakReferenceMessenger.Default.Register<EditActivityMessage>(this, (r, msg) =>
        {
            _form.CleanFields();
            _form.LoadForEdit(msg.Value);
            OpenFormWindow();
        });
        
        OnAddActivityView = new RelayCommand(_ =>
        {
            _form.CleanFields();
            OpenFormWindow();
        });
        
    }
    
    private void MarkActivityAsDone(string id, bool done)
    {
        var activityEntry = ActivitiesData.Activities!.FirstOrDefault(x => x.Id == id);
        if (activityEntry == null) return;
        activityEntry.Done = done;

        ActivitiesData.Serialize();
        ReloadCollection();
        
    }
    private void DeleteActivity(string activityId)
    {
        var activityEntry = ActivitiesData.Activities!.FirstOrDefault(x => x.Id == activityId);
        if (activityEntry == null) return;
        ActivitiesData.Delete(activityEntry!);
    }
    private void ReloadCollection()
    {
        ActivitiesForToday = null;
        ActivitiesForToday = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!.Where(x => x!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(x => new ActivityItemViewModel(x)));
        
        ActivitiesNotForToday = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!.Where(x => !x!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .Select(x => new ActivityItemViewModel(x)));
    }
    
    private void OpenFormWindow()
    {
        var window = new Window
        {
            Title = "ActivityWindowView",
            DataContext = _form,
            Owner = Application.Current.MainWindow,
            WindowStyle = WindowStyle.None,
            ResizeMode = ResizeMode.NoResize,
            AllowsTransparency = true,
            Background = Brushes.Transparent,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Width = 400,
            Height = 550,
            Content = new AddActivityWindow()
        };

        _form.SetCloseAction(() => window.Close());
        window.ShowDialog();
    }
    
    private bool _showAllActivities;

    public bool ShowAllActivities
    {
        get => _showAllActivities;
        set
        {
            _showAllActivities = value;
            OnPropertyChanged();
        }
    }
}