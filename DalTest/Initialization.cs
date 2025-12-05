namespace DalTest;
using DalApi;
using DO;

/// <summary>
/// Test-data initializer that populates the DAL with configuration, couriers, orders and deliveries.
/// </summary>
/// <remarks>
/// - Intended for testing, demos and local initialization only (not production). 
/// - Uses a pseudo-random generator to create varied sample data; results will differ between runs.
/// - Courier Ids are generated to avoid collisions by checking the DAL.
/// - Distances are computed with the Haversine formula using the embedded sample addresses around Israel.
/// </remarks>
public static class Initialization
{
    private static IDal? s_dal;
    private static readonly Random s_rand = new();

    private static readonly (string Address, double Lat, double Lon) companyAddress =
        ("Tel Aviv, Israel", 32.08088, 34.78902);
   

    private static readonly (string Address, double Lat, double Lon)[] addresses =
    {
        
        ("14 Rothschild Blvd, Tel Aviv,Israel", 32.06672, 34.77403),
        ("59 Ibn Gabirol St, Tel Aviv,Israel", 32.07206, 34.78138),
        ("34 David HaMelech St, Tel Aviv,Israel", 32.06999, 34.77224),
        ("45 King George St, Jerusalem,Israel", 31.77196, 35.21766),
        ("28 Jaffa St, Jerusalem,Israel", 31.76832, 35.21371),
        ("15 Ben Gurion Blvd, Haifa,Israel", 32.81669, 34.98957),
        ("10 HaAtzmaut St, Haifa,Israel", 32.81421, 34.98912),
        ("22 Herzl St, Beersheba,Israel", 31.25179, 34.79146),
        ("9 Jerusalem Blvd, Beersheba,Israel", 31.25693, 34.79332)
    };

    /// <summary>
    /// Configure default application settings in the provided <see cref="IConfig"/> instance.
    /// </summary>
    /// <remarks>
    /// - Sets manager credentials, company address, speed/range defaults and clock.
    /// - Intended as sensible defaults for the demo dataset.
    /// </remarks>
    private static void createConfig()
    {
        s_dal!.Config.ManagerId = 123456789;
        s_dal!.Config.ManagerPassword = "admin123";
        s_dal!.Config.CompanyAddress = companyAddress.Address;
        s_dal!.Config.Latitude = companyAddress.Lat;
        s_dal!.Config.Longitude = companyAddress.Lon;
        s_dal!.Config.MaxRange = 30;
        s_dal!.Config.AvgCarSpeed = 60.0;
        s_dal!.Config.AvgMotorBikeSpeed = 45.0;
        s_dal!.Config.AvgBicycleSpeed = 15.0;
        s_dal!.Config.AvgWalkingSpeed = 5.0;
        s_dal!.Config.MaxDeliveryTime = TimeSpan.FromHours(3);
        s_dal!.Config.RiskRange = TimeSpan.FromHours(2);
        s_dal!.Config.InactivityRange = TimeSpan.FromDays(1);
        s_dal!.Config.Clock = DateTime.Now;
    }

    /// <summary>
    /// Create a set of sample couriers and add them to the DAL.
    /// </summary>
    /// <remarks>
    /// - Generates randomized but realistic contact details and availability.
    /// - Ensures generated courier Ids do not collide with existing DAL entries by calling <see cref="ICourier.Read(int)"/>.
    /// - Distance values represent an approximate personal maximum travel distance (kilometers).
    /// </remarks>
    private static void createCouriers()
    {
        string[] names = { "David", "Sarah", "Michael", "Leah", "Yossi", "Rina", "Eli", "Tamar", "Noam", "Shira",
                           "Daniel", "Maya", "Itay", "Aya", "Jonathan", "Hadas", "Shimon", "Nava", "Oren", "Yael" };
        TypeOfShipment[] shipmentTypes = { TypeOfShipment.Car, TypeOfShipment.Bike, TypeOfShipment.Scooter, TypeOfShipment.OnFoot };

        for (int i = 0; i < 20; i++)
        {
            string name = names[i];
            string phone = $"05{s_rand.Next(0, 10)}{s_rand.Next(1000000, 9999999)}";
            string email = $"{name.ToLower()}{s_rand.Next(1, 10000)}@gmail.com";
            string code = s_rand.Next(1000, 10000).ToString();
            TypeOfShipment type = shipmentTypes[s_rand.Next(shipmentTypes.Length)];
            bool active = s_rand.NextDouble() > 0.2; 
            double? maxDistance = type switch
            {
                TypeOfShipment.Car => s_rand.Next(20, 50),
                TypeOfShipment.Scooter => s_rand.Next(15, 40),
                TypeOfShipment.Bike => s_rand.Next(5, 20),
                TypeOfShipment.OnFoot => s_rand.Next(1, 10),
                _ => 10
            };
            DateTime startTime = s_dal!.Config.Clock.AddMinutes(-s_rand.Next(0, 120));

            int id;
            do { id = s_rand.Next(200000000, 400000000); }
            while (s_dal!.Courier.Read(id) != null);

            s_dal!.Courier.Create(new Courier(
                Id: id,
                Name: name,
                PhoneNumber: phone,
                Email: email,
                Code: code,
                Active: active,
                TimeBegin: startTime,
                typeofshipment: type,
                Distance: maxDistance
            ));
        }
    }

