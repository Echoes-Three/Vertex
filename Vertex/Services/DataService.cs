using System.IO;
using System.Windows.Media;
using Vertex.Models;
using Vertex.Models.EnumDefinitions;
using System.Text.Json;


namespace Vertex.Services;

public class DataService
{
    private readonly string _dataPath;
    private readonly string _dailyPath;
    private readonly string _breakPath;
    private readonly string _weeklyPath;
    private readonly string _archivePath;
    private readonly string _reminderPath;

    public DataService()
    {
        var basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Vertex"
        );

        _dataPath = Path.Combine(basePath, "Data");
        _dailyPath = Path.Combine(_dataPath, "Daily");
        _breakPath = Path.Combine(_dataPath, "Breaks");
        _weeklyPath = Path.Combine(_dataPath, "Weekly");
        _archivePath = Path.Combine(_dataPath, "Archive");
        _reminderPath = Path.Combine(_dataPath, "Reminders");

        Directory.CreateDirectory(_dataPath);
        Directory.CreateDirectory(_dailyPath);
        Directory.CreateDirectory(_breakPath);
        Directory.CreateDirectory(_weeklyPath);
        Directory.CreateDirectory(_archivePath);
        Directory.CreateDirectory(_reminderPath);
    }
    
}