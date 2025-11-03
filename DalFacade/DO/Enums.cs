namespace DO;

/// <summary>
/// The mode of transport used for a shipment.
/// </summary>
public enum TypeOfShipment
{
    /// <summary>Delivery made by car.</summary>
    Car,
    /// <summary>Delivery made by scooter/moped.</summary>
    Scooter,
    /// <summary>Delivery made by bicycle.</summary>
    Bike,
    /// <summary>Delivery made on foot.</summary>
    OnFoot
}

/// <summary>
/// Classification of an order.
/// </summary>
public enum TypeOfOrder
{
    /// <summary>A single customer order.</summary>
    SingleOrder,
    /// <summary>A bulk order containing many items or deliveries.</summary>
    BulkOrder,
    /// <summary>A high-priority or expedited order.</summary>
    ExpressOrder,
    /// <summary>An internal transfer not for an external customer.</summary>
    InternalTransfer
}

/// <summary>
/// Final outcome of a courier attempt or delivery.
/// </summary>
public enum Endofcourier
{
    /// <summary>Package successfully delivered.</summary>
    Delivered,
    /// <summary>Recipient refused the delivery.</summary>
    Refused,
    /// <summary>Delivery canceled before completion.</summary>
    Canceled,
    /// <summary>Recipient was not present at the address.</summary>
    NotHere,
    /// <summary>Delivery failed for other reasons.</summary>
    Failed
}



