namespace DalApi;

public interface IDal
{

    ICourier Courier { get; }
    IOrder Order { get; }       
    IConfig Config { get; }
    IDelivery Delivery { get; }
    void ResetDB();


}
