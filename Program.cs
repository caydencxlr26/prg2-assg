using Microsoft.VisualBasic;
using prg2_assg;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Runtime.CompilerServices;

Dictionary<string, Restaurant> restaurantlist = new Dictionary<string, Restaurant>();
Dictionary<string, FoodItem> fooditemlist = new Dictionary<string, FoodItem>();
Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
LoadData();

int currentOrderId = 1000;
Dictionary<string, Customer> customerList = [];
LoadCustomers();
LoadOrders(ref currentOrderId);

Console.WriteLine("");
while (true)
{
    // Main menu
    Console.WriteLine("===== Gruberoo Food Delivery System =====");
    Console.WriteLine("1. List all restaurants and menu items");
    Console.WriteLine("2. List all orders");
    Console.WriteLine("3. Create a new order");
    Console.WriteLine("4. Process an order");
    Console.WriteLine("5. Modify an existing order");
    Console.WriteLine("6. Delete an existing order");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: "); string choice = Console.ReadLine() ?? "";

    if (choice == "1")
    {
        ListRestaurants();
    }
    else if (choice == "3")
    {
        CreateNewOrder(ref currentOrderId);
    }
    else if (choice == "5")
    {
        ModifyOrder();
    }
    else if (choice == "0")
    {
        break;
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }
}

// Data validation methods
string ValidateNonEmptyInput(string message, string errorMessage = "Input must not be empty.")
{
    while (true)
    {
        Console.Write(message); string input = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine(errorMessage);
        }
        else
        {
            return input;
        }
    }
}
string ValidateInputInList(string message, IEnumerable<string> list, string errorMessage = "Input not found in list.")
{
    while (true)
    {
        Console.Write(message); string input = Console.ReadLine() ?? string.Empty;
        if (list.Contains(input))
        {
            return input;
        }
        else
        {
            Console.WriteLine(errorMessage);
        }
    }
}
DateOnly ValidateDateOnly(string message)
{
    while (true)
    {
        Console.Write(message); string input = Console.ReadLine() ?? "";
        if (DateOnly.TryParseExact(input, "dd/MM/yyyy", out DateOnly date))
        {
            return date;
        }
        else
        {
            Console.WriteLine("Invalid date format.");
        }
    }
}
TimeOnly ValidateTimeOnly(string message)
{
    while (true)
    {
        Console.Write(message); string input = Console.ReadLine() ?? "";
        if (TimeOnly.TryParseExact(input, "HH:mm", out TimeOnly time))
        {
            return time;
        }
        else
        {
            Console.WriteLine("Invalid time format.");
        }
    }
}
int ValidateInt(string message, string errorMessage = "Invalid input.", float min = 0, float max = float.PositiveInfinity)
{
    while (true)
    {
        try
        {
            Console.Write(message); int number = Convert.ToInt16(Console.ReadLine());
            if (number >= min && number <= max) {
                return number;
            }
            else
            {
                Console.WriteLine("Input out of bounds.");
            }
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid input.");
        }
    }
}

