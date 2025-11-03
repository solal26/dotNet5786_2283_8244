namespace DalTest;
using DalApi;
using DO;

public static class Initialization
{
    private static ICourier? s_dalCourier;
    private static IOrder? s_dalOrder;
    private static IDelivery? s_dalDelivery;
    private static IConfig? s_dalConfig;
    private static readonly Random s_rand = new();

    private static readonly (string Address, double Lat, double Lon) companyAddress =
        ("Tel Aviv, Israel", 32.08088, 34.78902);

    
    private static readonly (string Address, double Lat, double Lon)[] addresses =
    {
        ("132 Begin Road, Tel Aviv", 32.08088, 34.78902),
        ("14 Rothschild Blvd, Tel Aviv", 32.06672, 34.77403),
        ("59 Ibn Gabirol St, Tel Aviv", 32.07206, 34.78138),
        ("34 David HaMelech St, Tel Aviv", 32.06999, 34.77224),
        ("45 King George St, Jerusalem", 31.77196, 35.21766),
        ("28 Jaffa St, Jerusalem", 31.76832, 35.21371),
        ("15 Ben Gurion Blvd, Haifa", 32.81669, 34.98957),
        ("10 HaAtzmaut St, Haifa", 32.81421, 34.98912),
        ("22 Herzl St, Beersheba", 31.25179, 34.79146),
        ("9 Jerusalem Blvd, Beersheba", 31.25693, 34.79332)
    };

    private static void createConfig()
    {
        s_dalConfig!.ManagerId = 123456789;
        s_dalConfig.ManagerPassword = "admin123";
        s_dalConfig.CompanyAddress = companyAddress.Address;
        s_dalConfig.MaxRange = 30; 
        s_dalConfig.AvgCarSpeed = 60.0;
        s_dalConfig.AvgMotorbikeSpeed = 45.0;
        s_dalConfig.AvgBicycleSpeed = 15.0;
        s_dalConfig.AvgWalkingSpeed = 5.0;
        s_dalConfig.MaxDeliveryTime = TimeSpan.FromHours(3);
        s_dalConfig.RiskRange = TimeSpan.FromMinutes(30);
        s_dalConfig.InactivityRange = TimeSpan.FromMinutes(15);
        s_dalConfig.Clock = DateTime.Now;
    }

    private static void createCouriers()
    {
        string[] names = { "David", "Sarah", "Michael", "Leah", "Yossi", "Rina", "Eli", "Tamar", "Noam", "Shira",
                           "Daniel", "Maya", "Itay", "Aya", "Jonathan", "Hadas", "Shimon", "Nava", "Oren", "Yael" };
        TypeOfShipment[] shipmentTypes = { TypeOfShipment.Car, TypeOfShipment.Bike, TypeOfShipment.Scooter, TypeOfShipment.OnFoot };

        for (int i = 0; i < 20; i++)
        {
            string name = names[i];
            string phone = $"05{s_rand.Next(0, 10)}{s_rand.Next(10000000, 99999999)}";
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
            DateTime startTime = s_dalConfig!.Clock.AddMinutes(-s_rand.Next(0, 120));

            int id;
            do { id = s_rand.Next(200000000, 400000000); }
            while (s_dalCourier!.Read(id) != null);

            s_dalCourier.Create(new Courier(
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

    private static void createOrders()
    {
        string[] customerNames = { "David Cohen", "Sarah Levy", "Michael Gold", "Leah Stern", "Yossi Azulay",
                                   "Rina Ben", "Eli Horowitz", "Tamar Shalev", "Noam Katz", "Shira Klein" };
        TypeOfOrder[] orderTypes = { TypeOfOrder.SingleOrder, TypeOfOrder.BulkOrder };

        int totalOrders = 50;

        for (int i = 0; i < totalOrders; i++)
        {
            string customerName = customerNames[s_rand.Next(customerNames.Length)];
            string customerPhone = $"05{s_rand.Next(0, 10)}{s_rand.Next(10000000, 99999999)}";

            var addr = addresses[s_rand.Next(addresses.Length)];
            string address = addr.Address;
            double lat = addr.Lat;
            double lon = addr.Lon;

            TypeOfOrder type = orderTypes[s_rand.Next(orderTypes.Length)];
            double weight = Math.Round(s_rand.NextDouble() * 20 + 1, 2);
            DateTime orderTime = s_dalConfig!.Clock.AddMinutes(-s_rand.Next(0, 720));
            string? verbalDesc = s_rand.NextDouble() > 0.5 ? "Fragile, handle with care" : null;

            s_dalOrder!.Create(new Order(
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

    private static void createDeliveries()
    {
        
        List<Order> allOrders = s_dalOrder!.ReadAll().ToList();
        List<Courier> allCouriers = s_dalCourier!.ReadAll().ToList();
        List<Order> availableOrders = new List<Order>(allOrders); 

        foreach (Order order in allOrders)
        {
            
            if (!availableOrders.Contains(order))
                continue;

            Courier? chosenCourier = null;
            double distance = 0;

            foreach (Courier courier in allCouriers)
            {
                distance = CalculateDistance(courier, order);

               
                if (courier.Active && distance <= (courier.Distance ?? 10))
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

            s_dalDelivery!.Create(newDelivery);

            
            if (isFinished || s_rand.NextDouble() > 0.5)
                availableOrders.Remove(order);
        }
    }



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

    public static void Do(ICourier? dalCourier, IOrder? dalOrder, IDelivery? dalDelivery, IConfig? dalConfig)
    {
        s_dalCourier = dalCourier ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalOrder = dalOrder ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalDelivery = dalDelivery ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL object can not be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalCourier.DeleteAll();
        s_dalOrder.DeleteAll();
        s_dalDelivery.DeleteAll();

        Console.WriteLine("Initializing system data ...");
        createConfig();
        createCouriers();
        createOrders();
        createDeliveries();
    }
}
