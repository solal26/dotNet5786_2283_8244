namespace BO;
using Helpers;
public class ClosedDeliveryInList
{
    public int DeliveryId { get; init; }
    public int OrderId { get; init; }
    public OrderType OrderType { get; init; }
    public string FullAddress { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public double? ActualDistance { get; init; }
    public TimeSpan CompletionTime { get; init; }
    public DeliveryEndType? EndType { get; init; }

    public override string ToString() => this.ToStringProperty();
}