    /// <summary>
    /// Create a set of sample orders and add them to the DAL.
    /// </summary>
    /// <remarks>
    /// - Randomizes customer, address, weight and optional verbal description.
    /// - Order timestamps are set relative to <see cref="IConfig.Clock"/>.
    /// </remarks>
    private static void createOrders()
    {
        string[] customerNames = { "David Cohen", "Sarah Levy", "Michael Gold", "Leah Stern", "Yossi Azulay",
                                   "Rina Ben", "Eli Horowitz", "Tamar Shalev", "Noam Katz", "Shira Klein" };
        TypeOfOrder[] orderTypes = { TypeOfOrder.SingleOrder, TypeOfOrder.BulkOrder };

        int totalOrders = 50;

        for (int i = 0; i < totalOrders; i++)
        {
            string customerName = customerNames[s_rand.Next(customerNames.Length)];
            string customerPhone = $"05{s_rand.Next(0, 10)}{s_rand.Next(1000000, 9999999)}";

            var addr = addresses[s_rand.Next(addresses.Length)];
            string address = addr.Address;
            double lat = addr.Lat;
            double lon = addr.Lon;

            TypeOfOrder type = orderTypes[s_rand.Next(orderTypes.Length)];
            double weight = Math.Round(s_rand.NextDouble() * 20 + 1, 2);
            DateTime orderTime = s_dal!.Config.Clock.AddMinutes(-s_rand.Next(0, 720));
            string? verbalDesc = s_rand.NextDouble() > 0.5 ? "Fragile, handle with care" : null;

            s_dal!.Order.Create(new Order(
                OrderId: 0,
                typeOfOrder: type,
                ShippingAddress: address,
                Latitude: lat,
                Longitude: lon,
                CustomerName: customerName,
                CustomerPhone: customerPhone,
                weight: weight,
                OrderInitiation: orderTime,
                VerbalDescription: verbalDesc
            ));
        }
    }

    /// <summary>
    /// Match available orders to couriers and create delivery records.
    /// </summary>
    /// <remarks>
    /// - Chooses the first active courier whose configured distance can reach the order location.
    /// - Randomly sets delivery completion and status for some deliveries to simulate finished and in-progress deliveries.
    /// - Removes orders from the available pool when they are considered assigned or finished.
    /// </remarks>
    //private static void createDeliveries()
    //{

    //    List<Order> allOrders = s_dal!.Order.ReadAll().ToList();
    //    List<Courier> allCouriers = s_dal!.Courier.ReadAll().ToList();
    //    List<Order> availableOrders = new List<Order>(allOrders); 

    //    foreach (Order order in allOrders)
    //    {

    //        if (!availableOrders.Contains(order))
    //            continue;

    //        Courier? chosenCourier = null;
    //        double distance = 0;

    //        foreach (Courier courier in allCouriers)
    //        {
    //            distance = CalculateDistance(courier, order);


    //            if (courier.Active && distance <= (courier.Distance ?? 10))
    //            {
    //                chosenCourier = courier;
    //                break; 
    //            }
    //        }


    //        if (chosenCourier == null)
    //            continue;

    //        DateTime startDelivery = order.OrderInitiation.AddMinutes(s_rand.Next(5, 20));
    //        DateTime? endDelivery = null;
    //        Endofcourier? endStatus = null;

