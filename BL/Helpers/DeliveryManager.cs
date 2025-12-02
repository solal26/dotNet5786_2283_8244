
using DalApi;

namespace Helpers;

internal static  class DeliveryManager
{
    private static IDal s_dal = Factory.Get;

    internal static ObserverManager Observers = new(); //stage 5 
                                                       //...

}
