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

    

    public double Weight { get; set; }

    public DateTime OpenTime { get; init; }

    public DateTime ExpectedDeliveryTime { get;init; }

    public DateTime MaxDeliveryTime { get; init; }

    public OrderStatus OrderStatus { get; init; }

    public ScheduleStatus ScheduleStatus { get; init; }

    public TimeSpan TimeRemaining { get; init; }

    public List<DeliveryPerOrderInList> Deliveries { get; init; }

    public override string ToString()
    {
        return this.ToStringProperty();
    }
}
