using System;
using BO;
using Helpers;
namespace BO;

public class OrderInProgress
{
    public int DeliveryId { get; init; }

    public int OrderId { get; init; }

    public OrderType Type { get; init; }

    public string Description { get; init; }

    public string FullAddress { get; init; }

    public double AirDistance { get; init; }

    public double? ActualDistance { get; init; }

    public string CustomerName { get; init; }

    public string CustomerPhone { get; init; }

    public DateTime OpenTime { get; init; }

    public DateTime StartTime { get; init; }

    public DateTime ExpectedDeliveryTime { get; init; }

    public DateTime MaxDeliveryTime { get; init; }

    public OrderStatus OrderStatus { get; init; }

    public ScheduleStatus ScheduleStatus { get; init; }

    public TimeSpan TimeRemaining { get; init; }

  public override string ToString()
    {
        return this.ToStringProperty();
    }
}