namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

// Simple in-memory CRUD repository for Courier entities.
// Operates on the shared DataSource.Couriers list and is intended for demo/tests.
internal class CourierImplementation : ICourier
{
    // Create: add a new courier if the id is not already present.
    public void Create(Courier item)
    {
        // Check if an object with the same ID already exists
        if (Read(item.Id) != null)
            throw new Exception($"Courier with ID={item.Id} already exists");

        // Add to the list
        DataSource.Couriers.Add(item);
    }

    // Read: return the courier with the given id or null if not found.
    public Courier? Read(int id)
    {
        // Search for the object by ID
        return DataSource.Couriers.FirstOrDefault(o => o.Id == id)!;
    }

    // ReadAll: return a snapshot copy of all couriers (prevents external mutation).
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null) => filter == null
         ? DataSource.Couriers.Select(item => item)
            :DataSource.Couriers.Where(filter);  

    // Update: replace an existing courier matched by id; throws if not found.
    public void Update(Courier item)
    {
        var existing = Read(item.Id);
        if (existing == null)
            throw new Exception($"Courier with ID={item.Id} does not exist");

        // Remove the existing object and add the updated one
        DataSource.Couriers.Remove(existing);
        DataSource.Couriers.Add(item);
    }

    // Delete: remove a courier by id; throws if the courier does not exist.
    public void Delete(int id)
    {
        var existing = Read(id);
        if (existing == null)
            throw new Exception($"Courier with ID={id} does not exist");

        // Remove the object from the list
        DataSource.Couriers.Remove(existing);
    }

    // DeleteAll: clear all courier records (used for reset/testing).
    public void DeleteAll()
    {
        // Clear the entire list
        DataSource.Couriers.Clear();
    }

    public Courier? Read(Func<Courier, bool> filter)=> DataSource.Couriers.FirstOrDefault(filter);
   
}
