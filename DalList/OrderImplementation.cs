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
            Order copy = item with { Id = newId };
            DataSource.Orders.Add(copy);
        }

        public Order? Read(int id)
        {
            return DataSource.Orders.Find(o => o.Id == id)!;
        }

        public List<Order> ReadAll()
        {
            return new List<Order>(DataSource.Orders);
        }

        public void Update(Order item)
        {
            if (Read(item.Id) is null)
                throw new Exception($"Order with ID={item.Id} does NOT exist (cannot update).");

            DataSource.Orders.RemoveAll(o => o.Id == item.Id);
            DataSource.Orders.Add(item);
        }

        public void Delete(int id)
        {
            if (Read(id) is null)
                throw new Exception($"Order with ID={id} does NOT exist (cannot delete).");

            DataSource.Orders.RemoveAll(o => o.Id == id);
        }

        public void DeleteAll()
        {
            DataSource.Orders.Clear();
        }
    }
}
