using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Vertex.MVVM;

public class DeleteReminderMessage(string id) : ValueChangedMessage<string>(id);
public class ChangeReminderStateMessage((string id, bool done) parameters) : ValueChangedMessage<(string, bool)>(parameters);
public class EditReminderMessage(string id) : ValueChangedMessage<string>(id);
public class DeleteActivityMessage(string id) : ValueChangedMessage<string>(id);
public class ChangeActivityStateMessage(( string Id,bool done) parameters) : ValueChangedMessage<(string, bool)>(parameters);
public class EditActivityMessage(string id) : ValueChangedMessage<string>(id);
public class RebuildSlicesMessage {}
public class RelaunchOrbitersMessage {}
public class ActivityEditedMessage {}
public class ReminderEditedMessage {}