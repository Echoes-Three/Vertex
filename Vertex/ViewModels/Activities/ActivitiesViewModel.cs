using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Vertex.Views.Activities;

namespace Vertex.ViewModels.Activities;

public class ActivitiesViewModel : ViewModelBase
{
    private ActivitiesHandler ActivitiesData { get; set; }
    
    private readonly ActivityFormViewModel _form;

    public ObservableCollection<ActivityItemViewModel> TodayActivities
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    
    public ObservableCollection<ActivityItemViewModel> RemainingActivities
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }

    private readonly int _todayIndex = (int)DateTime.Today.DayOfWeek;
    
    public RelayCommand OnAddActivityView { get; }
    
    public ActivitiesViewModel(ActivitiesHandler activitiesHandler)
    {
        ActivitiesData = activitiesHandler;
        
        _form = new ActivityFormViewModel(activitiesHandler);

        LoadCollection();

        activitiesHandler.Activities!.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
                foreach (ActivityEntry entry in e.NewItems)
                {
                    if (entry.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                        TodayActivities.Add(new ActivityItemViewModel(entry));
                    else
                        RemainingActivities.Add(new ActivityItemViewModel(entry));
                    
                    LoadCollection();
                }
            
            if (e.OldItems != null)
                foreach (ActivityEntry entry in e.OldItems)
                {
                    var vmOne = RemainingActivities.FirstOrDefault(a => a.EntryData!.Id == entry.Id);
                    if (vmOne != null)
                        RemainingActivities.Remove(vmOne);
                    var vmTwo = TodayActivities.FirstOrDefault(a => a.EntryData!.Id == entry.Id);
                    if (vmTwo != null)
                        TodayActivities.Remove(vmTwo);
                    
                    LoadCollection();
                }
        };
        
        WeakReferenceMessenger.Default.Register<DeleteActivityMessage>(this, (r, msg) =>
            DeleteActivity(msg.Value));
        
        WeakReferenceMessenger.Default.Register<ActivityEditedMessage>(this, (r, msg) =>
            LoadCollection());
        
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

        TodayIsEmpty = !TodayActivities?.Any() ?? true;
        RemainingIsEmpty = !RemainingActivities?.Any() ?? true;
    }
    
    private void LoadCollection()
    {
        TodayActivities = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!
                .Where(a => a!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .OrderBy(e => e.Title)
                .Select(a => new ActivityItemViewModel(a)));

        RemainingActivities = new ObservableCollection<ActivityItemViewModel>(
            ActivitiesData.Activities!
                .Where(a => !a!.RepeatOn.Contains(DateTime.Today.DayOfWeek))
                .OrderBy(e => e.Title)
                .Select(a => new ActivityItemViewModel(a)));
        
        TodayIsEmpty = !TodayActivities?.Any() ?? true;
        RemainingIsEmpty = !RemainingActivities?.Any() ?? true;
    }
    private void OpenFormWindow()
    {
        
        var screenHeight = SystemParameters.WorkArea.Height;
        var height = screenHeight * 0.6;
        var width = height * 0.7272;
        
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
            Width = width,
            Height = height,
            Content = new AddActivityWindow(),
            Icon = BitmapFrame.Create(new Uri("pack://application:,,,/Assets/Icon/VertexIcon.ico"))
        };
        
        _form.SetCloseAction(() => window.Close());
        window.ShowDialog();
    }
    
    private void DeleteActivity(string activityId)
    {
        var activityEntry = ActivitiesData.Activities!.FirstOrDefault(x => x.Id == activityId);
        if (activityEntry == null) return;
        ActivitiesData.Delete(activityEntry!);
    }
    
    public bool ShowRemainingActivities
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public bool TodayIsEmpty
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
    public bool RemainingIsEmpty
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged();
        }
    }
}