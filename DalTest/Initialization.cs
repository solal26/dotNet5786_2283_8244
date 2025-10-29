namespace DalTest;
using DalApi;
using DO;

public static class Initialization
{
    private static ICourier? s_dalCourier; //stage 1
    private static IOrder? s_dalOrder; //stage 1
    private static IDelivery? s_dalDelivery; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();

    private static void createCouriers() { }//complete
    private static void createOrders() { }//complete
    private static void createDeliveries() { }//complete
    private static void createConfig() { }//complete

    public static void Do(ICourier? dalCourier, IOrder? dalOrder, IDelivery? dalDelivery, IConfig? dalConfig) 
    {
        s_dalCourier = dalCourier ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalOrder = dalOrder ?? throw new NullReferenceException("DAL object can not be null!"); 
        s_dalDelivery = dalDelivery ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset(); 
        s_dalCourier.DeleteAll();
        s_dalOrder.DeleteAll();
        s_dalDelivery.DeleteAll();

        Console.WriteLine("Initializing Students list ...");
        createCouriers();
        createOrders();
        createDeliveries();
        createConfig();
    }

}
