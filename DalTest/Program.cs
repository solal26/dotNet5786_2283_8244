using Dal;
using DalApi;

namespace DalTest
{
    internal class Program
    {

        //private static ICourier? s_dalCourier = new CourierImplementation();//stage 1;
        //private static IOrder? s_dalOrder=new OrderImplementation(); //stage 1
        //private static IDelivery? s_dalDelivery=new DeliveryImplementation(); //stage 1
        //private static IConfig? s_dalConfig=new ConfigImplementation(); //stage 1

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

        // -------------------- COURIER MENU --------------------
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
                        Console.Write("Enter Courier ID: ");
                        int id = int.Parse(Console.ReadLine()!);

                        Console.Write("Enter Courier Name: ");
                        string name = Console.ReadLine()!;

                        // Step 1: Create object
                        CourierImplementation newCourier = new CourierImplementation();
                        newCourier.Id = id;
                       newCourier.Name = name;
                        s_dalCourier.Create(newCourier); // s_dalCourier 

                        // Step 2: Add to DAL
                        s_dalCourier.Create(newCourier);
                        break;
                    case CrudOption.Read:
                        var readCourier = ReadCourierById();
                        if (readCourier != null) Console.WriteLine(readCourier);
                        break;
                    case CrudOption.ReadAll:
                        foreach (var c in s_dalCourier!.ReadAll()) Console.WriteLine(c);
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
                        var newOrder = CreateOrderFromInput();
                        s_dalOrder!.Create(newOrder);
                        break;
                    case CrudOption.Read:
                        var readOrder = ReadOrderById();
                        if (readOrder != null) Console.WriteLine(readOrder);
                        break;
                    case CrudOption.ReadAll:
                        foreach (var o in s_dalOrder!.ReadAll()) Console.WriteLine(o);
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
                        var newDelivery = CreateDeliveryFromInput();
                        s_dalDelivery!.Create(newDelivery);
                        break;
                    case CrudOption.Read:
                        var readDelivery = ReadDeliveryById();
                        if (readDelivery != null) Console.WriteLine(readDelivery);
                        break;
                    case CrudOption.ReadAll:
                        foreach (var d in s_dalDelivery!.ReadAll()) Console.WriteLine(d);
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

