namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// XML-backed DAL implementation for <see cref="Delivery"/> entities.
/// Loads and saves delivery collections using <see cref="XMLTools"/> and the file path defined in <see cref="Config.s_deliveries_xml"/>.
/// </summary>
/// <remarks>
/// - Internal to the DalXml assembly; not part of the public facade.
/// - Methods operate on the full delivery list: they load the list, modify it in-memory and persist it back to the XML file.
/// - Typical exceptions (implementation-dependent): <see cref="DalAlreadyExistsException"/> for duplicate creates,
///   <see cref="DalDoesNotExistException"/> for updates/deletes of missing items. Current methods follow simple list semantics.
/// </remarks>
internal class DeliveryImplementation : IDelivery
{
    /// <summary>
    /// Creates a new delivery record.
    /// </summary>
    /// <param name="item">Delivery to create. The method assigns a new <see cref="Delivery.Id"/> from <see cref="Config.NextDeliveryId"/>.</param>
    public void Create(Delivery item)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        Delivery copy = item with { Id = Config.NextDeliveryId };
        deliveries.Add(copy);
        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_deliveries_xml);
    }

    /// <summary>
    /// Deletes the delivery with the specified id.
    /// </summary>
    /// <param name="id">Identifier of the delivery to delete.</param>
    public void Delete(int id)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        deliveries.RemoveAll(d => d.Id == id);
        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_deliveries_xml);
    }

    /// <summary>
    /// Removes all delivery records from storage.
    /// </summary>
    public void DeleteAll()
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        deliveries.Clear();
        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_deliveries_xml);
    }

    /// <summary>
    /// Reads a delivery by its identifier.
    /// </summary>
    /// <param name="id">Identifier of the delivery to read.</param>
    /// <returns>The matching <see cref="Delivery"/> or <c>null</c> if not found.</returns>
    public Delivery? Read(int id)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        return deliveries.FirstOrDefault(d => d.Id == id);
    }

    /// <summary>
    /// Reads the first delivery that matches the provided filter.
    /// </summary>
    /// <param name="filter">Predicate used to find the delivery.</param>
    /// <returns>The first matching <see cref="Delivery"/> or <c>null</c> if none match.</returns>
    public Delivery? Read(Func<Delivery, bool> filter)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        return deliveries.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all deliveries, optionally filtered by <paramref name="filter"/>.
    /// </summary>
    /// <param name="filter">Optional predicate to filter returned deliveries. If <c>null</c>, all deliveries are returned.</param>
    /// <returns>An enumerable of deliveries matching the filter (or all deliveries when the filter is null).</returns>
    public IEnumerable<Delivery> ReadAll(Func<Delivery, bool>? filter = null)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        return filter == null ? deliveries : deliveries.Where(filter);
    }

    /// <summary>
    /// Updates an existing delivery record.
    /// </summary>
    /// <param name="item">Delivery containing updated values. The <see cref="Delivery.Id"/> identifies the record to update.</param>
    public void Update(Delivery item)
    {
        List<Delivery> deliveries = XMLTools.LoadListFromXMLSerializer<Delivery>(Config.s_deliveries_xml);
        deliveries.RemoveAll(d => d.Id == item.Id);
        deliveries.Add(item);
        XMLTools.SaveListToXMLSerializer(deliveries, Config.s_deliveries_xml);
    }
}