void LoadData()
{
    using (StreamReader sr = new StreamReader("restaurants.csv"))
    {
        string? line;
        sr.ReadLine();
        int total = 0;
        while((line=sr.ReadLine())!= null)
        {
            string[] vals = line.Split(",");
            Restaurant r = new Restaurant(vals[0], vals[1], vals[2]);
            total += 1;

            Menu menu = new("M001", "Main Menu");
            r.AddMenu(menu);

            restaurantlist[vals[0]] = r;
        }
        Console.WriteLine(total + " restaurants loaded!");
        sr.Close();
    }
    using (StreamReader sr = new StreamReader("fooditems.csv"))
    {
        string? line;
        sr.ReadLine(); 
        int total = 0;
        while ((line=sr.ReadLine())!= null)
        {
            string[] vals = line.Split(",");
            FoodItem fi = new FoodItem(vals[1], vals[2],Convert.ToDouble(vals[3]),"");
            total += 1;
            fooditemlist[vals[1]] = fi;

            string restaurantId = vals[0];
            Restaurant restaurant = restaurantlist[restaurantId];
            restaurant.RestaurantMenus[0].AddFoodItem(fi);
        }
        Console.WriteLine(total + " food items loaded!");
        sr.Close();
    }
}
void LoadCustomers()
{
    string[] lines = File.ReadAllLines("customers.csv")[1..];
    foreach (string line in lines)
    {
        if (string.IsNullOrEmpty(line)) {
            continue;
        }
        string[] split_line = line.Split(",");

        string name = split_line[0];
        string email = split_line[1];

        Customer c = new(email, name);
        customerList[email] = c;
    }
    Console.WriteLine($"{customerList.Count} customers loaded!");
}
void LoadOrders(ref int currentOrderId)
{
    int total = 0;

    string[] lines = File.ReadAllLines("orders.csv")[1..];
    foreach (string line in lines)
    {
        if (string.IsNullOrEmpty(line))
        {
            continue;
        }
        string[] split_line = line.Split(",", 10);

        int orderId = Convert.ToInt16(split_line[0]);
        string email = split_line[1];
        string restaurantId = split_line[2];
        DateTime deliveryDateTime = Convert.ToDateTime($"{split_line[3]} {split_line[4]}");
        string deliveryAddress = split_line[5];
        DateTime createdDateTime = Convert.ToDateTime(split_line[6]);
        double totalAmount = Convert.ToDouble(split_line[7]);
        string status = split_line[8];
        Order order = new(orderId, createdDateTime, totalAmount, status, deliveryDateTime, deliveryAddress, "", true);

        string[] foods = split_line[9].Trim('"').Split('|');
        foreach (string food in foods)
        {
            string[] split_food = food.Split(',');

            string foodName = split_food[0];
            int quantity = Convert.ToInt16(split_food[1]);

            FoodItem foodItem = fooditemlist[foodName];
            OrderedFoodItem orderedFoodItem = new(
                foodItem.ItemName,
                foodItem.ItemDesc,
                foodItem.ItemPrice,
                foodItem.Customise,
                quantity,
                0
            );
            orderedFoodItem.CalculateSubtotal();

            order.AddOrderedFoodItem(orderedFoodItem);
        }

        customerList[email].AddOrder(order);
        restaurantlist[restaurantId].RestaurantOrders.Enqueue(order);
        total += 1;
        currentOrderId += 1;
    }
    Console.WriteLine($"{total} orders loaded!");
}

void ListRestaurants()
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");
    foreach (Restaurant restaurant in restaurantlist.Values)
    {
        Console.WriteLine(restaurant.ToString());
        restaurant.DisplayMenu();
        Console.WriteLine("");
    }
}

