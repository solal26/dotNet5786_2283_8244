using System;
using System.Collections.Generic;
using Helpers;

namespace BO;

public class Order
{
    public int Id { get; init; }

    public OrderType Type { get; set; }

    public string Description { get; set; }

    public string FullAddress { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double AirDistance { get; set; }

    public string CustomerName { get; set; }

    public string CustomerPhone { get; set; }

    public double Volume { get; set; }

    public double Weight { get; set; }

    public DateTime OpenTime { get; init; }

    public DateTime ExpectedDeliveryTime { get; set; }

    public DateTime MaxDeliveryTime { get; set; }

    public OrderStatus OrderStatus { get; set; }

    public ScheduleStatus ScheduleStatus { get; set; }

    public TimeSpan TimeRemaining { get; set; }

    public List<DeliveryPerOrderInList> Deliveries { get; set; }

    public override string ToString()
    {
        return this.ToStringProperty();
    }
}
