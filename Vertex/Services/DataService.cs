using System.IO;
namespace Vertex.Services;

public class DataService
{
    private readonly string _dataPath;
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
        _weeklyPath = Path.Combine(_dataPath, "Weekly");
        _archivePath = Path.Combine(_dataPath, "Archive");
        _reminderPath = Path.Combine(_dataPath, "Reminders");

        Directory.CreateDirectory(_dataPath);
        Directory.CreateDirectory(_weeklyPath);
        Directory.CreateDirectory(_archivePath);
        Directory.CreateDirectory(_reminderPath);
    }
}