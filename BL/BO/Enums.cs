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

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}

public enum CourierSortBy
{
    Id,
    FullName,
    Active,
    MaxDeliveryDistance,
    DeliveryType,
    StartDate,
    DeliveredOnTime,
    DeliveredLate
}


public enum OrderInListFilter{ 
    Type,
    OrderStatus,
    ScheduleStatus
}

public enum OrderInListSort 
{
    OrderId,
    AirDistance,
    TimeRemaining,
    TotalCompletionTime,
    TotalDeliveries
}

public enum OpenOrderInListSort
{
    OrderId,
    Weight,
    AirDistance,
    ActualDistance,
    EstimatedActualTime,
    TimeRemaining,
    MaxDeliveryTime
}

public enum ClosedDeliveryInListSort 
{
    DeliveryId,
    OrderId,
    ActualDistance,
    CompletionTime
}




