using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System;



namespace Dal
{
    public class OrderImplementation : IOrder
    {
        public void Create(Order item)
        {
            int newId = Config.NextOrderId;
            Order copy = item with { OrderId = newId };
            DataSource.Orders.Add(copy);
        }

        public Order? Read(int id)
        {
            return DataSource.Orders.Find(o => o.OrderId == id)!;
        }

        public List<Order> ReadAll()
        {
            return new List<Order>(DataSource.Orders);
        }

        public void Update(Order item)
        {
            if (Read(item.OrderId) is null)
                throw new Exception($"Order with ID={item.OrderId} does NOT exist (cannot update).");

            DataSource.Orders.RemoveAll(o => o.OrderId == item.OrderId);
            DataSource.Orders.Add(item);
        }

        public void Delete(int id)
        {
            if (Read(id) is null)
                throw new Exception($"Order with ID={id} does NOT exist (cannot delete).");

            DataSource.Orders.RemoveAll(o => o.OrderId == id);
        }

        public void DeleteAll()
        {
            DataSource.Orders.Clear();
        }
    }
}
