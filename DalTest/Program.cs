using Dal;
using DalApi;
using DO;

namespace DalTest
{
    /// <summary>
    /// Console demo application for exercising the DAL implementations.
    /// Provides interactive menus to create, read, update and delete entities:
    /// Couriers, Orders and Deliveries, plus simple configuration and data init.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Top-level main menu choices presented to the user.
        /// Values map to the numeric input from the console.
        /// </summary>
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

        /// <summary>
        /// CRUD submenu choices for each entity type.
        /// </summary>
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

        /// <summary>
        /// Configuration-related operations exposed in the configuration menu.
        /// </summary>
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

        // Static DAL instances used by the console app to delegate persistence operations.
        // These are concrete implementations used for testing and manual interaction.
        private static ICourier? s_dalCourier = new CourierImplementation();
        private static IOrder? s_dalOrder = new OrderImplementation();
        private static IDelivery? s_dalDelivery = new DeliveryImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();

        /// <summary>
        /// Print the main menu to the console.
        /// Kept minimal — numeric choices correspond to <see cref="MainMenuOption"/>.
        /// </summary>
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

        /// <summary>
        /// Application entry point. Runs an interactive loop until the user chooses to exit.
        /// All top-level menu choices are handled here and exceptions are displayed to the console.
        /// </summary>
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
                try
                {
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Generic CRUD menu header printer for a given entity name.
        /// Numbers correspond to <see cref="CrudOption"/>.
        /// </summary>
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

        /// <summary>
        /// Prompt the user and create a new courier entity.
        /// Input is not heavily validated — intended for manual testing only.
        /// </summary>
        private static void CreateCourier()
        {
            try
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

                Console.Write("Enter Type of Shipment (Car / Bike / Drone): ");
                TypeOfShipment type = Enum.Parse<TypeOfShipment>(Console.ReadLine()!, true);

                Console.Write("Enter Distance (optional, press Enter to skip): ");
                string? distanceInput = Console.ReadLine();
                double? distance = string.IsNullOrWhiteSpace(distanceInput) ? null : double.Parse(distanceInput);

                Courier newCourier = new(id, name, phone, email, code, active, DateTime.Now, type, distance);

                s_dalCourier!.Create(newCourier);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Read and display a courier by id.
        /// </summary>
        private static void ReadCourier()
        {
            try
            {
                Console.Write("Enter ID: ");
                int id = int.Parse(Console.ReadLine()!);

                var courier = s_dalCourier!.Read(id);
                if (courier != null)
                    Console.WriteLine(courier);
                else
                    Console.WriteLine("Courier not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Display all couriers returned from the DAL.
        /// </summary>
        private static void ReadAllCouriers()
        {
            try
            {
                foreach (var c in s_dalCourier!.ReadAll())
                    Console.WriteLine(c);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Update a courier interactively. Empty input keeps the current value.
        /// </summary>
        private static void UpdateCourier()
        {
            try
            {
                Console.Write("Enter ID of courier to update: ");
                int id = int.Parse(Console.ReadLine()!);

                var existingCourier = s_dalCourier!.Read(id);
                if (existingCourier == null)
                {
                    Console.WriteLine("Courier not found.");
                    return;
                }

                Console.WriteLine("Current Courier:");
                Console.WriteLine(existingCourier);

                Console.Write("Enter new Name (leave empty to keep current): ");
                string name = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(name)) name = existingCourier.Name;

                Console.Write("Enter new Phone Number (leave empty to keep current): ");
                string phone = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(phone)) phone = existingCourier.PhoneNumber;

                Console.Write("Enter new Email (leave empty to keep current): ");
                string email = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(email)) email = existingCourier.Email;

                Console.Write("Enter new Code (leave empty to keep current): ");
                string code = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(code)) code = existingCourier.Code;

                Console.Write("Is Active (true/false, leave empty to keep current): ");
                string activeInput = Console.ReadLine()!;
                bool active = string.IsNullOrWhiteSpace(activeInput) ? existingCourier.Active : bool.Parse(activeInput);

                Console.Write("Enter Type of Shipment (Car/Drone/Bike, leave empty to keep current): ");
                string typeInput = Console.ReadLine()!;
                TypeOfShipment type = string.IsNullOrWhiteSpace(typeInput) ? existingCourier.typeofshipment : Enum.Parse<TypeOfShipment>(typeInput);

                Console.Write("Enter Distance (leave empty to keep current): ");
                string distInput = Console.ReadLine()!;
                double? distance = string.IsNullOrWhiteSpace(distInput) ? existingCourier.Distance : double.Parse(distInput);

                var updatedCourier = new Courier(id, name, phone, email, code, active, existingCourier.TimeBegin, type, distance);

                s_dalCourier.Update(updatedCourier);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a courier by id.
        /// </summary>
        private static void DeleteCourier()
        {
            try
            {
                Console.Write("Enter ID of courier to delete: ");
                int id = int.Parse(Console.ReadLine()!);

                s_dalCourier!.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Interactive loop for courier operations using the CRUD submenu.
        /// </summary>
        private static void CourierMenu()
        {
            bool exit = false;
            while (!exit)
            {
                ShowCrudMenu("Courier");
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                try
                {
                    switch ((CrudOption)choice)
                    {
                        case CrudOption.Exit: exit = true; break;
                        case CrudOption.Create: CreateCourier(); break;
                        case CrudOption.Read: ReadCourier(); break;
                        case CrudOption.ReadAll: ReadAllCouriers(); break;
                        case CrudOption.Update: UpdateCourier(); break;
                        case CrudOption.Delete: DeleteCourier(); break;
                        case CrudOption.DeleteAll: s_dalCourier!.DeleteAll(); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught: {ex.Message}");
                }
            }
        }

        // -------------------- ORDER MENU --------------------

        /// <summary>
        /// Prompt user and create a new order.
        /// Minimal validation; intended for manual testing.
        /// </summary>
        private static void CreateOrder()
        {
            try
            {

                Console.Write("Enter Type of Order (SingleOrder/OtherType...): ");
                TypeOfOrder type = Enum.Parse<TypeOfOrder>(Console.ReadLine()!);

                Console.Write("Enter Shipping Address: ");
                string address = Console.ReadLine()!;

                Console.Write("Enter Customer Name: ");
                string customerName = Console.ReadLine()!;

                Console.Write("Enter Customer Phone: ");
                string customerPhone = Console.ReadLine()!;

                Console.Write("Enter Weight: ");
                double weight = double.Parse(Console.ReadLine()!);

                Console.Write("Enter Verbal Description (optional, leave empty if none): ");
                string? desc = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(desc)) desc = null;

                var newOrder = new Order(0, type, address, 0, 0, customerName, customerPhone, weight, DateTime.Now, desc);

                s_dalOrder!.Create(newOrder);
                Console.WriteLine("Order created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Read and display an order by id.
        /// </summary>
        private static void ReadOrder()
        {
            try
            {
                Console.Write("Enter Order ID: ");
                int id = int.Parse(Console.ReadLine()!);

                var order = s_dalOrder!.Read(id);
                if (order != null)
                    Console.WriteLine(order);
                else
                    Console.WriteLine("Order not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Display all orders from the DAL.
        /// </summary>
        private static void ReadAllOrders()
        {
            try
            {
                foreach (var o in s_dalOrder!.ReadAll())
                    Console.WriteLine(o);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an order interactively. Empty input preserves current values.
        /// </summary>
        private static void UpdateOrder()
        {
            try
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

                Console.Write("Enter new Customer Name (leave empty to keep current): ");
                string customerName = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(customerName)) customerName = existingOrder.CustomerName;

                Console.Write("Enter new Customer Phone (leave empty to keep current): ");
                string customerPhone = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(customerPhone)) customerPhone = existingOrder.CustomerPhone;

                Console.Write("Enter new Weight (leave empty to keep current): ");
                string weightInput = Console.ReadLine()!;
                double weight = string.IsNullOrWhiteSpace(weightInput) ? existingOrder.weight : double.Parse(weightInput);

                Console.Write("Enter new Verbal Description (leave empty to keep current): ");
                string? desc = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(desc)) desc = existingOrder.VerbalDescription;

                var updatedOrder = new Order(id, type, address, 0, 0, customerName, customerPhone, weight, existingOrder.OrderInitiation, desc);

                s_dalOrder.Update(updatedOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete an order by id.
        /// </summary>
        private static void DeleteOrder()
        {
            try
            {
                Console.Write("Enter Order ID to delete: ");
                int id = int.Parse(Console.ReadLine()!);

                s_dalOrder!.Delete(id);
                Console.WriteLine("Order deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Interactive loop for order CRUD operations.
        /// </summary>
        private static void OrderMenu()
        {
            bool exit = false;
            while (!exit)
            {
                ShowCrudMenu("Order");
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                try
                {
                    switch ((CrudOption)choice)
                    {
                        case CrudOption.Exit: exit = true; break;
                        case CrudOption.Create: CreateOrder(); break;
                        case CrudOption.Read: ReadOrder(); break;
                        case CrudOption.ReadAll: ReadAllOrders(); break;
                        case CrudOption.Update: UpdateOrder(); break;
                        case CrudOption.Delete: DeleteOrder(); break;
                        case CrudOption.DeleteAll: s_dalOrder!.DeleteAll(); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught: {ex.Message}");
                }
            }
        }


        // -------------------- DELIVERY MENU --------------------

        /// <summary>
        /// Create a delivery by prompting for order id, courier id, shipment type and distance.
        /// TimeBegin is set to now. End and completion details are optional.
        /// </summary>
        private static void CreateDelivery()
        {
            try
            {
                Console.Write("Enter Order ID: ");
                int orderId = int.Parse(Console.ReadLine()!);

                Console.Write("Enter Courier ID: ");
                int courierId = int.Parse(Console.ReadLine()!);

                Console.Write("Enter Type of Shipment (Car/Drone/Bike): ");
                TypeOfShipment type = Enum.Parse<TypeOfShipment>(Console.ReadLine()!);

                Console.Write("Enter Distance (optional, leave empty for null): ");
                string? distanceInput = Console.ReadLine();
                double? distance = string.IsNullOrWhiteSpace(distanceInput) ? null : double.Parse(distanceInput);

                var newDelivery = new Delivery(0, orderId, courierId, type, DateTime.Now, null, null, distance);

                s_dalDelivery!.Create(newDelivery);
                Console.WriteLine("Delivery created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Read and display a delivery by id.
        /// </summary>
        private static void ReadDelivery()
        {
            try
            {
                Console.Write("Enter Delivery ID: ");
                int id = int.Parse(Console.ReadLine()!);

                var delivery = s_dalDelivery!.Read(id);
                if (delivery != null)
                    Console.WriteLine(delivery);
                else
                    Console.WriteLine("Delivery not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Show all deliveries from the DAL.
        /// </summary>
        private static void ReadAllDeliveries()
        {
            try
            {
                foreach (var d in s_dalDelivery!.ReadAll())
                    Console.WriteLine(d);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a delivery by id. Prompts the user and delegates to the DAL.
        /// </summary>
        private static void DeleteDelivery()
        {
            try
            {
                Console.Write("Enter Delivery ID to delete: ");
                int id = int.Parse(Console.ReadLine()!);

                s_dalDelivery!.Delete(id);
                Console.WriteLine("Delivery deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Interactive loop for delivery CRUD. Update is intentionally not implemented.
        /// Closed deliveries should include end time and completion type in a fuller implementation.
        /// </summary>
        private static void DeliveryMenu()
        {
            bool exit = false;
            while (!exit)
            {
                ShowCrudMenu("Delivery");
                if (!int.TryParse(Console.ReadLine(), out int choice)) continue;

                try
                {
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
                            Console.WriteLine("Update operation is not implemented for Delivery.");
                            break;
                        case CrudOption.Delete:
                            DeleteDelivery();
                            break;
                        case CrudOption.DeleteAll:
                            s_dalDelivery!.DeleteAll();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught: {ex.Message}");
                }
            }
        }

        // -------------------- CONFIGURATION MENU --------------------

        /// <summary>
        /// Manage simple configuration actions: advance clock, set values and reset config.
        /// </summary>
        private static void ConfigurationMenu()
        {
            bool exit = false;
            while (!exit)
            {
                try
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
                        case ConfigOption.IncrementMinute: s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(1); break;
                        case ConfigOption.IncrementHour: s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(1); break;
                        case ConfigOption.ShowClock: Console.WriteLine(s_dalConfig!.Clock); break;
                        case ConfigOption.SetConfigValue: SetConfigValue(); break;
                        case ConfigOption.ShowConfigValues: ShowConfigValue(); break;
                        case ConfigOption.ResetConfig: s_dalConfig!.Reset(); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception caught: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Print a selected configuration value to the console.
        /// </summary>
        private static void ShowConfigValue()
        {
            try
            {
                Console.WriteLine("\nWhich configuration value do you want to view?");
                Console.WriteLine("1. Manager ID");
                Console.WriteLine("2. Manager Password");
                Console.WriteLine("3. Company Address");
                Console.WriteLine("6. Max Range");
                Console.WriteLine("7. Avg Car Speed");
                Console.WriteLine("8. Avg Motorbike Speed");
                Console.WriteLine("9. Avg Bicycle Speed");
                Console.WriteLine("10. Avg Walking Speed");
                Console.WriteLine("11. Max Delivery Time");
                Console.WriteLine("12. Risk Range");
                Console.WriteLine("13. Inactivity Range");
                Console.WriteLine("14. Clock");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int option))
                {
                    Console.WriteLine("Invalid input.");
                    return;
                }

                switch (option)
                {
                    case 1: Console.WriteLine($"Manager ID: {s_dalConfig!.ManagerId}"); break;
                    case 2: Console.WriteLine($"Manager Password: {s_dalConfig!.ManagerPassword}"); break;
                    case 3: Console.WriteLine($"Company Address: {s_dalConfig!.CompanyAddress}"); break;
                    case 6: Console.WriteLine($"Max Range: {s_dalConfig!.MaxRange}"); break;
                    case 7: Console.WriteLine($"Avg Car Speed: {s_dalConfig!.AvgCarSpeed}"); break;
                    case 8: Console.WriteLine($"Avg Motorbike Speed: {s_dalConfig!.AvgMotorbikeSpeed}"); break;
                    case 9: Console.WriteLine($"Avg Bicycle Speed: {s_dalConfig!.AvgBicycleSpeed}"); break;
                    case 10: Console.WriteLine($"Avg Walking Speed: {s_dalConfig!.AvgWalkingSpeed}"); break;
                    case 11: Console.WriteLine($"Max Delivery Time: {s_dalConfig!.MaxDeliveryTime}"); break;
                    case 12: Console.WriteLine($"Risk Range: {s_dalConfig!.RiskRange}"); break;
                    case 13: Console.WriteLine($"Inactivity Range: {s_dalConfig!.InactivityRange}"); break;
                    case 14: Console.WriteLine($"Clock: {s_dalConfig!.Clock}"); break;
                    default: Console.WriteLine("Invalid choice."); break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        /// <summary>
        /// Prompt the user to set a configuration value. Inputs are parsed and assigned to the config instance.
        /// </summary>
        private static void SetConfigValue()
        {
            try
            {
                Console.WriteLine("\nWhich configuration value do you want to set?");
                Console.WriteLine("1. Manager ID");
                Console.WriteLine("2. Manager Password");
                Console.WriteLine("3. Company Address");
                Console.WriteLine("4. Max Range");
                Console.WriteLine("5. Avg Car Speed");
                Console.WriteLine("6. Avg Motorbike Speed");
                Console.WriteLine("7. Avg Bicycle Speed");
                Console.WriteLine("8. Avg Walking Speed");
                Console.WriteLine("9. Max Delivery Time");
                Console.WriteLine("10. Risk Range");
                Console.WriteLine("11. Inactivity Range");
                Console.WriteLine("12. Clock");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int option)) return;

                switch (option)
                {
                    case 1:
                        Console.Write("Enter new Manager ID: ");
                        s_dalConfig!.ManagerId = int.Parse(Console.ReadLine()!);
                        break;
                    case 2:
                        Console.Write("Enter new Manager Password: ");
                        s_dalConfig!.ManagerPassword = Console.ReadLine()!;
                        break;
                    case 3:
                        Console.Write("Enter new Company Address: ");
                        s_dalConfig!.CompanyAddress = Console.ReadLine();
                        break;
                    case 4:
                        Console.Write("Enter new Max Range: ");
                        s_dalConfig!.MaxRange = int.Parse(Console.ReadLine()!);
                        break;
                    case 5:
                        Console.Write("Enter new Avg Car Speed: ");
                        s_dalConfig!.AvgCarSpeed = double.Parse(Console.ReadLine()!);
                        break;
                    case 6:
                        Console.Write("Enter new Avg Motorbike Speed: ");
                        s_dalConfig!.AvgMotorbikeSpeed = double.Parse(Console.ReadLine()!);
                        break;
                    case 7:
                        Console.Write("Enter new Avg Bicycle Speed: ");
                        s_dalConfig!.AvgBicycleSpeed = double.Parse(Console.ReadLine()!);
                        break;
                    case 8:
                        Console.Write("Enter new Avg Walking Speed: ");
                        s_dalConfig!.AvgWalkingSpeed = double.Parse(Console.ReadLine()!);
                        break;
                    case 9:
                        Console.Write("Enter new Max Delivery Time: ");
                        s_dalConfig!.MaxDeliveryTime = TimeSpan.Parse(Console.ReadLine()!);
                        break;
                    case 10:
                        Console.Write("Enter new Risk Range: ");
                        s_dalConfig!.RiskRange = TimeSpan.Parse(Console.ReadLine()!);
                        break;
                    case 11:
                        Console.Write("Enter new Inactivity Range: ");
                        s_dalConfig!.InactivityRange = TimeSpan.Parse(Console.ReadLine()!);
                        break;
                    case 12:
                        Console.Write("Enter new Clock (hh:mm:ss): ");
                        s_dalConfig!.Clock = DateTime.Parse(Console.ReadLine()!);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        // -------------------- SHOW ALL DATA --------------------

        /// <summary>
        /// Dump all entities to the console for inspection.
        /// Useful after initialization to verify the seeded data.
        /// </summary>
        private static void ShowAllData()
        {
            try
            {
                Console.WriteLine("\n--- All Data ---");
                foreach (var c in s_dalCourier!.ReadAll()) Console.WriteLine(c);
                foreach (var o in s_dalOrder!.ReadAll()) Console.WriteLine(o);
                foreach (var d in s_dalDelivery!.ReadAll()) Console.WriteLine(d);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        // -------------------- RESET DATABASE --------------------

        /// <summary>
        /// Reset the in-memory data store by deleting all entities and resetting configuration.
        /// Intended for test/demo convenience.
        /// </summary>
        private static void ResetDatabase()
        {
            try
            {
                s_dalCourier!.DeleteAll();
                s_dalOrder!.DeleteAll();
                s_dalDelivery!.DeleteAll();
                s_dalConfig!.Reset();
                Console.WriteLine("Database reset completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught: {ex.Message}");
            }
        }

    }
}
//////////////////////////////////////////////////////////////////

