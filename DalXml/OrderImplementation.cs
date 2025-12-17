using DalApi;
using DO;
using System.Xml.Linq;
using System;

namespace Dal
{
    /// <summary>
    /// XML-backed implementation of <see cref="IOrder"/> for the DAL.
    /// Persists orders as XML elements and maps between <see cref="XElement"/> and <see cref="DO.Order"/>.
    /// Intended for test/demo scenarios.
    /// </summary>
    internal class OrderImplementation : IOrder
    {
        /// <summary>
        /// Build a DO.Order from an XML element.
        /// Handles missing or malformed values with sensible defaults or exceptions where appropriate.
        /// </summary>
        static Order getOrder(XElement s)
        {
            // Map XML element children to the Order record properties.
            return new DO.Order()
            {
                OrderId = (int?)s.ToIntNullable("OrderId") ?? throw new FormatException("can't convert id"),
                CustomerName = (string?)s.Element("CustomerName") ?? "",
                typeOfOrder = s.ToEnumNullable<TypeOfOrder>("TypeOfOrder") ?? TypeOfOrder.SingleOrder,
                ShippingAddress = (string?)s.Element("ShippingAddress") ?? "",
                Latitude = s.ToDoubleNullable("Latitude") ?? 0.0,
                Longitude = s.ToDoubleNullable("Longitude") ?? 0.0,
                CustomerPhone = (string?)s.Element("CustomerPhone") ?? "",
                weight = s.ToDoubleNullable("Weight") ?? 0.0,
                OrderInitiation = s.ToDateTimeNullable("OrderInitiation") ?? DateTime.MinValue,
                VerbalDescription = (string?)s.Element("VerbalDescription"),
            };
        }
        //throw new FormatException("can't convert id"),
        /// <summary>
        /// Read a single order by id from the orders XML.
        /// Returns null if not found.
        /// </summary>
        public Order? Read(int id)
        {
            // Load XML root and find the element with matching Id (note: code checks "Id" element here).
            XElement? orderElem =
                XMLTools.LoadListFromXMLElement(Config.s_orders_xml).Elements().FirstOrDefault(st => (int?)st.Element("OrderId") == id);
            return orderElem is null ? null : getOrder(orderElem);
        }

        /// <summary>
        /// Create a new order: assign a generated OrderId, append the element and save XML.
        /// </summary>
        public void Create(Order item)
        {
            XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);
            // Assign unique OrderId from Config, create a copy (records are immutable-ish) and persist as XML.
            item = item with { OrderId = Config.NextOrderId };
            ordersRootElem.Add(new XElement(createOrderElement(item)));
            XMLTools.SaveListToXMLElement(ordersRootElem, Config.s_orders_xml);
        }

        /// <summary>
        /// Delete an order by id. Throws DalDoesNotExistException when not found.
        /// </summary>
        public void Delete(int id)
        {
            XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);
            // Find element by OrderId and remove it; throw if missing.
            (ordersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("OrderId") == id)
                ?? throw new DO.DalDoesNotExistException($"Order with ID={id} does Not exist"))
                    .Remove();
            XMLTools.SaveListToXMLElement(ordersRootElem, Config.s_orders_xml);
        }

        /// <summary>
        /// Remove all orders from the XML store.
        /// </summary>
        public void DeleteAll()
        {
            XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);
            ordersRootElem.RemoveAll();
            XMLTools.SaveListToXMLElement(ordersRootElem, Config.s_orders_xml);
        }

        /// <summary>
        /// Read first order matching the provided predicate.
        /// </summary>
        public Order? Read(Func<Order, bool> filter)
        {
            // Enumerate XML elements, map to Order objects and apply filter.
            return XMLTools.LoadListFromXMLElement(Config.s_orders_xml).Elements().Select(s => getOrder(s)).FirstOrDefault(filter);
        }

        /// <summary>
        /// Read all orders optionally filtered by predicate.
        /// Not implemented yet.
        /// </summary>
        public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
        {
            XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);
            IEnumerable<Order> orders = ordersRootElem.Elements().Select(s => getOrder(s));
            return filter is null ? orders : orders.Where(filter);

        }

        /// <summary>
        /// Create an XElement representing an Order for persistence.
        /// Produces child elements for all non-null order properties.
        /// </summary>
        public static XElement createOrderElement(Order order)
        {
            return new XElement("Order",
                new XElement("OrderId", order.OrderId),
                new XElement("TypeOfOrder", order.typeOfOrder.ToString()),
                new XElement("ShippingAddress", order.ShippingAddress),
                new XElement("Latitude", order.Latitude),
                new XElement("Longitude", order.Longitude),
                new XElement("CustomerName", order.CustomerName),
                new XElement("CustomerPhone", order.CustomerPhone),
                new XElement("Weight", order.weight),
                new XElement("OrderInitiation", order.OrderInitiation),
                order.VerbalDescription != null
                    ? new XElement("VerbalDescription", order.VerbalDescription)
                    : null
            );
        }

        /// <summary>
        /// Update an existing order: replace its XML element with the new data.
        /// Throws DalDoesNotExistException if the order does not exist.
        /// </summary>
        public void Update(Order item)
        {
            XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);

            // Find existing element by OrderId; remove it and add updated element.
            (ordersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("OrderId") == item.OrderId)
                ?? throw new DO.DalDoesNotExistException($"Order with ID={item.OrderId} does Not exist"))
                    .Remove();

            ordersRootElem.Add(new XElement(createOrderElement(item)));

            XMLTools.SaveListToXMLElement(ordersRootElem, Config.s_orders_xml);
        }
    }
}
