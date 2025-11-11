
using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal;

internal class OrderImplementation : IOrder
{

    static Order getOrder(XElement s)
    {
        return new DO.Order()
        {
            OrderId = s.ToIntNullable("OrderId") ?? throw new FormatException("can't convert id"),
            CustomerName = (string?)s.Element("CustomerName") ?? "",
            typeOfOrder = s.ToEnumNullable<TypeOfOrder>("typeOfOrder")??TypeOfOrder.SingleOrder,
            ShippingAddress= (string?)s.Element("ShippingAddress") ?? "",
            Latitude = s.ToDoubleNullable("Latitude") ?? 0.0,   
            Longitude = s.ToDoubleNullable("Longitude") ?? 0.0,
            CustomerPhone = (string?)s.Element("CustomerPhone") ?? "",
            weight = s.ToDoubleNullable("weight") ?? 0.0,
            OrderInitiation = s.ToDateTimeNullable("OrderInitiation") ?? DateTime.MinValue,
            VerbalDescription = (string?)s.Element("VerbalDescription"),
        };


    }

    public Order? Read(int id)
    {
        XElement? orderElem =
    XMLTools.LoadListFromXMLElement(Config.s_orders_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return orderElem is null ? null : getOrder(orderElem);
    }



    public void Create(Order item)
    {
        XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);
        item = item with { OrderId = Config.NextOrderId };
        ordersRootElem.Add(new XElement("Order", createOrderElement(item)));
        XMLTools.SaveListToXMLElement(ordersRootElem, Config.s_orders_xml);
    }

    public void Delete(int id)
    {
        XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);
        (ordersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("OrderId") == id)
        ?? throw new DO.DalDoesNotExistException($"Order with ID={id} does Not exist"))
                .Remove();
        XMLTools.SaveListToXMLElement(ordersRootElem, Config.s_orders_xml);

    }

    public void DeleteAll()
    {
        XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);
        ordersRootElem.RemoveAll();
        XMLTools.SaveListToXMLElement(ordersRootElem, Config.s_orders_xml);
    }

   

    public Order? Read(Func<Order, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_orders_xml).Elements().Select(s => getOrder(s)).FirstOrDefault(filter);
    }

    public IEnumerable<Order> ReadAll(Func<Order, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

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

    public void Update(Order item)
    {
        XElement ordersRootElem = XMLTools.LoadListFromXMLElement(Config.s_orders_xml);

        (ordersRootElem.Elements().FirstOrDefault(st => (int?)st.Element("OrderId") == item.OrderId)
        ?? throw new DO.DalDoesNotExistException($"Order with ID={item.OrderId} does Not exist"))
                .Remove();

        ordersRootElem.Add(new XElement("Order", createOrderElement(item)));

        XMLTools.SaveListToXMLElement(ordersRootElem, Config.s_orders_xml);

    }
}