void CreateNewOrder(ref int currentOrderId)
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");
    string email = ValidateNonEmptyInput("Enter Customer Email: ");
    string restaurantId = ValidateInputInList("Enter Restaurant ID: ", restaurantlist.Keys, errorMessage: "Restaurant not found.");

    DateOnly deliveryDate = ValidateDateOnly("Enter Delivery Date (dd/mm/yyyy): ");
    TimeOnly deliveryTime = ValidateTimeOnly("Enter Delivery Time (hh:mm): ");
    DateTime deliveryDateTime = deliveryDate.ToDateTime(deliveryTime);

    string deliveryAddress = ValidateNonEmptyInput("Enter Delivery Address: ");

    Order order = new(++currentOrderId, DateTime.Now, 0, "", deliveryDateTime, deliveryAddress, "", false);

    Restaurant restaurant = restaurantlist[restaurantId];
    List<FoodItem> restaurantFoodList = [];
    foreach (Menu menu in restaurant.RestaurantMenus)
    {
        foreach (FoodItem foodItem in menu.FoodItemList)
        {
            if (!restaurantFoodList.Contains(foodItem))
            {
                restaurantFoodList.Add(foodItem);
            }
        }
    }

    Console.WriteLine("\nAvailable Food Items: ");
    int count = 0;
    foreach (FoodItem foodItem1 in restaurantFoodList)
    {
        count++;
        Console.WriteLine($"{count}: {foodItem1.ItemName} - ${foodItem1.ItemPrice:F2}");
    }

    string itemsString = "\"";
    while (true)
    {
        int itemNumber = ValidateInt("Enter item number (0 to finish): ", "Invalid item number.", max: count);
        if (itemNumber == 0)
        {
            break;
        }
        FoodItem foodItem = restaurantFoodList[itemNumber - 1];
        int quantity = ValidateInt("Enter quantity: ", min: 1);
        while (true)
        {
            Console.Write("Add special request? [Y/N]: "); string input = Console.ReadLine() ?? "";
            if (input == "Y")
            {
                string specialRequest = ValidateNonEmptyInput("Enter special request: ");
                Console.WriteLine("");
                foodItem.Customise = specialRequest;
                break;
            }
            else if (input == "N")
            {
                break;
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
        OrderedFoodItem orderedFoodItem = new(foodItem.ItemName, foodItem.ItemDesc, foodItem.ItemPrice, foodItem.Customise, quantity, 0);
        order.AddOrderedFoodItem(orderedFoodItem);
        if (itemsString == "\"")
        {
            itemsString += $"{foodItem.ItemName},{quantity}";
        }
        else
        {
            itemsString += $"|{foodItem.ItemName},{quantity}";
        }
    }
    itemsString += "\"";

    double orderTotal = order.CalculateOrderTotal();
    Console.WriteLine($"\nOrder Total: ${orderTotal:F2} + $5.00 (delivery) = ${orderTotal+5:F2}");
    while (true)
    {
        Console.Write("Proceed to payment? [Y/N]: "); string input = Console.ReadLine() ?? "";
        if (input == "Y")
        {
            Console.WriteLine("\nPayment method:");
            string paymentMethod = ValidateInputInList("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ", ["CC", "PP", "CD"], "Invalid payment method.");
            order.OrderPaymentMethod = paymentMethod;
            order.OrderPaid = true;
            break;
        }
        else if (input == "N")
        {
            Console.Write("Payment cancelled, press Enter to terminate"); string? _ = Console.ReadLine();
            return;
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }
    order.OrderStatus = "Pending";
        customerList[email].AddOrder(order);
        restaurant.RestaurantOrders.Enqueue(order);
        using (StreamWriter sw = File.AppendText("orders.csv"))
        {
            sw.WriteLine($"{currentOrderId},{email},{restaurantId},{deliveryDate:dd/MM/yyyy},{deliveryTime:HH:mm},{deliveryAddress},{order.OrderDateTime:dd/MM/yyyy HH:mm},{orderTotal},{order.OrderStatus},{itemsString}");
        }
        Console.WriteLine($"\nOrder {order.OrderId} created successfully! Status: Pending");
}

void ModifyOrder()
{
    Console.WriteLine("Modify Order");
    Console.WriteLine("============");

    Customer? customer = null;
    while (true)
    {
        string email = ValidateNonEmptyInput("Enter Customer Email: ");
        if (customerList.ContainsKey(email))
        {
            customer = customerList[email];
            break;
        }
        Console.WriteLine("Email not found.");
    }

    Dictionary<int, Order> orders = customer.CustomerOrders.Where(kvp => kvp.Value.OrderStatus == "Pending").ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    if (orders.Count == 0)
    {
        Console.Write("No pending orders found, press Enter to terminate"); string? _ = Console.ReadLine();
        return;
    }
    Console.WriteLine("Pending orders");
    foreach (Order order1 in orders.Values)
    {
        Console.WriteLine(order1.OrderId);
    }

    int orderId = 0;
    while (true)
    {
        orderId = ValidateInt("Enter Order ID: ");
        if (orders.TryGetValue(orderId, out Order? order1) && order1.OrderStatus == "Pending")
        {
            break;
        }
        Console.WriteLine("Order not found.");
    }
    Order order = orders[orderId];
    Order oldOrder = order;

    int count = 0;
    Console.WriteLine("Order Items:");
    foreach (OrderedFoodItem orderedFoodItem in order.OrderedFoodItemList)
    {
        Console.WriteLine($"{++count}. {orderedFoodItem.ItemName} - {orderedFoodItem.QtyOrdered}");
    }
    Console.WriteLine("Address:");
    Console.WriteLine(order.DeliveryAddress);
    Console.WriteLine("Delivery Date/Time:");
    Console.WriteLine(order.DeliveryDateTime);

    Console.WriteLine("");

    Restaurant? restaurant = null;
    foreach (Restaurant restaurant1 in restaurantlist.Values)
    {
        if (restaurant1.RestaurantOrders.Contains(order))
        {
            restaurant = restaurant1;
            break;
        }
    }
    if (restaurant == null) // Checks if none of the restaurants match, also serves as assurance that restaurant exists for the compiler
    {
        Console.WriteLine("Error: No restaurant found with order");
        return;
    }

    int choice = ValidateInt("Modify: [1] Items [2] Address [3] Delivery Time: ", "Invalid choice.", 1, 3);
    if (choice == 1)
    {
        while (true)
        {
            choice = ValidateInt("[1] Delete [2] Append [3] Exit: ", "Invalid choice.", 1, 3);
            if (choice == 1)
            {
                choice = ValidateInt("Enter index of food item to remove: ", "Invalid choice.", 1, count);
                OrderedFoodItem orderedFoodItem = order.OrderedFoodItemList[choice - 1];
                order.RemoveOrderedFoodItem(orderedFoodItem);
                Console.WriteLine("Successfully removed food from order.");
            }
            else if (choice == 2)
            {
                List<FoodItem> restaurantFoodList = [];
                foreach (Menu menu in restaurant.RestaurantMenus)
                {
                    foreach (FoodItem foodItem in menu.FoodItemList)
                    {
                        if (!restaurantFoodList.Contains(foodItem))
                        {
                            restaurantFoodList.Add(foodItem);
                        }
                    }
                }

                Console.WriteLine("\nAvailable Food Items: ");
                count = 0;
                foreach (FoodItem foodItem1 in restaurantFoodList)
                {
                    count++;
                    Console.WriteLine($"{count}: {foodItem1.ItemName} - ${foodItem1.ItemPrice:F2}");
                }

                while (true)
                {
                    int itemNumber = ValidateInt("Enter item number (0 to finish): ", "Invalid item number.", max: count);
                    if (itemNumber == 0)
                    {
                        break;
                    }
                    FoodItem foodItem = restaurantFoodList[itemNumber - 1];
                    int quantity = ValidateInt("Enter quantity: ", min: 1);
                    while (true)
                    {
                        Console.Write("Add special request? [Y/N]: "); string input = Console.ReadLine() ?? "";
                        if (input == "Y")
                        {
                            string specialRequest = ValidateNonEmptyInput("Enter special request: ");
                            Console.WriteLine("");
                            foodItem.Customise = specialRequest;
                            break;
                        }
                        else if (input == "N")
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input.");
                        }
                    }
                    OrderedFoodItem orderedFoodItem = new(foodItem.ItemName, foodItem.ItemDesc, foodItem.ItemPrice, foodItem.Customise, quantity, 0);
                    order.AddOrderedFoodItem(orderedFoodItem);
                }
            }
            else if (choice == 3)
            {
                string itemsString = "\"";
                foreach (OrderedFoodItem orderedFoodItem1 in order.OrderedFoodItemList)
                {
                    if (itemsString == "\"")
                    {
                        itemsString += $"{orderedFoodItem1.ItemName},{orderedFoodItem1.QtyOrdered}";
                    }
                    else
                    {
                        itemsString += $"|{orderedFoodItem1.ItemName},{orderedFoodItem1.QtyOrdered}";
                    }
                }
                itemsString += "\"";

                double oldOrderTotal = order.OrderTotal;
                double orderTotal = order.CalculateOrderTotal();
                Console.WriteLine($"\nNew Order Total: ${orderTotal:F2} + $5.00 (delivery) = ${orderTotal+5:F2} ({(orderTotal >= oldOrderTotal ? "+" : "-")}${Math.Abs(orderTotal-oldOrderTotal):F2})");
                Console.WriteLine($"{(orderTotal >- oldOrderTotal ? "Payment Amount" : "Refund Amount")}: ${Math.Abs(orderTotal-oldOrderTotal):F2}");
                if (orderTotal > oldOrderTotal)
                {
                    while (true)
                    {
                        Console.Write("Proceed to payment? [Y/N]: "); string input = Console.ReadLine() ?? "";
                        if (input == "Y")
                        {
                            Console.WriteLine("\nPayment method:");
                            string paymentMethod = ValidateInputInList("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ", ["CC", "PP", "CD"], "Invalid payment method.");
                            order.OrderPaymentMethod = paymentMethod;
                            order.OrderPaid = true;
                            break;
                        }
                        else if (input == "N")
                        {
                            Console.Write("Payment cancelled, press Enter to terminate"); string? _ = Console.ReadLine();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input.");
                        }
                    }
                };

                restaurant.RemoveOrder(oldOrder);
                restaurant.AddOrder(order);
                customer.RemoveOrder(oldOrder);
                customer.AddOrder(order);

                string[] lines = File.ReadAllLines("orders.csv");
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(order.OrderId.ToString()))
                    {
                        string[] split = lines[i].Split(',',10);
                        lines[i] = $"{split[0]},{split[1]},{split[2]},{split[3]},{split[4]},{split[5]},{split[6]},{order.OrderTotal},{split[8]},{itemsString}";
                    }
                }
                File.WriteAllLines("orders.csv", lines);
                
                Console.WriteLine($"\nOrder {order.OrderId} updated. New Amount: ${order.OrderTotal:F2}");
                break;
            }
        }
    }
    else if (choice == 2)
    {
        string address = ValidateNonEmptyInput("Enter new Address: ");
        order.DeliveryAddress = address;

        restaurant.RemoveOrder(oldOrder);
        restaurant.AddOrder(order);
        customer.RemoveOrder(oldOrder);
        customer.AddOrder(order);

        string[] lines = File.ReadAllLines("orders.csv");
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith(order.OrderId.ToString()))
            {
                string[] split = lines[i].Split(',',10);
                lines[i] = $"{split[0]},{split[1]},{split[2]},{split[3]},{split[4]},{address},{split[6]},{split[7]},{split[8]},{split[9]}";
            }
        }
        File.WriteAllLines("orders.csv", lines);
        
        Console.WriteLine($"\nOrder {order.OrderId} updated. New Address: {address}");
    }
    else if (choice == 3)
    {
        TimeOnly time = ValidateTimeOnly("Enter new Delivery Time (hh:mm): ");
        order.DeliveryDateTime = DateOnly.FromDateTime(order.DeliveryDateTime).ToDateTime(time);

        restaurant.RemoveOrder(oldOrder);
        restaurant.AddOrder(order);
        customer.RemoveOrder(oldOrder);
        customer.AddOrder(order);

        string[] lines = File.ReadAllLines("orders.csv");
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith(order.OrderId.ToString()))
            {
                string[] split = lines[i].Split(',',10);
                lines[i] = $"{split[0]},{split[1]},{split[2]},{split[3]},{time:HH:mm},{split[5]},{split[6]},{split[7]},{split[8]},{split[9]}";
            }
        }
        File.WriteAllLines("orders.csv", lines);
        
        Console.WriteLine($"\nOrder {order.OrderId} updated. New Delivery Time: {time:HH:mm}");
    }
}