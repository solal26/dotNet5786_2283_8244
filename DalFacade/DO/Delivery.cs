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
    public Delivery() : this(0, 0, 0, TypeOfShipment.Car, DateTime.MinValue) { }    
}

