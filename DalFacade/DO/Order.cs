
namespace DO;

public record Order
    (
    int OrderId,
    TypeOfOrder typeOfOrder,
    string ShippingAddress,
    double Latitude,
    double Longitude,
    string CustomerName,
    string CustomerPhone,
    double weight,
    DateTime OrderInitiation,
    string? VerbalDescription=null
    )
{
    public Order(): this(0, TypeOfOrder.SingleOrder, "", 0.0, 0.0, "", "", 0.0, DateTime.MinValue) { }
    
}











