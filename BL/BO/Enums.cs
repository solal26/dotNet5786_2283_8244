namespace BO;

/// <summary>
/// Classification of an order's business type.
/// Use to drive routing, pricing and assignment rules.
/// </summary>
public enum OrderType
{
    /// <summary>Single customer order (typical one-recipient delivery).</summary>
    SingleOrder,
    /// <summary>Bulk order containing multiple items or deliveries (may affect batching).</summary>
    BulkOrder,
    /// <summary>High-priority / expedited order that should be delivered faster.</summary>
    ExpressOrder,
    /// <summary>Internal transfer not intended for an external customer.</summary>
    InternalTransfer
}

/// <summary>
/// Logical lifecycle status of an order as determined by BL using related delivery records.
/// </summary>
public enum OrderStatus
{
    /// <summary>Order created and not yet assigned to an active delivery.</summary>
    Open,
    /// <summary>Order currently being delivered (there is an active delivery attempt).</summary>
    InProgress,
    /// <summary>Order successfully delivered to the recipient.</summary>
    Delivered,
    /// <summary>Delivery attempt was refused by the recipient.</summary>
    Refused,
    /// <summary>Order was cancelled and will not be delivered.</summary>
    Cancelled
}

/// <summary>
/// Schedule status relative to the configured SLA (MaxDeliveryTime and RiskRange).
/// Used to indicate urgency for the UI and prioritization logic.
/// </summary>
public enum ScheduleStatus
{
    /// <summary>Delivery is comfortably within SLA.</summary>
    OnTime,
    /// <summary>Delivery is within the risk window (close to SLA threshold).</summary>
    InRisk,
    /// <summary>Delivery is past the allowed time window (late).</summary>
    Late
}

/// <summary>
/// Transport mode used by a courier / delivery attempt.
/// Affects speed and distance calculations.
/// </summary>
public enum DeliveryType
{
    /// <summary>Car transport.</summary>
    Car,
    /// <summary>Scooter / motorbike transport.</summary>
    Scooter,
    /// <summary>Bicycle transport.</summary>
    Bike,
    /// <summary>On-foot (walking) delivery.</summary>
    OnFoot,
    /// <summary>All transport types (used for filtering).</summary>
    All
}


/// <summary>
/// Final outcome of a delivery attempt.
/// </summary>
public enum DeliveryEndType
{
    /// <summary>Package delivered to recipient.</summary>
    Delivered,
    /// <summary>Recipient refused the package.</summary>
    Refused,
    /// <summary>Delivery was cancelled.</summary>
    Cancelled,
    /// <summary>Recipient was not present at the address.</summary>
    NotHere,
    /// <summary>Delivery failed for other reasons (vehicle breakdown, error, etc.).</summary>
    Failed
}

/// <summary>
/// Time units accepted by administrative actions that advance the system clock.
/// </summary>
public enum TimeUnit
{
    /// <summary>Minute unit.</summary>
    Minute,
    /// <summary>Hour unit.</summary>
    Hour,
    /// <summary>Day unit.</summary>
    Day,
    /// <summary>Month unit.</summary>
    Month,
    /// <summary>Year unit.</summary>
    Year
}

/// <summary>
/// Available sort key options when listing couriers.
/// </summary>
public enum CourierSortBy
{
    /// <summary>Sort by courier identifier.</summary>
    Id,
    /// <summary>Sort by courier full name.</summary>
    FullName,
    /// <summary>Sort by active status.</summary>
    Active,
    /// <summary>Sort by maximum delivery distance.</summary>
    MaxDeliveryDistance,
    /// <summary>Sort by delivery transport type.</summary>
    DeliveryType,
    /// <summary>Sort by courier start date (tenure).</summary>
    StartDate,
    /// <summary>Sort by number of deliveries delivered on time.</summary>
    DeliveredOnTime,
    /// <summary>Sort by number of deliveries delivered late.</summary>
    DeliveredLate
}

/// <summary>
/// Fields that can be used to filter orders in list views.
/// </summary>
public enum OrderInListFilter
{
    /// <summary>Filter by order type.</summary>
    Type,
    /// <summary>Filter by order lifecycle status.</summary>
    OrderStatus,
    /// <summary>Filter by schedule/SLA status.</summary>
    ScheduleStatus
}

/// <summary>
/// Sort keys for order list projections.
/// </summary>
public enum OrderInListSort
{
    /// <summary>Sort by order identifier.</summary>
    OrderId,
    /// <summary>Sort by straight-line distance (air distance).</summary>
    AirDistance,
    /// <summary>Sort by time remaining until SLA deadline.</summary>
    TimeRemaining,
    /// <summary>Sort by total completion time for the order.</summary>
    TotalCompletionTime,
    /// <summary>Sort by number of delivery attempts.</summary>
    TotalDeliveries
}

/// <summary>
/// Sort keys for open-order lists shown to couriers.
/// </summary>
public enum OpenOrderInListSort
{
    /// <summary>Sort by order identifier.</summary>
    OrderId,
    /// <summary>Sort by shipment weight.</summary>
    Weight,
    /// <summary>Sort by straight-line (air) distance.</summary>
    AirDistance,
    /// <summary>Sort by estimated or measured actual route distance.</summary>
    ActualDistance,
    /// <summary>Sort by estimated travel time for the courier.</summary>
    EstimatedActualTime,
    /// <summary>Sort by remaining time until SLA deadline.</summary>
    TimeRemaining,
    /// <summary>Sort by maximum allowed delivery DateTime.</summary>
    MaxDeliveryTime
}

/// <summary>
/// Sort keys for closed-deliveries list projections.
/// </summary>
public enum ClosedDeliveryInListSort
{
    /// <summary>Sort by delivery identifier.</summary>
    DeliveryId,
    /// <summary>Sort by associated order identifier.</summary>
    OrderId,
    /// <summary>Sort by actual distance travelled.</summary>
    ActualDistance,
    /// <summary>Sort by completion time (duration).</summary>
    CompletionTime
}




