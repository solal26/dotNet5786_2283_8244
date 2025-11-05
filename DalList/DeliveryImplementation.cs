using System;
using System.Collections.Generic;
using System.Linq;
using DalApi;
using DO;

namespace Dal
{
    // Minimal in-memory CRUD implementation for Delivery entities.
    // Operates on the shared `DataSource.Deliveries` list and is intended for demos/tests.
    internal class DeliveryImplementation : IDelivery
    {
        // Create: assign a new delivery id and add a copy to the data store.
        public void Create(Delivery item)
        {
            int newId = Config.NextDeliveryId;
            Delivery copy = item with { Id = newId };
            DataSource.Deliveries.Add(copy);
        }

        // Read: return the delivery with the given id or null if not found.
        public Delivery? Read(int id)
        {
            return DataSource.Deliveries.FirstOrDefault(d => d.Id == id);
        }

        // ReadAll: return a snapshot copy of all deliveries.
        public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null) => filter == null
      ? DataSource.Deliveries.Select(item => item)
         : DataSource.Deliveries.Where(filter);

        // Update: replace an existing delivery matched by id; throws if not present.
        public void Update(Delivery item)
        {
            if (Read(item.Id) is null)
                throw new DalDoesNotExistException($"Delivery with ID={item.Id} does NOT exist.");

            DataSource.Deliveries.RemoveAll(d => d.Id == item.Id);
            DataSource.Deliveries.Add(item);
        }

        // Delete: remove a delivery by id; throws if not present.
        public void Delete(int id)
        {
            if (Read(id) is null)
                throw new DalDoesNotExistException($"Delivery with ID={id} does NOT exist.");

            DataSource.Deliveries.RemoveAll(d => d.Id == id);
        }

        // DeleteAll: clear all delivery records (used for reset/testing).
        public void DeleteAll()
        {
            DataSource.Deliveries.Clear();
        }

        public Delivery? Read(Func<Delivery, bool> filter) => DataSource.Deliveries.FirstOrDefault(filter);
      
    }
}
