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
using Vertex.ViewModels.DonutGraph;
using Vertex.Views.Reminders;

namespace Vertex.ViewModels.Reminders;

public class RemindersViewModel : ViewModelBase
{
    private RemindersHandler RemindersData { get; set; }
    
    private readonly ReminderFormViewModel _form;
    
    private ObservableCollection<ReminderItemViewModel> _reminders;
    public ObservableCollection<ReminderItemViewModel> Reminders
    {
        get => _reminders;
        set
        {
            _reminders = value;
            OnPropertyChanged();
        }
    }
    
    public RelayCommand OnAddNewReminder { get; }
    

public RemindersViewModel(RemindersHandler remindersHandler)
    {
        RemindersData = remindersHandler;
        
        _form = new ReminderFormViewModel(remindersHandler);
        
        Reminders = new ObservableCollection<ReminderItemViewModel>(
            RemindersData.Reminders!
                .Select(r => new ReminderItemViewModel(r))
                .OrderBy(r => r.IsDone)
                .ToList());
        
        remindersHandler.Reminders!.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
                foreach (ReminderEntry entry in e.NewItems)
                    Reminders.Add(new ReminderItemViewModel(entry));

            if (e.OldItems != null)
                foreach (ReminderEntry entry in e.OldItems)
                {
                    var vm = Reminders.FirstOrDefault(x => x.EntryData!.Id == entry.Id);
                    if (vm != null)
                        Reminders.Remove(vm);
                }
        };
        
        WeakReferenceMessenger.Default.Register<DeleteReminderMessage>(this, (r, msg) =>
            DeleteReminder(msg.Value));

        WeakReferenceMessenger.Default.Register<ChangeReminderStateMessage>(this, (r, msg) =>
            MarkReminderAsDone(msg.Value.Item1, msg.Value.Item2));

        WeakReferenceMessenger.Default.Register<ReminderEditedMessage>(this, (r, msg) =>
            ReloadCollection());
        
        WeakReferenceMessenger.Default.Register<EditReminderMessage>(this, (r, msg) =>
        {
            _form.CleanFields();
            _form.LoadForEdit(msg.Value);
            OpenFormWindow();
        });
        
        OnAddNewReminder = new RelayCommand(_ => 
        {
            _form.CleanFields();
            OpenFormWindow();
        });
    }

    /*Actions on RemindersNotDoneView*/
    private void MarkReminderAsDone(string id, bool done)
    {
        var reminderEntry = RemindersData.Reminders!.FirstOrDefault(x => x.Id == id);
        if (reminderEntry == null) return;
        
        reminderEntry!.Done = done;
        RemindersData.Serialize();
        ReloadCollection();
    }
    private void DeleteReminder(string reminderId)
    {
        var reminderEntry = RemindersData.Reminders!.FirstOrDefault(x => x.Id == reminderId);
        if (reminderEntry == null) return;
        RemindersData.Delete(reminderEntry!);
    }
     private void ReloadCollection() =>
            Reminders = new ObservableCollection<ReminderItemViewModel>(
                RemindersData.Reminders!
                    .Select(x => new ReminderItemViewModel(x))
                    .OrderBy(r => r.IsDone)
                    .ToList());
    
     private void OpenFormWindow()
     {
         var window = new Window
         {
             Title = "AddReminderWindow",
             DataContext = _form,
             Owner = Application.Current.MainWindow,
             WindowStyle = WindowStyle.None,
             ResizeMode = ResizeMode.NoResize,
             AllowsTransparency = true,
             Background = Brushes.Transparent,
             WindowStartupLocation = WindowStartupLocation.CenterScreen,
             Width = 400,
             Height = 550,
             Content = new AddReminderWindow()
         }; 
         
         _form.SetCloseAction(() => window.Close());
         window.ShowDialog();
     }
    
}