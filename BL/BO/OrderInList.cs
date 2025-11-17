using System;
using BO;
using Helpers;

namespace BO;

public class OrderInList
{
    
    public int? DeliveryId { get; init; }

    public int OrderId { get; init; }

    public OrderType Type { get; init; }

    public double AirDistance { get; init; }

    public OrderStatus OrderStatus { get; init; }

    public ScheduleStatus ScheduleStatus { get; init; }

    public TimeSpan TimeRemaining { get; init; }

    public TimeSpan TotalCompletionTime { get; init; }

    public int TotalDeliveries { get; init; }

    public override string ToString()
    {
        return this.ToStringProperty();
    }
}