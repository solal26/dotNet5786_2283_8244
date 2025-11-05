using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System;



namespace Dal
{
    // Simple in-memory CRUD repository for Order entities.
    // Operates on DataSource.Orders and used for demo/testing.
    internal class OrderImplementation : IOrder
    {
        // Create: assign a generated OrderId and add a copy to the store.
        public void Create(Order item)
        {
            int newId = Config.NextOrderId;                    // get unique id
            Order copy = item with { OrderId = newId };       // create copy with new id
            DataSource.Orders.Add(copy);
        }

        // Read: find an order by its OrderId or return null.
        public Order? Read(int id)
        {
            return DataSource.Orders.FirstOrDefault(o => o.OrderId == id)!;
        }

        // ReadAll: return a snapshot copy of all orders.
        public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null) => filter == null
         ? DataSource.Orders.Select(item => item)
            : DataSource.Orders.Where(filter);

        // Update: replace an existing order matched by OrderId; throw if not found.
        public void Update(Order item)
        {
            if (Read(item.OrderId) is null)
                throw new Exception($"Order with ID={item.OrderId} does NOT exist (cannot update).");

            DataSource.Orders.RemoveAll(o => o.OrderId == item.OrderId);
            DataSource.Orders.Add(item);
        }

        // Delete: remove an order by OrderId; throw if not found.
        public void Delete(int id)
        {
            if (Read(id) is null)
                throw new Exception($"Order with ID={id} does NOT exist (cannot delete).");

            DataSource.Orders.RemoveAll(o => o.OrderId == id);
        }

        // DeleteAll: clear all orders from the in-memory store.
        public void DeleteAll()
        {
            DataSource.Orders.Clear();
        }

        public Order? Read(Func<Order, bool> filter) => DataSource.Orders.FirstOrDefault(filter);

    }
}
