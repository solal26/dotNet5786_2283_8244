
using DalApi;

namespace Dal;

sealed public class DalXml : IDal
{
    public ICourier Courier { get; } = new CourierImplementation();
    public IOrder Order { get; } = new OrderImplementation();
    public IDelivery Delivery { get; } = new DeliveryImplementation();
    public IConfig Config { get; } = new ConfigImplementation();


    void IDal.ResetDB()
    {
        Config.Reset();
        Courier.DeleteAll();
        Order.DeleteAll();
        Delivery.DeleteAll();
    }
}
