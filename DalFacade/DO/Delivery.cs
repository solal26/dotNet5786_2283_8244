namespace DO;

using System;

/// <summary>
/// Represents a single delivery assignment linking an order to a courier,
/// including timing, shipment type and final delivery outcome.
/// </summary>
/// <param name="Id">Unique identifier for the delivery record.</param>
/// <param name="OrderId">Identifier of the associated order.</param>
/// <param name="CourierId">Identifier of the assigned courier.</param>
/// <param name="typeofshipment">Preferred type of shipment/vehicle used for this delivery.</param>
/// <param name="TimeBegin">Start time of the delivery (dispatch time).</param>
/// <param name="endcour">Optional delivery outcome (<see cref="Endofcourier"/>).</param>
/// <param name="TimeEnd">Optional end time of the delivery (completion or failure time).</param>
/// <param name="Distance">Optional distance traveled for this delivery in kilometers.</param>
public record Delivery
    (
    int Id,
    int OrderId,
    int CourierId,
    TypeOfShipment typeofshipment,//shiluah
    DateTime TimeBegin,
    Endofcourier? endcour = null,
    DateTime? TimeEnd = null,
    double? Distance = null 
    )
{ 
    /// <summary>
    /// Initializes a default <see cref="Delivery"/> instance with conservative defaults.
    /// </summary>
    public Delivery() : this(0, 0, 0, TypeOfShipment.Car, DateTime.MinValue) { }    
}

