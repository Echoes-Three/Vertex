using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Vertex.MVVM;

public class SetSliceMessage((string id, double x, double dy) values) 
    : ValueChangedMessage<(string, double, double)>(values);
public class DeleteReminderMessage(string id) : ValueChangedMessage<string>(id);
public class EditReminderMessage(string id) : ValueChangedMessage<string>(id);
public class DeleteActivityMessage(string id) : ValueChangedMessage<string>(id);
public class EditActivityMessage(string id) : ValueChangedMessage<string>(id);
public class RebuildSlicesMessage;
public class ActivityEditedMessage;
public class ReminderEditedMessage;