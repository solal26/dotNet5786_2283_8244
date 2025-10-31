using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
        enum MainMenuOption
        {
            Exit = 0,
            Couriers = 1,
            Orders = 2,
            Deliveries = 3,
            InitializeData = 5,
            ShowAllData = 6,
            Configuration = 7,
            ResetDatabase = 8
        }

        // CRUD options
        enum CrudOption
        {
            Exit = 0,
            Create = 1,
            Read = 2,
            ReadAll = 3,
            Update = 4,
            Delete = 5,
            DeleteAll = 6
        }

        // Configuration options
        enum ConfigOption
        {
            Exit = 0,
            IncrementMinute = 1,
            IncrementHour = 2,
            ShowClock = 3,
            SetConfigValue = 4,
            ShowConfigValues = 5,
            ResetConfig = 6
        }

        // Static DAL instances
        private static ICourier? s_dalCourier = new CourierImplementation();
        private static IOrder? s_dalOrder = new OrderImplementation();
        private static IDelivery? s_dalDelivery = new DeliveryImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();

        private static void ShowMainMenu()
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Manage Couriers");
            Console.WriteLine("2. Manage Orders");
            Console.WriteLine("3. Manage Deliveries");
            Console.WriteLine("5. Initialize Data");
            Console.WriteLine("6. Show All Data");
            Console.WriteLine("7. Configuration");
            Console.WriteLine("8. Reset Database");
            Console.Write("Choose an option: ");
        }
        static void Main(string[] args)
        {
            bool exit = false;
            while (!exit)
            {
                ShowMainMenu();
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input!");
                    continue;
                }

                switch ((MainMenuOption)choice)
                {
                    case MainMenuOption.Exit: exit = true; break;
                    case MainMenuOption.Couriers: CourierMenu(); break;
                    case MainMenuOption.Orders: OrderMenu(); break;
                    case MainMenuOption.Deliveries: DeliveryMenu(); break;
                    case MainMenuOption.InitializeData: Initialization.Do(s_dalCourier!, s_dalOrder!, s_dalDelivery!, s_dalConfig!); break;
                    case MainMenuOption.ShowAllData: ShowAllData(); break;
                    case MainMenuOption.Configuration: ConfigurationMenu(); break;
                    case MainMenuOption.ResetDatabase: ResetDatabase(); break;
                    default: Console.WriteLine("Option does not exist"); break;
                }
            }
        }

        private static void ShowCrudMenu(string entityName)
        {
            Console.WriteLine($"\n--- {entityName} Menu ---");
            Console.WriteLine("0. Back");
            Console.WriteLine("1. Create");
            Console.WriteLine("2. Read by ID");
            Console.WriteLine("3. Read All");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Delete All");
            Console.Write("Choose an option: ");
        }

        // -------------------- COURIER MENU --------------------

        private static void CreateCourier()
        {
            Console.Write("Enter Courier ID: ");
            int id = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Courier Name: ");
            string name = Console.ReadLine()!;

            Console.Write("Enter Phone Number: ");
            string phone = Console.ReadLine()!;

            Console.Write("Enter Email: ");
            string email = Console.ReadLine()!;

            Console.Write("Enter Code: ");
            string code = Console.ReadLine()!;

            Console.Write("Is Active (true/false): ");
            bool active = bool.Parse(Console.ReadLine()!);

            Console.Write("Enter Start Time (yyyy-MM-dd HH:mm): ");
            DateTime timeBegin = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Enter Type of Shipment (Car / Bike / Drone): ");
            TypeOfShipment type = Enum.Parse<TypeOfShipment>(Console.ReadLine()!, true);

            Console.Write("Enter Distance (optional, press Enter to skip): ");
            string? distanceInput = Console.ReadLine();
            double? distance = string.IsNullOrWhiteSpace(distanceInput)
                ? null
                : double.Parse(distanceInput);

            // שלב 1: יצירת אובייקט חדש
            Courier newCourier = new(id, name, phone, email, code, active, timeBegin, type, distance);

            // שלב 2: הוספה לרשימה דרך הממשק בלבד
            s_dalCourier!.Create(newCourier);
        }
        private static void ReadCourier()
        {
            Console.Write("Enter ID: ");
            int id = int.Parse(Console.ReadLine()!);

            var courier = s_dalCourier!.Read(id);
            if (courier != null)
                Console.WriteLine(courier);
            else
                Console.WriteLine("Courier not found.");
        }
        private static void ReadAllCouriers()
        {
            var couriers = s_dalCourier!.ReadAll();
            foreach (var c in couriers)
                Console.WriteLine(c);
        }
        private static void UpdateCourier()
        {
            Console.Write("Enter ID of courier to update: ");
            int id = int.Parse(Console.ReadLine()!);

            Console.Write("Enter new Name: ");
            string name = Console.ReadLine()!;

            Console.Write("Enter new Phone Number: ");
            string phone = Console.ReadLine()!;

            Console.Write("Enter new Email: ");
            string email = Console.ReadLine()!;

            Console.Write("Enter new Code: ");
            string code = Console.ReadLine()!;

            Console.Write("Is Active (true/false): ");
            bool active = bool.Parse(Console.ReadLine()!);

            Console.Write("Enter new Time Begin (yyyy-MM-dd HH:mm): ");
            DateTime timeBegin = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Enter Type of Shipment (Car/Drone/Bike): ");
            TypeOfShipment type = Enum.Parse<TypeOfShipment>(Console.ReadLine()!);

            Console.Write("Enter Distance (optional, leave empty for null): ");
            string distInput = Console.ReadLine()!;
            double? distance = string.IsNullOrWhiteSpace(distInput) ? null : double.Parse(distInput);

            var updatedCourier = new Courier(id, name, phone, email, code, active, timeBegin, type, distance);

            s_dalCourier!.Update(updatedCourier);

        }
        private static void DeleteCourier()
        {
            Console.Write("Enter ID of courier to delete: ");
            int id = int.Parse(Console.ReadLine()!);

            s_dalCourier!.Delete(id);
        }
        private static void CourierMenu()
        {
            bool exit = false;
            while (!exit)
            {
                ShowCrudMenu("Courier");
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                switch ((CrudOption)choice)
                {
                    case CrudOption.Exit: exit = true; break;
                    case CrudOption.Create:
                        CreateCourier();
                        break;
                    case CrudOption.Read:
                        ReadCourier();
                        break;
                    case CrudOption.ReadAll:
                        ReadAllCouriers();
                        break;
                    case CrudOption.Update:
                        UpdateCourier();
                        break;
                    case CrudOption.Delete:
                        DeleteCourier();
                        break;
                    case CrudOption.DeleteAll:
                        s_dalCourier!.DeleteAll();
                        break;
                }
            }
        }

        // -------------------- ORDER MENU --------------------
        private static void CreateOrder()
        {
            Console.Write("Enter Order ID: ");
            int id = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Type of Order (SingleOrder/OtherType...): ");
            TypeOfOrder type = Enum.Parse<TypeOfOrder>(Console.ReadLine()!);

            Console.Write("Enter Shipping Address: ");
            string address = Console.ReadLine()!;

            Console.Write("Enter Latitude: ");
            double lat = double.Parse(Console.ReadLine()!);

            Console.Write("Enter Longitude: ");
            double lon = double.Parse(Console.ReadLine()!);

            Console.Write("Enter Customer Name: ");
            string customerName = Console.ReadLine()!;

            Console.Write("Enter Customer Phone: ");
            string customerPhone = Console.ReadLine()!;

            Console.Write("Enter Weight: ");
            double weight = double.Parse(Console.ReadLine()!);

            Console.Write("Enter Order Initiation Date (yyyy-MM-dd HH:mm): ");
            DateTime initiation = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Enter Verbal Description (optional, leave empty if none): ");
            string? desc = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(desc)) desc = null;

            var newOrder = new Order(id, type, address, lat, lon, customerName, customerPhone, weight, initiation, desc);

            s_dalOrder!.Create(newOrder);
            Console.WriteLine("Order created successfully.");
        }
        private static void ReadOrder()
        {
            Console.Write("Enter Order ID: ");
            int id = int.Parse(Console.ReadLine()!);

            var order = s_dalOrder!.Read(id);
            if (order != null)
                Console.WriteLine(order);
            else
                Console.WriteLine("Order not found.");
        }
        private static void ReadAllOrders()
        {
            foreach (var o in s_dalOrder!.ReadAll())
                Console.WriteLine(o);
        }
        private static void UpdateOrder()
        {
            Console.Write("Enter ID of order to update: ");
            int id = int.Parse(Console.ReadLine()!);

            var existingOrder = s_dalOrder!.Read(id);
            if (existingOrder == null)
            {
                Console.WriteLine("Order not found.");
                return;
            }

            Console.WriteLine("Current Order: ");
            Console.WriteLine(existingOrder);

            Console.Write("Enter new Type of Order (leave empty to keep current): ");
            string typeInput = Console.ReadLine()!;
            TypeOfOrder type = string.IsNullOrWhiteSpace(typeInput) ? existingOrder.typeOfOrder : Enum.Parse<TypeOfOrder>(typeInput);

            Console.Write("Enter new Shipping Address (leave empty to keep current): ");
            string address = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(address)) address = existingOrder.ShippingAddress;

            Console.Write("Enter new Latitude (leave empty to keep current): ");
            string latInput = Console.ReadLine()!;
            double lat = string.IsNullOrWhiteSpace(latInput) ? existingOrder.Latitude : double.Parse(latInput);

            Console.Write("Enter new Longitude (leave empty to keep current): ");
            string lonInput = Console.ReadLine()!;
            double lon = string.IsNullOrWhiteSpace(lonInput) ? existingOrder.Longitude : double.Parse(lonInput);

            Console.Write("Enter new Customer Name (leave empty to keep current): ");
            string customerName = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(customerName)) customerName = existingOrder.CustomerName;

            Console.Write("Enter new Customer Phone (leave empty to keep current): ");
            string customerPhone = Console.ReadLine()!;
            if (string.IsNullOrWhiteSpace(customerPhone)) customerPhone = existingOrder.CustomerPhone;

            Console.Write("Enter new Weight (leave empty to keep current): ");
            string weightInput = Console.ReadLine()!;
            double weight = string.IsNullOrWhiteSpace(weightInput) ? existingOrder.weight : double.Parse(weightInput);

            Console.Write("Enter new Order Initiation Date (yyyy-MM-dd HH:mm, leave empty to keep current): ");
            string dateInput = Console.ReadLine()!;
            DateTime initiation = string.IsNullOrWhiteSpace(dateInput) ? existingOrder.OrderInitiation : DateTime.Parse(dateInput);

            Console.Write("Enter new Verbal Description (leave empty to keep current): ");
            string? desc = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(desc)) desc = existingOrder.VerbalDescription;

            var updatedOrder = new Order(id, type, address, lat, lon, customerName, customerPhone, weight, initiation, desc);

            s_dalOrder.Update(updatedOrder);
            Console.WriteLine("Order updated successfully.");
        }
        private static void DeleteOrder()
        {
            Console.Write("Enter Order ID to delete: ");
            int id = int.Parse(Console.ReadLine()!);

            s_dalOrder!.Delete(id);
            Console.WriteLine("Order deleted successfully.");
        }
        private static void OrderMenu()
        {
            bool exit = false;
            while (!exit)
            {
                ShowCrudMenu("Order");
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                switch ((CrudOption)choice)
                {
                    case CrudOption.Exit: exit = true; break;
                    case CrudOption.Create:
                         CreateOrder();
                        break;
                    case CrudOption.Read:
                        ReadOrder();
                        break;
                    case CrudOption.ReadAll:
                        ReadAllOrders();
                        break;
                    case CrudOption.Update:
                        UpdateOrder();
                        break;
                    case CrudOption.Delete:
                        DeleteOrder();
                        break;
                    case CrudOption.DeleteAll:
                        s_dalOrder!.DeleteAll();
                        break;
                }
            }
        }

        // -------------------- DELIVERY MENU --------------------
        private static void CreateDelivery()
        {
            Console.Write("Enter Delivery ID: ");
            int id = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Order ID: ");
            int orderId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Courier ID: ");
            int courierId = int.Parse(Console.ReadLine()!);

            Console.Write("Enter Type of Shipment (Car/Drone/Bike): ");
            TypeOfShipment type = Enum.Parse<TypeOfShipment>(Console.ReadLine()!);

            Console.Write("Enter Time Begin (yyyy-MM-dd HH:mm): ");
            DateTime timeBegin = DateTime.Parse(Console.ReadLine()!);

            Console.Write("Enter End of Courier (optional, leave empty for null): ");
            string? endCourierInput = Console.ReadLine();
            endofcourier? endCourier = string.IsNullOrWhiteSpace(endCourierInput) ? null : Enum.Parse<endofcourier>(endCourierInput);

            Console.Write("Enter Time End (optional, leave empty for null): ");
            string? timeEndInput = Console.ReadLine();
            DateTime? timeEnd = string.IsNullOrWhiteSpace(timeEndInput) ? null : DateTime.Parse(timeEndInput);

            Console.Write("Enter Distance (optional, leave empty for null): ");
            string? distanceInput = Console.ReadLine();
            double? distance = string.IsNullOrWhiteSpace(distanceInput) ? null : double.Parse(distanceInput);

            var newDelivery = new Delivery(id, orderId, courierId, type, timeBegin, endCourier, timeEnd, distance);

            s_dalDelivery!.Create(newDelivery);
            Console.WriteLine("Delivery created successfully.");
        }
        private static void ReadDelivery()
        {
            Console.Write("Enter Delivery ID: ");
            int id = int.Parse(Console.ReadLine()!);

            var delivery = s_dalDelivery!.Read(id);
            if (delivery != null)
                Console.WriteLine(delivery);
            else
                Console.WriteLine("Delivery not found.");
        }
        private static void ReadAllDeliveries()
        {
            foreach (var d in s_dalDelivery!.ReadAll())
                Console.WriteLine(d);
        }
        private static void UpdateDelivery()
        {
            Console.Write("Enter ID of delivery to update: ");
            int id = int.Parse(Console.ReadLine()!);

            var existingDelivery = s_dalDelivery!.Read(id);
            if (existingDelivery == null)
            {
                Console.WriteLine("Delivery not found.");
                return;
            }

            Console.WriteLine("Current delivery: " + existingDelivery);

            // קלט עם אפשרות להשאיר ריק כדי לא לעדכן
            Console.Write("Enter new Order ID (leave empty to keep current): ");
            string input = Console.ReadLine()!;
            int orderId = string.IsNullOrWhiteSpace(input) ? existingDelivery.OrderId : int.Parse(input);

            Console.Write("Enter new Courier ID (leave empty to keep current): ");
            input = Console.ReadLine()!;
            int courierId = string.IsNullOrWhiteSpace(input) ? existingDelivery.CourierId : int.Parse(input);

            Console.Write("Enter Type of Shipment (Car/Drone/Bike, leave empty to keep current): ");
            input = Console.ReadLine()!;
            TypeOfShipment type = string.IsNullOrWhiteSpace(input) ? existingDelivery.typeofshipment : Enum.Parse<TypeOfShipment>(input);

            Console.Write("Enter new Time Begin (yyyy-MM-dd HH:mm, leave empty to keep current): ");
            input = Console.ReadLine()!;
            DateTime timeBegin = string.IsNullOrWhiteSpace(input) ? existingDelivery.TimeBegin : DateTime.Parse(input);

            Console.Write("Enter End of Courier (leave empty to keep current): ");
            input = Console.ReadLine()!;
            endofcourier? endCourier = string.IsNullOrWhiteSpace(input) ? existingDelivery.endcour : Enum.Parse<endofcourier>(input);

            Console.Write("Enter Time End (leave empty to keep current): ");
            input = Console.ReadLine()!;
            DateTime? timeEnd = string.IsNullOrWhiteSpace(input) ? existingDelivery.TimeEnd : DateTime.Parse(input);

            Console.Write("Enter Distance (leave empty to keep current): ");
            input = Console.ReadLine()!;
            double? distance = string.IsNullOrWhiteSpace(input) ? existingDelivery.Distance : double.Parse(input);

            var updatedDelivery = new Delivery(id, orderId, courierId, type, timeBegin, endCourier, timeEnd, distance);

            s_dalDelivery.Update(updatedDelivery);
        }
        private static void DeleteDelivery()
        {
            Console.Write("Enter Delivery ID to delete: ");
            int id = int.Parse(Console.ReadLine()!);

            s_dalDelivery!.Delete(id);
            Console.WriteLine("Delivery deleted successfully.");
        }
        private static void DeliveryMenu()
        {
            bool exit = false;
            while (!exit)
            {
                ShowCrudMenu("Delivery");
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                switch ((CrudOption)choice)
                {
                    case CrudOption.Exit: exit = true; break;
                    case CrudOption.Create:
                        CreateDelivery();
                        break;
                    case CrudOption.Read:
                        ReadDelivery();
                        break;
                    case CrudOption.ReadAll:
                        ReadAllDeliveries();
                        break;
                    case CrudOption.Update:
                        UpdateDelivery();
                        break;
                    case CrudOption.Delete:
                        DeleteDelivery();
                        break;
                    case CrudOption.DeleteAll:
                        s_dalDelivery!.DeleteAll();
                        break;
                }
            }
        }

        // -------------------- CONFIGURATION MENU --------------------
        private static void ConfigurationMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- Configuration Menu ---");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Increment Minute");
                Console.WriteLine("2. Increment Hour");
                Console.WriteLine("3. Show Clock");
                Console.WriteLine("4. Set Config Value");
                Console.WriteLine("5. Show Config Values");
                Console.WriteLine("6. Reset Config");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                switch ((ConfigOption)choice)
                {
                    case ConfigOption.Exit: exit = true; break;
                    case ConfigOption.IncrementMinute: s_dalConfig!.IncrementMinute(); break;
                    case ConfigOption.IncrementHour: s_dalConfig!.IncrementHour(); break;
                    case ConfigOption.ShowClock: Console.WriteLine(s_dalConfig!.Clock); break;
                    case ConfigOption.SetConfigValue: s_dalConfig!.SetSomeValue(); break;
                    case ConfigOption.ShowConfigValues: s_dalConfig!.ShowValues(); break;
                    case ConfigOption.ResetConfig: s_dalConfig!.Reset(); break;
                }
            }
        }


        // -------------------- SHOW ALL DATA --------------------
        private static void ShowAllData()
        {
            Console.WriteLine("\n--- All Data ---");
            foreach (var c in s_dalCourier!.ReadAll()) Console.WriteLine(c);
            foreach (var o in s_dalOrder!.ReadAll()) Console.WriteLine(o);
            foreach (var d in s_dalDelivery!.ReadAll()) Console.WriteLine(d);
        }

        // -------------------- RESET DATABASE --------------------
        private static void ResetDatabase()
        {
            s_dalCourier!.DeleteAll();
            s_dalOrder!.DeleteAll();
            s_dalDelivery!.DeleteAll();
            s_dalConfig!.Reset();
            Console.WriteLine("Database reset completed.");
        }
    }
}
//////////////////////////////////////////////////////////////////