    //        bool isFinished = s_rand.NextDouble() > 0.5;
    //        if (isFinished)
    //        {
    //            endDelivery = startDelivery.AddMinutes(s_rand.Next(10, 90));
    //            Array values = Enum.GetValues(typeof(Endofcourier));
    //            endStatus = (Endofcourier)values.GetValue(s_rand.Next(values.Length))!;
    //        }


    //        Delivery newDelivery = new Delivery(
    //            Id: 0,
    //            OrderId: order.OrderId,
    //            CourierId: chosenCourier.Id,
    //            typeofshipment: chosenCourier.typeofshipment,
    //            TimeBegin: startDelivery,
    //            endcour: endStatus,
    //            TimeEnd: endDelivery,
    //            Distance: distance
    //        );

    //        s_dal!.Delivery.Create(newDelivery);


    //        if (isFinished || s_rand.NextDouble() > 0.5)
    //            availableOrders.Remove(order);
    //    }

    //}
    private static void createDeliveries()
    {
        List<Order> allOrders = s_dal!.Order.ReadAll().ToList();
        List<Courier> allCouriers = s_dal!.Courier.ReadAll().ToList();

        double maxCompanyDistance = s_dal!.Config.MaxRange;

        foreach (Order order in allOrders)
        {
            Courier? chosenCourier = null;
            double distance = 0;

            foreach (Courier courier in allCouriers)
            {
                
                bool hasActiveDelivery = s_dal.Delivery.ReadAll()
                                            .Any(d => d.CourierId == courier.Id && d.TimeEnd == null);

                distance = CalculateDistance(courier, order);

              
                bool withinDistance = distance <= (courier.Distance ?? maxCompanyDistance) &&
                                      distance <= maxCompanyDistance;

                if (courier.Active && !hasActiveDelivery && withinDistance)
                {
                    chosenCourier = courier;
                    break; 
                }
            }

            if (chosenCourier == null)
                continue; 

            DateTime startDelivery = order.OrderInitiation.AddMinutes(s_rand.Next(5, 20));
            DateTime? endDelivery = null;
            Endofcourier? endStatus = null;

            bool isFinished = s_rand.NextDouble() > 0.5;
            if (isFinished)
            {
                endDelivery = startDelivery.AddMinutes(s_rand.Next(10, 90));
                Array values = Enum.GetValues(typeof(Endofcourier));
                endStatus = (Endofcourier)values.GetValue(s_rand.Next(values.Length))!;
            }

            Delivery newDelivery = new Delivery(
                Id: 0,
                OrderId: order.OrderId,
                CourierId: chosenCourier.Id,
                typeofshipment: chosenCourier.typeofshipment,
                TimeBegin: startDelivery,
                endcour: endStatus,
                TimeEnd: endDelivery,
                Distance: distance
            );

            s_dal!.Delivery.Create(newDelivery);
        }

    }

    /// <summary>
    /// Calculate the great-circle distance (in kilometers) between the company and the order using the Haversine formula.
    /// </summary>
    /// <param name="c">The courier (used only for context in this implementation).</param>
    /// <param name="o">The order location.</param>
    /// <returns>Approximate distance in kilometers.</returns>
    private static double CalculateDistance(Courier c, Order o)
    {
        double R = 6371;
        double dLat = (o.Latitude - companyAddress.Lat) * Math.PI / 180;
        double dLon = (o.Longitude - companyAddress.Lon) * Math.PI / 180;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(companyAddress.Lat * Math.PI / 180) * Math.Cos(o.Latitude * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double cAngle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * cAngle;
    }

    /// <summary>
    /// Entry point to initialize the DAL. Resets storage and populates demo data.
    /// </summary>
    /// <param name="dalCourier">ICourier implementation used to persist couriers (required).</param>
    /// <param name="dalOrder">IOrder implementation used to persist orders (required).</param>
    /// <param name="dalDelivery">IDelivery implementation used to persist deliveries (required).</param>
    /// <param name="dalConfig">IConfig implementation used to persist configuration (required).</param>
    /// <exception cref="NullReferenceException">Thrown when any provided DAL interface is null.</exception>
    public static void Do()
    {
        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!");
        s_dal = DalApi.Factory.Get;
        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();

        Console.WriteLine("Initializing system data ...");
        createConfig();
        createCouriers();
        createOrders();
        createDeliveries();
    }
}
