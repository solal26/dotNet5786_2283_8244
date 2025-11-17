namespace BO;

public enum OrderType
{
    SingleOrder,
    BulkOrder,
    ExpressOrder,
    InternalTransfer
}

public enum OrderStatus
{
    Open,
    InProgress,
    Delivered,
    Refused,
    Cancelled
    
}

public enum ScheduleStatus
{
    OnTime,
    InRisk,
    Late
}

public enum DeliveryType {
Car,
Scooter,
Bike,
OnFoot
}

public enum DeliveryEndType { 
    Delivered,
    Refused,
    Cancelled,
    NotHere,
    Failed
}



