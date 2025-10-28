namespace DO;


public record Courier
    (
    int Id,
    string Name,
    string PhoneNumber,
    string Email,
    string Code,
    bool Active,
    DateTime TimeBegin,

    TypeOfShipment typeofshipment,//shiluah
    double? Distance = null

    )
{
    public Courier() : this(0, "", "", "", "", true, DateTime.MinValue, TypeOfShipment.Car) { }
}




