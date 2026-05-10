using CommunityToolkit.Mvvm.Messaging.Messages;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace Vertex.Models.Entities.Helpers;

public class DeleteReminderMessage(string id) : ValueChangedMessage<string>(id);
public class MarkReminderAsDoneMessage(string id) : ValueChangedMessage<string>(id);

public class RestoreReminderMessage(string id) : ValueChangedMessage<string>(id);