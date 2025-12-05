
using DalApi;
using DO;

namespace Helpers;

internal static  class DeliveryManager
{
    private static IDal s_dal = Factory.Get;

    internal static ObserverManager Observers = new();

}
