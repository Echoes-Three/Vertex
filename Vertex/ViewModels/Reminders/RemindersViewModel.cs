using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Messaging;
using Vertex.Data.Handlers;
using Vertex.Data.Services;
using Vertex.Models.Entities;
using Vertex.MVVM;
using Vertex.ViewModels.DonutGraph;
using Vertex.Views.Reminders;

namespace Vertex.ViewModels.Reminders;

public class RemindersViewModel : ViewModelBase
{
    private RemindersHandler RemindersData { get; set; }
    
    private readonly ReminderFormViewModel _form;

    public ObservableCollection<ReminderItemViewModel> Reminders
    {
        get;
        set
        {
            field = value;
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
                    .OrderBy(r => r.SetFor)
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
                .OrderBy(r => r.SetFor)
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
             Content = new AddReminderWindow(),
             Icon = BitmapFrame.Create(new Uri("pack://application:,,,/Assets/Icon/VertexIcon.ico"))
         }; 
         
         _form.SetCloseAction(() => window.Close());
         window.ShowDialog();
     }
}