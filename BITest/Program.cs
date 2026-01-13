using System;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using BO;

namespace BlTest
{
    /// <summary>
    /// Simple console test harness for the BL layer.
    /// Provides interactive menus for admin, order and courier operations using the <see cref="BlApi.IBl"/> facade.
    /// </summary>
    internal class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static int s_currentUserId = 0;

        /// <summary>
        /// Application entry point. Initializes current user id and drives the main menu loop.
        /// Catches and prints BL exceptions to the console.
        /// </summary>
        static void Main(string[] args)
        {
            bool exitRequested = false;

            Console.Write("Enter your User ID (0 for Admin): ");
            if (int.TryParse(Console.ReadLine(), out int userId))
            {
                s_currentUserId = userId;
            }


            while (!exitRequested)
            {
                Console.WriteLine("=== BL Test Main Menu ===");
                Console.WriteLine("1. Admin Management");
                Console.WriteLine("2. Order Management");
                Console.WriteLine("3. Courier Management");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");
                try
                {
                    string? input = Console.ReadLine();
                    switch (input)
                    {
                        case "1":
                            ShowAdminMenu();
                            break;
                        case "2":
                            ShowOrderMenu();
                            break;
                        case "3":
                            ShowCourierMenu();
                            break;
                        case "0":
                            exitRequested = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                catch (BlDoesNotExistException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BlAlreadyExistsException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BlInvalidValueException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BlInvalidDateException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BlNullPropertyException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BlUnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BlInvalidAddressException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BlInvalidRequestException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (BLTemporaryNotAvailableException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        // =============== ADMIN MENU ===============
        /// <summary>
        /// Displays the admin submenu and routes admin actions.
        /// </summary>
        static void ShowAdminMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n=== Admin Management ===");
                Console.WriteLine("1. Initialize Database");
                Console.WriteLine("2. Reset Database");
                Console.WriteLine("3. Get Current Clock");
                Console.WriteLine("4. Forward Clock");
                Console.WriteLine("5. Get Configuration");
                Console.WriteLine("6. Set Configuration");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");
                string? input = Console.ReadLine();

                try
                {
                    switch (input)
                    {
                        case "1":
                            InitializeDatabase();
                            break;
                        case "2":
                            ResetDatabase();
                            break;
                        case "3":
                            GetClock();
                            break;
                        case "4":
                            ForwardClock();
                            break;
                        case "5":
                            GetConfiguration();
                            break;
                        case "6":
                            SetConfiguration();
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Initialize demo database (user confirmation required).
        /// </summary>
        static void InitializeDatabase()
        {
            Console.Write("Are you sure you want to initialize the database? (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                s_bl.Admin.InitializeDB();
                Console.WriteLine("Database initialized successfully.");
            }
        }

        /// <summary>
        /// Reset database to defaults (user confirmation required).
        /// </summary>
        static void ResetDatabase()
        {
            Console.Write("Are you sure you want to reset the database? This will delete all data! (y/n): ");
            if (Console.ReadLine()?.ToLower() == "y")
            {
                s_bl.Admin.ResetDB();
                Console.WriteLine("Database reset successfully.");
            }
        }

        /// <summary>
        /// Prints the current system clock provided by the BL admin facade.
        /// </summary>
        static void GetClock()
        {
            var clock = s_bl.Admin.GetClock();
            Console.WriteLine($"Current system clock: {clock}");
        }

        /// <summary>
        /// Prompts the user for a time unit and forwards the system clock by one unit.
        /// </summary>
        static void ForwardClock()
        {
            Console.WriteLine("Select time unit to forward:");
            Console.WriteLine("1. Minute");
            Console.WriteLine("2. Hour");
            Console.WriteLine("3. Day");
            Console.Write("Select: ");

            string? input = Console.ReadLine();
            TimeUnit unit = input switch
            {
                "1" => TimeUnit.Minute,
                "2" => TimeUnit.Hour,
                "3" => TimeUnit.Day,
                _ => TimeUnit.Minute
            };

            s_bl.Admin.ForwardClock(unit);
            Console.WriteLine($"Clock forwarded by one {unit}.");
            Console.WriteLine($"New time: {s_bl.Admin.GetClock()}");
        }

        /// <summary>
        /// Displays current configuration values from the BL admin facade.
        /// </summary>
        static async Task GetConfiguration()
        {
            var config =await s_bl.Admin.GetConfig();
            Console.WriteLine("\n=== Current Configuration ===");
            Console.WriteLine($"Clock: {config.Clock}");
            Console.WriteLine($"Manager ID: {config.ManagerId}");
            Console.WriteLine($"Manager Password: {config.ManagerPassword}");
            Console.WriteLine($"Company Address: {config.CompanyAddress}");
            Console.WriteLine($"Location: Lat={config.Latitude}, Lon={config.Longitude}");
            Console.WriteLine($"Max Range: {config.MaxRange} km");
            Console.WriteLine($"Avg Car Speed: {config.AvgCarSpeed} km/h");
            Console.WriteLine($"Avg Motorbike Speed: {config.AvgMotorBikeSpeed} km/h");
            Console.WriteLine($"Avg Bicycle Speed: {config.AvgBicycleSpeed} km/h");
            Console.WriteLine($"Avg Walking Speed: {config.AvgWalkingSpeed} km/h");
            Console.WriteLine($"Max Delivery Time: {config.MaxDeliveryTime}");
            Console.WriteLine($"Risk Range: {config.RiskRange}");
            Console.WriteLine($"Inactivity Range: {config.InactivityRange}");
        }

        /// <summary>
        /// Prompts the user and applies configuration updates via BL admin facade.
        /// Press Enter to keep current values.
        /// </summary>
        static async Task SetConfiguration()
        {
            var currentConfig = await s_bl.Admin.GetConfig();
            Console.WriteLine("\n=== Set New Configuration ===");
            Console.WriteLine("(Press Enter to keep current value)");

            Console.Write($"Manager ID [{currentConfig.ManagerId}]: ");
            string? input = Console.ReadLine();
            int managerId = string.IsNullOrWhiteSpace(input) ? currentConfig.ManagerId : int.Parse(input);

            Console.Write($"Manager Password: ");
            input = Console.ReadLine();
            string managerPassword = string.IsNullOrWhiteSpace(input) ? currentConfig.ManagerPassword : input;

            Console.Write($"Company Address [{currentConfig.CompanyAddress}]: ");
            input = Console.ReadLine();
            string? companyAddress = string.IsNullOrWhiteSpace(input) ? currentConfig.CompanyAddress : input;

            input = Console.ReadLine();
            double? longitude = string.IsNullOrWhiteSpace(input) ? currentConfig.Longitude : double.Parse(input);

            Console.Write($"Max Range [{currentConfig.MaxRange}]: ");
            input = Console.ReadLine();
            int maxRange = string.IsNullOrWhiteSpace(input) ? currentConfig.MaxRange : int.Parse(input);

            Console.Write($"Avg Car Speed [{currentConfig.AvgCarSpeed}]: ");
            input = Console.ReadLine();
            double avgCarSpeed = string.IsNullOrWhiteSpace(input) ? currentConfig.AvgCarSpeed : double.Parse(input);

            Console.Write($"Avg Motorbike Speed [{currentConfig.AvgMotorBikeSpeed}]: ");
            input = Console.ReadLine();
            double avgMotorbikeSpeed = string.IsNullOrWhiteSpace(input) ? currentConfig.AvgMotorBikeSpeed : double.Parse(input);

            Console.Write($"Avg Bicycle Speed [{currentConfig.AvgBicycleSpeed}]: ");
            input = Console.ReadLine();
            double avgBicycleSpeed = string.IsNullOrWhiteSpace(input) ? currentConfig.AvgBicycleSpeed : double.Parse(input);

            Console.Write($"Avg Walking Speed [{currentConfig.AvgWalkingSpeed}]: ");
            input = Console.ReadLine();
            double avgWalkingSpeed = string.IsNullOrWhiteSpace(input) ? currentConfig.AvgWalkingSpeed : double.Parse(input);

            Console.Write($"Max Delivery Time in minutes [{currentConfig.MaxDeliveryTime.TotalMinutes}]: ");
            input = Console.ReadLine();
            TimeSpan maxDeliveryTime = string.IsNullOrWhiteSpace(input) ? currentConfig.MaxDeliveryTime : TimeSpan.FromMinutes(double.Parse(input));

            Console.Write($"Risk Range in minutes [{currentConfig.RiskRange.TotalMinutes}]: ");
            input = Console.ReadLine();
            TimeSpan riskRange = string.IsNullOrWhiteSpace(input) ? currentConfig.RiskRange : TimeSpan.FromMinutes(double.Parse(input));

            Console.Write($"Inactivity Range in minutes [{currentConfig.InactivityRange.TotalMinutes}]: ");
            input = Console.ReadLine();
            TimeSpan inactivityRange = string.IsNullOrWhiteSpace(input) ? currentConfig.InactivityRange : TimeSpan.FromMinutes(double.Parse(input));

            var newConfig = new Config
            {
                Clock = currentConfig.Clock,
                ManagerId = managerId,
                ManagerPassword = managerPassword,
                CompanyAddress = companyAddress,
                Latitude = 0,
                Longitude = 0,
                MaxRange = maxRange,
                AvgCarSpeed = avgCarSpeed,
                AvgMotorBikeSpeed = avgMotorbikeSpeed,
                AvgBicycleSpeed = avgBicycleSpeed,
                AvgWalkingSpeed = avgWalkingSpeed,
                MaxDeliveryTime = maxDeliveryTime,
                RiskRange = riskRange,
                InactivityRange = inactivityRange
            };

            s_bl.Admin.SetConfig(newConfig);
            Console.WriteLine("Configuration updated successfully.");
        }

        // =============== ORDER MENU ===============
        /// <summary>
        /// Displays the order submenu and routes order-related actions.
        /// </summary>
        static void ShowOrderMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n=== Order Management ===");
                Console.WriteLine("1. Add New Order");
                Console.WriteLine("2. Get Order Details");
                Console.WriteLine("3. Get Orders List");
                Console.WriteLine("4. Get Orders Summary");
                Console.WriteLine("5. Update Order");
                Console.WriteLine("6. Cancel Order");
                Console.WriteLine("7. Delete Order");
                Console.WriteLine("8. Choose Order for Delivery");
                Console.WriteLine("9. Complete Order Delivery");
                Console.WriteLine("10. Get Open Orders for Courier");
                Console.WriteLine("11. Get Closed Deliveries");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");
                string? input = Console.ReadLine();
                try
                {
                    switch (input)
                    {
                        case "1":
                            AddOrder();
                            break;
                        case "2":
                            GetOrder();
                            break;
                        case "3":
                            GetOrdersList();
                            break;
                        case "4":
                            GetOrdersSummary();
                            break;
                        case "5":
                            UpdateOrder();
                            break;
                        case "6":
                            CancelOrder();
                            break;
                        case "7":
                            DeleteOrder();
                            break;
                        case "8":
                            ChooseOrderForDelivery();
                            break;
                        case "9":
                            CompleteOrderDelivery();
                            break;
                        case "10":
                            GetOpenOrdersForCourier();
                            break;
                        case "11":
                            GetClosedDeliveries();
                            break;
                        case "0":
                            back = true;
                            break;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Prompts for Order ID then calls BL to delete order (DeleteOrder implementation throws in BL).
        /// </summary>
        static void DeleteOrder()
        {
            Console.Write("Enter Order ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int orderId))
            {
                s_bl.Order.DeleteOrder(s_currentUserId, orderId);
            }
        }

        /// <summary>
        /// Collects order details from console and submits a new BO.Order via BL facade.
        /// </summary>
        static void AddOrder()
        {

            Console.WriteLine("\n=== Add New Order ===");

            Console.Write("Customer Name: ");
            string customerName = Console.ReadLine() ?? "";

            Console.Write("Customer Phone: ");
            string customerPhone = Console.ReadLine() ?? "";

            Console.Write("Address: ");
            string Address = Console.ReadLine() ?? "";

            Console.Write("OrderType: ");
            OrderType type = (OrderType)int.Parse(Console.ReadLine() ?? "");

            Console.Write("Verbaldescription: ");
            string? Verbaldescription = Console.ReadLine();

            Console.Write("Weight (kg): ");
            double weight = double.Parse(Console.ReadLine() ?? "0");

            var order = new BO.Order
            {
                CustomerName = customerName,
                CustomerPhone = customerPhone,
                FullAddress = Address,
                Weight = weight,
                Description = Verbaldescription!,
                Type = type
            };

            s_bl.Order.AddOrder(s_currentUserId, order);
        }

        /// <summary>
        /// Prompts for an order id and displays the order details retrieved from BL.
        /// </summary>
        static void GetOrder()
        {
            Console.Write("Enter Order ID: ");
            if (int.TryParse(Console.ReadLine(), out int orderId))
            {
                var order = s_bl.Order.GetOrder(s_currentUserId, orderId);
            }
        }

        /// <summary>
        /// Shows interactive filter/sort options and prints a list of orders returned by BL.
        /// </summary>
        static void GetOrdersList()
        {
            Console.WriteLine("\n=== Filter Options ===");
            Console.WriteLine("1. Type");
            Console.WriteLine("2. OrderStatus");
            Console.WriteLine("3. ScheduleStatus");
            Console.Write("Select: ");
            string? filterChoice = Console.ReadLine();

            OrderInListFilter? filterType = null;
            object? filterValue = null;

            switch (filterChoice)
            {
                case "1":
                    filterType = OrderInListFilter.Type;
                    Console.Write("Enter Type value: ");
                    filterValue = Console.ReadLine() ?? "";
                    break;
                case "2":
                    filterType = OrderInListFilter.OrderStatus;
                    Console.Write("Enter OrderStatus value: ");
                    filterValue = Console.ReadLine() ?? "";
                    break;
                case "3":
                    filterType = OrderInListFilter.ScheduleStatus;
                    Console.Write("Enter ScheduleStatus value: ");
                    filterValue = Console.ReadLine() ?? "";
                    break;
            }

            Console.WriteLine("\n=== Sort Options ===");
            Console.WriteLine("1. OrderId");
            Console.WriteLine("2. AirDistance");
            Console.WriteLine("3. TimeRemaining");
            Console.WriteLine("4. TotalCompletionTime");
            Console.WriteLine("5. TotalDeliveries");
            Console.Write("Select: ");
            string? sortChoice = Console.ReadLine();

            OrderInListSort sortBy = sortChoice switch
            {
                "2" => OrderInListSort.AirDistance,
                "3" => OrderInListSort.TimeRemaining,
                "4" => OrderInListSort.TotalCompletionTime,
                "5" => OrderInListSort.TotalDeliveries,
                _ => OrderInListSort.OrderId
            };

            var orders = s_bl.Order.GetOrdersList(s_currentUserId, filterType, filterValue, sortBy);

            Console.WriteLine("\n=== Orders List ===");
            foreach (var order in orders)
            {
                Console.WriteLine(order);
            }
        }
        /// <summary>
        /// Prints aggregated orders summary retrieved from BL.
        /// </summary>
        static void GetOrdersSummary()
        {
            var summary = s_bl.Order.GetOrdersSummary(s_currentUserId);
            Console.WriteLine("\n=== Orders Summary ===");
            Console.WriteLine($"Open: {summary[0]}");
            Console.WriteLine($"InProgress: {summary[1]}");
            Console.WriteLine($"Delivered: {summary[2]}");
            Console.WriteLine($"Refused: {summary[3]}");
            Console.WriteLine($"Cancelled: {summary[4]}");
            Console.WriteLine($"OnTime: {summary[5]}");
            Console.WriteLine($"InRisk: {summary[6]}");
            Console.WriteLine($"Late: {summary[7]}");
        }
        /// <summary>
        /// Prompts user to update an order and submits the update to BL.
        /// </summary>
        static void UpdateOrder()
        {
            Console.Write("Enter Order ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int orderId))
            {
                var order = s_bl.Order.GetOrder(s_currentUserId, orderId);

                Console.WriteLine("\n=== Update Fields ===");
                Console.WriteLine("(Press Enter to keep current value)");

                Console.Write($"Customer Name [{order.CustomerName}]: ");
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) order.CustomerName = input;

                Console.Write($"Customer Phone [{order.CustomerPhone}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) order.CustomerPhone = input;

                Console.Write($"Delivery Address [{order.FullAddress}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) order.FullAddress = input;

                Console.Write($"Weight [{order.Weight}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) order.Weight = double.Parse(input);

                Console.Write($"Verbaldescription [{order.Description}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) order.Description = input;

                Console.Write($"OrderType: [{order.Type}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) order.Type = (OrderType)int.Parse(input);

                s_bl.Order.UpdateOrder(s_currentUserId, order);
                Console.WriteLine("Order updated successfully!");
            }
        }
        /// <summary>
        /// Prompts for order id and cancels it via BL.
        /// </summary>
        static void CancelOrder()
        {
            Console.Write("Enter Order ID to cancel: ");
            if (int.TryParse(Console.ReadLine(), out int orderId))
            {
                s_bl.Order.CancelOrder(s_currentUserId, orderId);
                Console.WriteLine("Order cancelled successfully!");
            }
        }
        /// <summary>
        /// Prompts and assigns an order to a courier via BL.
        /// </summary>
        static void ChooseOrderForDelivery()
        {
            Console.Write("Enter Courier ID: ");
            int courierId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter Order ID: ");
            int orderId = int.Parse(Console.ReadLine() ?? "0");

            s_bl.Order.ChooseOrderForDelivery(s_currentUserId, courierId, orderId);
            Console.WriteLine("Order assigned to courier successfully!");
        }
        /// <summary>
        /// Prompts and marks a delivery complete via BL.
        /// </summary>
        static void CompleteOrderDelivery()
        {
            Console.Write("Enter Courier ID: ");
            int courierId = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter Delivery ID: ");
            int deliveryId = int.Parse(Console.ReadLine() ?? "0");
            Console.Write("Enter End Of delivery sort: ");
            DO.Endofcourier sort = (DO.Endofcourier)int.Parse(Console.ReadLine() ?? "0");

            s_bl.Order.CompleteOrderDelivery(s_currentUserId, courierId, deliveryId,sort);
            Console.WriteLine("Delivery completed successfully!");
        }
        /// <summary>
        /// Prompts for courier id and lists open orders returned from BL for that courier.
        /// </summary>
        static void GetOpenOrdersForCourier()
        {
            Console.Write("Enter Courier ID: ");
            int courierId = int.TryParse(Console.ReadLine(), out int tempId) ? tempId : 0;

            Console.WriteLine("\n=== Filter by Order Type ===");
            Console.WriteLine("1. SingleOrder");
            Console.WriteLine("2. BulkOrder");
            Console.WriteLine("3. ExpressOrder");
            Console.WriteLine("4. InternalTransfer");
            Console.Write("Select: ");
            string? filterChoice = Console.ReadLine();

            OrderType? filterType = filterChoice switch
            {
                "1" => OrderType.SingleOrder,
                "2" => OrderType.BulkOrder,
                "3" => OrderType.ExpressOrder,
                "4" => OrderType.InternalTransfer,
                _ => null
            };

            // Sort
            Console.WriteLine("\n=== Sort Options ===");
            Console.WriteLine("1. OrderId");
            Console.WriteLine("2. Weight");
            Console.WriteLine("3. AirDistance");
            Console.WriteLine("4. ActualDistance");
            Console.WriteLine("5. EstimatedActualTime");
            Console.WriteLine("6. TimeRemaining");
            Console.WriteLine("7. MaxDeliveryTime");
            Console.Write("Select: ");
            string? sortChoice = Console.ReadLine();

            OpenOrderInListSort sortBy = sortChoice switch
            {
                "1" => OpenOrderInListSort.OrderId,
                "2" => OpenOrderInListSort.Weight,
                "3" => OpenOrderInListSort.AirDistance,
                "4" => OpenOrderInListSort.ActualDistance,
                "5" => OpenOrderInListSort.EstimatedActualTime,
                "6" => OpenOrderInListSort.TimeRemaining,
                "7" => OpenOrderInListSort.MaxDeliveryTime,
                _ => OpenOrderInListSort.OrderId 
            };

            var orders = s_bl.Order.GetOpenOrdersForCourier(s_currentUserId, courierId, filterType, sortBy);

            Console.WriteLine("\n=== Open Orders ===");
            foreach (var order in orders)
            {
                Console.WriteLine(order);
            }
        }
        /// <summary>
        /// Prompts for courier id and prints closed deliveries returned from BL.
        /// </summary>
        static void GetClosedDeliveries()
        {
            Console.Write("Enter Courier ID: ");
            int courierId = int.TryParse(Console.ReadLine(), out int tempId) ? tempId : 0;

            Console.WriteLine("\n=== Filter by Order Type ===");
            Console.WriteLine("1. SingleOrder");
            Console.WriteLine("2. BulkOrder");
            Console.WriteLine("3. ExpressOrder");
            Console.WriteLine("4. InternalTransfer");
            Console.Write("Select: ");
            string? filterChoice = Console.ReadLine();

            OrderType? filterType = filterChoice switch
            {
                "1" => OrderType.SingleOrder,
                "2" => OrderType.BulkOrder,
                "3" => OrderType.ExpressOrder,
                "4" => OrderType.InternalTransfer,
                _ => null
            };

            // Sort
            Console.WriteLine("\n=== Sort Options ===");
            Console.WriteLine("1. DeliveryId");
            Console.WriteLine("2. OrderId");
            Console.WriteLine("3. ActualDistance");
            Console.WriteLine("4. CompletionTime");
            Console.Write("Select: ");
            string? sortChoice = Console.ReadLine();

            ClosedDeliveryInListSort sortBy = sortChoice switch
            {
                "2" => ClosedDeliveryInListSort.OrderId,
                "3" => ClosedDeliveryInListSort.ActualDistance,
                "4" => ClosedDeliveryInListSort.CompletionTime,
                _ => ClosedDeliveryInListSort.DeliveryId
            };

            var deliveries = s_bl.Order.GetClosedDeliveries(s_currentUserId, courierId, filterType, sortBy);

            Console.WriteLine("\n=== Closed Deliveries ===");
            foreach (var delivery in deliveries)
            {
                Console.WriteLine(delivery);
            }
        }

        // =============== COURIER MENU ===============
        /// <summary>
        /// Shows the courier submenu and routes courier operations.
        /// </summary>
        static void ShowCourierMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("\n=== Courier Management ===");
                Console.WriteLine("1. Add New Courier");
                Console.WriteLine("2. Get Courier Details");
                Console.WriteLine("3. Get Couriers List");
                Console.WriteLine("4. Update Courier");
                Console.WriteLine("5. Delete Courier");
                Console.WriteLine("6. Login as Courier");
                Console.WriteLine("0. Back to Main Menu");
                Console.Write("Select an option: ");
                string? input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        AddCourier();
                        break;
                    case "2":
                        GetCourier();
                        break;
                    case "3":
                        GetCouriersList();
                        break;
                    case "4":
                        UpdateCourier();
                        break;
                    case "5":
                        DeleteCourier();
                        break;
                    case "6":
                        LoginCourier();
                        break;
                    case "0":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }
        /// <summary>
        /// Prompts for courier id and prints courier details retrieved from BL.
        /// </summary>
        static void GetCourier()
        {
            Console.Write("Enter Courier ID: ");
            if (int.TryParse(Console.ReadLine(), out int courierId))
            {
                var courier = s_bl.Courier.GetCourier(s_currentUserId, courierId);
                Console.WriteLine(courier);
            }
        }
        /// <summary>
        /// Collects courier details from the console and submits a new courier to BL.
        /// </summary>
        static void AddCourier()
        {
            Console.WriteLine("\n=== Add New Courier ===");

            Console.Write("First Name: ");
            string? fullName = Console.ReadLine() ?? "";

            Console.Write("ID Number: ");
            int idNumber = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Phone: ");
            string? phone = Console.ReadLine() ?? "";

            Console.Write("Email: ");
            string? email = Console.ReadLine() ?? "";

            Console.Write("Username: ");
            double DistanceMaxDelivery = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Password: ");
            string? password = Console.ReadLine() ?? "";

            Console.Write("Delivery Type : ");
            int deliveryType = int.Parse(Console.ReadLine() ?? "0");

            var courier = new BO.Courier
            {
                FullName = fullName,
                Id = idNumber,
                Phone = phone,
                Email = email,
                Password = password,
                DeliveryType = (DeliveryType)deliveryType,
                StartDate = DateTime.Now,
                MaxDeliveryDistance = DistanceMaxDelivery,
                Active = true
            };

            s_bl.Courier.AddCourier(s_currentUserId, courier);

        }
        /// <summary>
        /// Interactive couriers list filter/sort UI and print.
        /// </summary>
        static void GetCouriersList()
        {
            Console.WriteLine("\n=== Filter by Active Status ===");
            Console.WriteLine("1. All");
            Console.WriteLine("2. Active Only");
            Console.WriteLine("3. Inactive Only");
            Console.Write("Select: ");
            string? filterChoice = Console.ReadLine();

            bool? isActive = filterChoice switch
            {
                "2" => true,
                "3" => false,
                _ => null
            };

            Console.WriteLine("\n=== Sort Options ===");
            Console.WriteLine("1. By ID");
            Console.WriteLine("2. By Name");
            Console.WriteLine("3. By Start Date");
            Console.WriteLine("4. By Deliveries On Time");
            Console.WriteLine("5. By Deliveries Late");
            Console.Write("Select: ");
            string? sortChoice = Console.ReadLine();

            CourierSortBy? sortBy = sortChoice switch
            {
                "2" => CourierSortBy.FullName,
                "3" => CourierSortBy.StartDate,
                "4" => CourierSortBy.DeliveredOnTime,
                "5" => CourierSortBy.DeliveredLate,
                _ => null
            };

            var couriers = s_bl.Courier.GetList(s_currentUserId, isActive, sortBy);

            Console.WriteLine("\n=== Couriers List ===");
            foreach (var courier in couriers)
            {
                Console.WriteLine(courier);
            }
        }
        /// <summary>
        /// Prompts for courier id and allows updating fields, then sends update to BL.
        /// </summary>
        static void UpdateCourier()
        {
            Console.Write("Enter Courier ID to update: ");
            if (int.TryParse(Console.ReadLine(), out int courierId))
            {
                var courier = s_bl.Courier.GetCourier(s_currentUserId, courierId);

                Console.WriteLine("\n=== Update Fields ===");
                Console.WriteLine("(Press Enter to keep current value)");

                Console.Write($"Full Name [{courier.FullName}]: ");
                string? input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) courier.FullName = input;

                Console.Write($"Phone [{courier.Phone}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) courier.Phone = input;

                Console.Write($"Email [{courier.Email}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) courier.Email = input;

                Console.Write($"password [{courier.Password}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) courier.Password = input;

                Console.Write($"MaxDeliveryDistance [{courier.MaxDeliveryDistance}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) courier.MaxDeliveryDistance = double.Parse(input);

                Console.Write($"DeliveryType [{courier.DeliveryType}]: ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) courier.DeliveryType = (DeliveryType)int.Parse(input);

                Console.Write($"Active [{courier.Active}] (y/n): ");
                input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                    courier.Active = input.ToLower() == "y";

                s_bl.Courier.UpdateCourier(s_currentUserId, courier);
                
            }
        }
        /// <summary>
        /// Prompts for courier id and deletes it (confirmation required).
        /// </summary>
        static void DeleteCourier()
        {
            Console.Write("Enter Courier ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int courierId))
            {
                Console.Write("Are you sure? (y/n): ");
                if (Console.ReadLine()?.ToLower() == "y")
                {
                    s_bl.Courier.DeleteCourier(s_currentUserId, courierId);
                    Console.WriteLine("Courier deleted successfully.");
                }
            }
        }
        /// <summary>
        /// Prompts for credentials and attempts to log in through BL.
        /// </summary>
        static void LoginCourier()
        {
            Console.Write("IdUser: ");
            int iduser = int.Parse(Console.ReadLine());

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            string result = s_bl.Courier.Login(iduser, password);
            Console.WriteLine(result);
        }
    }
}
        

