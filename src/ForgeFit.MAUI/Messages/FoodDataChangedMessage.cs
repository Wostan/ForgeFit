namespace ForgeFit.MAUI.Messages;

public sealed record FoodDataChangedMessage(string Source, Guid? EntryId = null);