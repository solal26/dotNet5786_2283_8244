namespace DO;

public record Order
    (
    int Id,
    TypeOfOrder typeOfOrder,
    string ShippingAddress,
    double Latitude,
    double Longitude,
    string CustomerName,
    string CustomerPhone,
    double weight,
    DateTime OrderInitiation,
    string? VerbalDescription=null;
    )
{
    public Order(): this(0, TypeOfOrder.Regular, "", 0.0, 0.0, "", "", 0.0, DateTime.MinValue)
    {
    }

}









