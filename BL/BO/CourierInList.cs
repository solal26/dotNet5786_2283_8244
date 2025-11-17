namespace BO;
using Helpers;
public class CourierInList
{
    public int Id { get; init; }
    public string FullName { get; init; }
    public bool Active { get; init; }
    public DeliveryType DeliveryType { get; init; }
    public DateTime StartDate { get; init; }
    public int DeliveredOnTime { get; init; }
    public int DeliveredLate { get; init; }
    public int? CurrentOrderId { get; init; }

    public override string ToString() => this.ToStringProperty();
}