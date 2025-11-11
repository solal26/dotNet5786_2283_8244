namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// XML-backed DAL implementation for <see cref="Courier"/> entities.
/// Loads the courier list from XML, performs in-memory changes and persists back using <see cref="XMLTools"/> and <see cref="Config.s_couriers_xml"/>.
/// </summary>
/// <remarks>
/// - Internal to the DalXml assembly; not exposed outside the DAL implementation.
/// - Methods operate on the entire stored list: they always load the list, modify it, then save it back.
/// - Exceptions:
///   - <see cref="DalAlreadyExistsException"/> when attempting to create a courier with an existing Id.
///   - <see cref="DalDoesNotExistException"/> when attempting to update/delete a non-existing courier.
/// </remarks>
internal class CourierImplementation : ICourier
{
    /// <summary>
    /// Adds a new courier to storage.
    /// </summary>
    /// <param name="item">Courier to add. If a courier with the same <see cref="Courier.Id"/> exists, an exception is thrown.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown when a courier with the same Id already exists.</exception>
    public void Create(Courier item)
    {
        List<Courier> Couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        if (Couriers.Any(it => it.Id == item.Id))
            throw new DalAlreadyExistsException($"Courier with ID={item.Id} already exists");
        Couriers.Add(item);
        XMLTools.SaveListToXMLSerializer(Couriers, Config.s_couriers_xml);
    }

    /// <summary>
    /// Deletes the courier with the specified id.
    /// </summary>
    /// <param name="id">Identifier of the courier to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when no courier with <paramref name="id"/> exists.</exception>
    public void Delete(int id)
    {
        List<Courier> Couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        if (Couriers.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Courier with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Couriers, Config.s_couriers_xml);
    }

    /// <summary>
    /// Removes all courier records from storage.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Courier>(), Config.s_couriers_xml);
    }

    /// <summary>
    /// Reads a courier by id.
    /// </summary>
    /// <param name="id">Identifier to find.</param>
    /// <returns>The <see cref="Courier"/> with the requested id, or <c>null</c> when not found.</returns>
    public Courier? Read(int id)
    {
        List<Courier> Couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        return Couriers.FirstOrDefault(o => o.Id == id);
    }

    /// <summary>
    /// Reads the first courier matching the provided predicate.
    /// </summary>
    /// <param name="filter">Predicate used to select the courier.</param>
    /// <returns>The first matching <see cref="Courier"/> or <c>null</c> if none match.</returns>
    public Courier? Read(Func<Courier, bool> filter)
    {
        List<Courier> Couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        return Couriers.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all couriers, optionally filtered by <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">Optional predicate to filter returned couriers. If <c>null</c>, all couriers are returned.</param>
    /// <returns>An enumerable of couriers matching the filter (or all couriers when the filter is null).</returns>
    public IEnumerable<Courier> ReadAll(Func<Courier, bool>? filter = null)
    {
        List<Courier> Couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        return (filter == null) ? Couriers.Select(item => item) : Couriers.Where(filter);
    }

    /// <summary>
    /// Updates an existing courier record.
    /// </summary>
    /// <param name="item">Courier containing updated values. The courier is identified by <see cref="Courier.Id"/>.</param>
    /// <exception cref="DalDoesNotExistException">Thrown when the courier to update does not exist.</exception>
    public void Update(Courier item)
    {
        List<Courier> Couriers = XMLTools.LoadListFromXMLSerializer<Courier>(Config.s_couriers_xml);
        if (Couriers.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Courier with ID={item.Id} does Not exist");
        Couriers.Add(item);
        XMLTools.SaveListToXMLSerializer(Couriers, Config.s_couriers_xml);
    }
}

