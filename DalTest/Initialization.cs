namespace DalTest;
using DalApi;
using DO;

public static class Initialization
{
    private static ICourier? s_dalCourier; //stage 1
    private static IOrder? s_dalOrder; //stage 1
    private static IDelivery? s_dalDelivery; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();

    private static void createConfig()
    {
        s_dalConfig!.ManagerId = 123456789;
        s_dalConfig.ManagerPassword = "admin123";
        s_dalConfig.CompanyAddress = "Tel Aviv, Israel";
        s_dalConfig.MaxRange = 30; // ק"מ
        s_dalConfig.AvgCarSpeed = 60.0;
        s_dalConfig.AvgMotorbikeSpeed = 45.0;
        s_dalConfig.AvgBicycleSpeed = 15.0;
        s_dalConfig.AvgWalkingSpeed = 5.0;
        s_dalConfig.MaxDeliveryTime = TimeSpan.FromHours(3);
        s_dalConfig.RiskRange = TimeSpan.FromMinutes(30);
        s_dalConfig.InactivityRange = TimeSpan.FromMinutes(15);
        s_dalConfig.Clock = DateTime.Now; // נקודת זמן המערכת
    }

    private static void createCouriers()
    {
        string[] names = { "David", "Sarah", "Michael", "Leah", "Yossi", "Rina", "Eli", "Tamar", "Noam", "Shira",
                       "Daniel", "Maya", "Itay", "Aya", "Jonathan", "Hadas", "Shimon", "Nava", "Oren", "Yael" };
        TypeOfShipment[] shipmentTypes = { TypeOfShipment.Car, TypeOfShipment.Bike, TypeOfShipment.Scooter, TypeOfShipment.OnFoot };

  

        for (int i = 0; i < 20; i++)
        {
            string name = names[i]; // random name
            
            string phone = $"05{ s_rand.Next(0, 10)}{ s_rand.Next(10000000, 99999999)}";// random phone number
            string email = $"{name.ToLower().Replace(" ", ".")}{s_rand.Next(1, 10000)}@gmail.com";  // random email
            string code = s_rand.Next(1000, 10000).ToString();// כל קוד ייחודי

            TypeOfShipment type = shipmentTypes[s_rand.Next(shipmentTypes.Length)];
            bool active = s_rand.NextDouble() > 0.2; // רובם פעילים

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
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dalCourier!.Read(id) != null);

            s_dalCourier.Create(new DO.Courier
            (
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

    private static readonly (string Address, double Latitude, double Longitude)[] addresses =
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

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // נוסחת Haversine בקילומטרים
        double R = 6371; // רדיוס כדור הארץ בק"מ
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static void createOrders()
    {
        string[] customerNames = { "David Cohen", "Sarah Levy", "Michael Gold", "Leah Stern", "Yossi Azulay",
                               "Rina Ben", "Eli Horowitz", "Tamar Shalev", "Noam Katz", "Shira Klein" };


        




        TypeOfOrder[] orderTypes = { TypeOfOrder.SingleOrder, TypeOfOrder.BulkOrder };

        // מספר ההזמנות
        int totalOrders = 50;

        for (int i = 0; i < totalOrders; i++)
        {
            string customerName = customerNames[s_rand.Next(customerNames.Length)];
            string customerPhone = $"05{s_rand.Next(0, 10)}{s_rand.Next(10000000, 99999999)}";// random phone number

            // כתובת רנדומלית מתוך המערך
            int addrIndex = s_rand.Next(addresses.Length);
            string address = addresses[addrIndex].Address;
            double latitude = addresses[addrIndex].Latitude;
            double longitude = addresses[addrIndex].Longitude;

            // סוג הזמנה אקראי
            TypeOfOrder type = orderTypes[s_rand.Next(orderTypes.Length)];

            // משקל אקראי 1-20 ק"ג
            double weight = Math.Round(s_rand.NextDouble() * 20 + 1, 2);

            // זמן פתיחת הזמנה קטן מהשעון
            DateTime orderTime = s_dalConfig!.Clock.AddMinutes(-s_rand.Next(0, 720));

            // תיאור מילולי אקראי או null
            string? verbalDesc = s_rand.NextDouble() > 0.5 ? "Fragile, handle with care" : null;

            // סטטוס הזמנה: 0-פתוחה, 1-בטיפול, 2-סגורה
            // 20 פתוחות, 10 בטיפול, 20 סגורות
            TypeOfOrder status = i < 20 ? TypeOfOrder.SingleOrder : i < 30 ? TypeOfOrder.BulkOrder : TypeOfOrder.SingleOrder;

            s_dalOrder!.Create(new DO.Order
            (
                OrderId: 0, // נוצרת ב-DAL
                typeOfOrder: type,
                ShippingAddress: address,
                Latitude: latitude,
                Longitude: longitude,
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
        var orders = s_dalOrder!.GetAll().ToList();
        var couriers = s_dalCourier!.GetAll().ToList();

        foreach (var order in orders.Where(o => o.Status != "Closed"))
        {
            // רשימת שליחים שעומדים במרחק מירבי ומפעילים
            var eligibleCouriers = couriers
                .Where(c => Distance(c.Address, order.Address) <= c.MaxDistance && c.Active)
                .ToList();

            if (!eligibleCouriers.Any()) continue;

            var courier = eligibleCouriers[s_rand.Next(eligibleCouriers.Count)];
            DateTime startDelivery = order.OrderTime.AddMinutes(s_rand.Next(1, 30));
            DateTime? endDelivery = null;
            string status = "InProcess";

            // הגרלת סיום המשלוח אם רלוונטי
            if (s_rand.NextDouble() > 0.5) // סיום המשלוח
            {
                status = "Completed";
                endDelivery = startDelivery.AddMinutes(s_rand.Next(10, 120));
            }

            s_dalDelivery!.Create(new DO.Delivery
            {
                Id = 0, // DAL יוצר אוטומטית
                OrderId = order.Id,
                CourierId = courier.Id,
                StartTime = startDelivery,
                EndTime = endDelivery,
                Status = status
            });
        }
    }

    // מתודת עזר לחישוב מרחק קירוב בין כתובות
    private static double Distance(string a, string b)
    {
        // בדוגמה זו, החזר מרחק רנדומלי
        return s_rand.NextDouble() * 50; // 0-50 ק"מ
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

        Console.WriteLine("Initializing Students list ...");
        createCouriers();
        createOrders();
        createDeliveries();
        createConfig();
    }

}
