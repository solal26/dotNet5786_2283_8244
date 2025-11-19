namespace BlImplementation;
using BO;
using BlApi;


internal class CourierImplementation : ICourier
{
    public void AddCourier(int requesterId, Courier courier)
    {
        throw new NotImplementedException();
    }

    public void DeleteCourier(int requesterId, int courierId)
    {
        throw new NotImplementedException();
    }

    public Courier GetCourier(int requesterId, int courierId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<CourierInList> GetList(string requesterId, bool? isActive = null, CourierSortBy? sortBy = null)
    {
        throw new NotImplementedException();
    }

    public string Login(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void UpdateCourier(int requesterId, Courier courier)
    {
        throw new NotImplementedException();
    }
}
