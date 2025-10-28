namespace DO;

public record Delivery
    (
    int Id,
    int OrderId,
    int CourierId,
    TypeOfShipment typeofshipment,//shiluah
    DateTime TimeBegin,
    endofcourier? endcour=null,
    DateTime? TimeEnd = null,
    double? Distance = null 
    )
{ 
}

