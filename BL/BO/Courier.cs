namespace BO;
using Helpers;
public class Courier
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public bool Active { get; set; }
    public double? MaxDeliveryDistance { get; set; }
    public DeliveryType DeliveryType { get; set; }
    public DateTime StartDate { get; init; }
    public int DeliveredOnTime { get; init; }
    public int DeliveredLate { get; init; }
    public OrderInProgress CurrentOrder { get; set; }

    public override string ToString() => this.ToStringProperty();
}