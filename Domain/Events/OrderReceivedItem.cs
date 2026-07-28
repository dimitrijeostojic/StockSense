namespace Domain.Events;

public sealed record OrderReceivedItem(int ProductId, int Quantity);