
using DalApi;
using DO;

namespace Dal;

internal class OrderImplementation : IOrder
{
    public void Create(Order item)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Order? Read(int id)
    {
        throw new NotImplementedException();
    }

    public Order? Read(Func<Order, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Order item)
    {
        throw new NotImplementedException();
    }
}
