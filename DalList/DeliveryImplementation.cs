using DalApi;
using DO;
using System.Collections.Generic;
using System;

namespace Dal
{
    public class DeliveryImplementation : IDelivery
    {
        public void Create(Delivery item)
        {
            int newId = Config.NextDeliveryId;
            Delivery copy = item with { Id = newId };
            DataSource.Deliveries.Add(copy);
        }

        public Delivery? Read(int id)
        {
            return DataSource.Deliveries.Find(d => d.Id == id);
        }

        public List<Delivery> ReadAll()
        {
            return new List<Delivery>(DataSource.Deliveries);
        }

        public void Update(Delivery item)
        {
            if (Read(item.Id) is null)
                throw new Exception($"Delivery with ID={item.Id} does NOT exist.");

            DataSource.Deliveries.RemoveAll(d => d.Id == item.Id);
            DataSource.Deliveries.Add(item);
        }

        public void Delete(int id)
        {
            if (Read(id) is null)
                throw new Exception($"Delivery with ID={id} does NOT exist.");

            DataSource.Deliveries.RemoveAll(d => d.Id == id);
        }

        public void DeleteAll()
        {
            DataSource.Deliveries.Clear();
        }
    }
}
