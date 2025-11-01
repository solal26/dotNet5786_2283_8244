
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CourierImplementation : ICourier
{
    public void Create(Courier item)
    {
        // Check if an object with the same ID already exists
        if (Read(item.Id) != null)
            throw new Exception($"Courier with ID={item.Id} already exists");

        // Add to the list
        DataSource.Couriers.Add(item);
    }

    public Courier? Read(int id)
    {
        // Search for the object by ID
        foreach (var c in DataSource.Couriers)
        {
            if (c.Id == id)
                return c;
        }
        // Return null if not found
        return null;
    }

    public List<Courier> ReadAll()
    {
        // Return a copy of the list
        return new List<Courier>(DataSource.Couriers);
    }

    public void Update(Courier item)
    {
        var existing = Read(item.Id);
        if (existing == null)
            throw new Exception($"Courier with ID={item.Id} does not exist");

        // Remove the existing object and add the updated one
        DataSource.Couriers.Remove(existing);
        DataSource.Couriers.Add(item);
    }

    public void Delete(int id)
    {
        var existing = Read(id);
        if (existing == null)
            throw new Exception($"Courier with ID={id} does not exist");

        // Remove the object from the list
        DataSource.Couriers.Remove(existing);
    }

    public void DeleteAll()
    {
        // Clear the entire list
        DataSource.Couriers.Clear();
    }
}
