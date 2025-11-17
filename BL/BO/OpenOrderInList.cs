using System;
using BO;
using Helpers;

namespace BO;

public class OpenOrderInList
{
    public int? CourierId { get; init; }

    public int OrderId { get; init; }

    public OrderType Type { get; init; }

    public double Volume { get; init; }

    public double Weight { get; init; }

    public string FullAddress { get; init; }

    public double AirDistance { get; init; }

    public double? ActualDistance { get; init; }

    public TimeSpan? EstimatedActualTime { get; init; }

    public ScheduleStatus ScheduleStatus { get; init; }

    public TimeSpan TimeRemaining { get; init; }

    public DateTime MaxDeliveryTime { get; init; }

    public override string ToString()
    {
        return this.ToStringProperty();
    }
}