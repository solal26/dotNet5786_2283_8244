namespace BO;
using Helpers;
public class DeliveryPerOrderInList
{
    public int DeliveryId { get; init; }
    public int? CourierId { get; init; }
    public string CourierName { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public DateTime StartTime { get; init; }
    public DeliveryEndType? EndType { get; init; }
    public DateTime? EndTime { get; init; }

    public override string ToString() => this.ToStringProperty();
}