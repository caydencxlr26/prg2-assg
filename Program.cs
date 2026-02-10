//==========================================================
// Student Number : S10272352
// Student Name : Cheah Lok Weng Cayden
// Partner Name : Kayden Tan Yu Hang
//==========================================================
using S10272352_PRG2Assignment;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

Dictionary<string, Restaurant> restaurantlist = new Dictionary<string, Restaurant>();
Dictionary<string, FoodItem> fooditemlist = new Dictionary<string, FoodItem>();
Dictionary<string, Customer> customerlist = new Dictionary<string, Customer>();
Dictionary<int, Order> orderlist = new Dictionary<int, Order>();
Stack<Order> refundedorders = new Stack<Order>();
int currentOrderId = 1000;
Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
LoadData();
GetMenus();
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
    Console.WriteLine("7. Display total order amount");
    Console.WriteLine("0. Exit");
    Console.Write("Enter your choice: ");
    string choice = Console.ReadLine();
    if (choice == "1")
    {
        Console.WriteLine();
        ListRestaurants();
    }
    else if (choice == "2")
    {
        Console.WriteLine();
        ListOrders();
    }
    else if (choice == "3")
    {
        CreateNewOrder(currentOrderId);
    }
    else if (choice == "4")
    {
        ProcessOrder();
    }
    else if (choice == "5")
    {
        ModifyOrder();
    }
    else if (choice == "6")
    {
        DeleteOrder();
    }
    else if (choice == "7")
    {
        TotalOrderAmount();
    }
    else if (choice == "0") { break; }
    else
    {
        Console.WriteLine("Invalid choice.");
    }
}
using (StreamWriter sw = new StreamWriter("queue.csv"))
{
    foreach (string r in restaurantlist.Keys)
    {
        foreach (Order o in restaurantlist[r].RestaurantOrders)
        {
            sw.WriteLine(o);
        }
    }
}
using (StreamWriter sw = new StreamWriter("stack.csv"))
{
    foreach (Order o in refundedorders)
    {
        sw.WriteLine(o);
    }
}
void LoadData()
{
    using (StreamReader sr = new StreamReader("restaurants.csv"))
    {
        string? line;
        sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] vals = line.Split(",");
            Restaurant r = new Restaurant(vals[0], vals[1], vals[2]);
            restaurantlist.Add(vals[0], r);
        }
        Console.WriteLine(restaurantlist.Count + " restaurants loaded!");
    }
    using (StreamReader sr = new StreamReader("fooditems.csv"))
    {
        string? line;
        sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] vals = line.Split(",");
            string restaurantid = vals[0];
            FoodItem fi = new FoodItem(vals[1], vals[2], Convert.ToDouble(vals[3]), "");
            fooditemlist.Add(vals[1],fi);
        }
        Console.WriteLine(fooditemlist.Count + " food items loaded!");
    }
    using (StreamReader sr = new StreamReader("customers.csv"))
    {
        string? line;
        sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] vals = line.Split(",");
            Customer c = new Customer(vals[1], vals[0]);
            customerlist.Add(vals[1],c);
        }
        Console.WriteLine(customerlist.Count + " customers loaded!");
    }
    using (StreamReader sr = new StreamReader("orders.csv"))
    {
        string? line;
        sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] data = line.Split("\"");
            string[] vals = data[0].Split(",");
            int id = Convert.ToInt32(vals[0]);
            double total = Convert.ToDouble(vals[7]);
            string addr = vals[5];
            string status = vals[8];
            DateTime order = DateTime.ParseExact(vals[6], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            string temp = vals[3] + " " + vals[4];
            DateTime delivery = DateTime.ParseExact(temp, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            Order o = new Order(id, order, total, status, delivery, addr, "", true);
            orderlist.Add(id,o);
            currentOrderId = id;

            string foodstring = data[1];
            string[] foods = foodstring.Split("|");
            foreach (string food in foods)
            {   
                string[] val = food.Split(",");
                string name = val[0];
                int quantity = Convert.ToInt32(val[1]);
                FoodItem foodItem = fooditemlist[name];
                OrderedFoodItem orderedFoodItem = new(
                    foodItem.ItemName,
                    foodItem.ItemDesc,
                    foodItem.ItemPrice,
                    foodItem.Customise,
                    quantity,
                    0
                );
                orderedFoodItem.CalculateSubtotal();

                o.AddOrderedFoodItem(orderedFoodItem);

            }
            restaurantlist[vals[2]].RestaurantOrders.Enqueue(o);
            customerlist[vals[1]].CustomerOrders.Add(o);
    }
        Console.WriteLine(orderlist.Count + " orders loaded!");
    }
}
//data validation
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
            Console.Write(message); int number = Convert.ToInt32(Console.ReadLine());
            if (number >= min && number <= max)
            {
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
void GetMenus()
{
    using (StreamReader sr = new StreamReader("fooditems.csv"))
    {
        string? line;
        sr.ReadLine();
        Dictionary<string,List<FoodItem>> temprestaurants = new Dictionary<string,List<FoodItem>>();
        while ((line = sr.ReadLine()) != null)
        {
            string[] vals = line.Split(",");
            string restaurantid = vals[0];
            FoodItem fi = new FoodItem(vals[1], vals[2], Convert.ToDouble(vals[3]), "");
            List<FoodItem> tempfooditems = new List<FoodItem>();
            if (temprestaurants.Keys.Contains(restaurantid) == false)
            {
                temprestaurants[restaurantid] = tempfooditems;
            }
            temprestaurants[restaurantid].Add(fi);
        }
        foreach(string restid in temprestaurants.Keys)
        {   
            string menuid = restid.Replace("R","M");
            string menuname = restaurantlist[restid].RestaurantName + "Menu";
            Menu m = new(menuid, menuname);
            foreach (FoodItem item in temprestaurants[restid])
            {
                m.AddFoodItem(item);
            }
            restaurantlist[restid].AddMenu(m);
        }
    }
}//basic feature 3 (student 1,kayden)
void ListRestaurants()
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");
    foreach (Restaurant restaurant in restaurantlist.Values)
    {
        Console.WriteLine("Restaurant: {0} ({1})",restaurant.RestaurantName,restaurant.RestaurantId);
        foreach(FoodItem item in restaurant.RestaurantMenus[0].FoodItemList)
        {
            Console.WriteLine("  - {0}: {1} - {2:C}", item.ItemName,item.ItemDesc,item.ItemPrice);
        }
        Console.WriteLine();
    }
}//basic feature 4 (student 2,cayden)
void ListOrders() 
{
    Console.WriteLine("All Orders");
    Console.WriteLine("==========");
    Console.WriteLine("{0,-12}{1,-15}{2,-20}{3,-30}{4,-10}{5,-12}", "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
    Console.WriteLine("{0,-12}{1,-15}{2,-20}{3,-30}{4,-10}{5,-12}", "--------", "----------", "-------------", "------------------", "------", "---------");
    string customername = " ";
    string restaurantname = "";
    foreach(int order in orderlist.Keys)
    {
        foreach(string r in restaurantlist.Keys)
        {
            foreach (Order o in restaurantlist[r].RestaurantOrders)
            {
                if (o.OrderId == order)
                {
                    restaurantname = restaurantlist[r].RestaurantName;
                }
            }
        }
        foreach(string c in customerlist.Keys)
        {
            foreach(Order ord in customerlist[c].CustomerOrders)
            {
                if (ord.OrderId == order)
                {
                    customername = customerlist[c].CustomerName;
                }
            }
        }
        Console.WriteLine("{0,-12}{1,-15}{2,-20}{3,-30}{4,-10:C}{5,-12}", order, customername, restaurantname, orderlist[order].DeliveryDateTime, orderlist[order].OrderTotal, orderlist[order].OrderStatus);
    }
    Console.WriteLine();
}
//basic feature 5 (student 1, kayden)
void CreateNewOrder(int currentOrderId) 
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");
    string email = ValidateInputInList("Enter Customer Email: ",customerlist.Keys, errorMessage: "Email not found.");
    string restaurantId = ValidateInputInList("Enter Restaurant ID: ", restaurantlist.Keys, errorMessage: "Restaurant not found.");
    DateOnly deliveryDate = ValidateDateOnly("Enter Delivery Date (dd/mm/yyyy): ");
    TimeOnly deliveryTime = ValidateTimeOnly("Enter Delivery Time (hh:mm): ");
    DateTime deliveryDateTime = deliveryDate.ToDateTime(deliveryTime);
    string deliveryAddress = ValidateNonEmptyInput("Enter Delivery Address: ");
    currentOrderId = currentOrderId + 1;
    Order order = new(currentOrderId, DateTime.Now, 0, "", deliveryDateTime, deliveryAddress, "", false);

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

    Console.WriteLine("Available Food Items: ");
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
    Console.WriteLine($"\nOrder Total: ${orderTotal:F2} + $5.00 (delivery) = ${orderTotal + 5:F2}");
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
    customerlist[email].AddOrder(order);
    restaurant.RestaurantOrders.Enqueue(order);
    orderlist.Add(currentOrderId, order);
    //appends into bin/debug/net8.0
    using (StreamWriter sw = new StreamWriter("orders.csv", true))
    { 
        sw.WriteLine($"{currentOrderId},{email},{restaurantId},{deliveryDate:dd/MM/yyyy},{deliveryTime:HH:mm},{deliveryAddress},{order.OrderDateTime:dd/MM/yyyy HH:mm},{orderTotal},{order.OrderStatus},{itemsString}");
    }
    Console.WriteLine($"\nOrder {order.OrderId} created successfully! Status: Pending");
}

//basic feature 6(student2, cayden)
void ProcessOrder()
{
    Console.WriteLine("Process Order");
    Console.WriteLine("=============");
    string restaurantId = ValidateInputInList("Enter Restaurant ID: ", restaurantlist.Keys, errorMessage: "Restaurant not found.");
    string customername = "";
    string customeremail = "";
    foreach (Order order in restaurantlist[restaurantId].RestaurantOrders)
    {
        Console.WriteLine("Order " + order.OrderId + ":");
        foreach (string cust in customerlist.Keys)
        {
            foreach (Order o in customerlist[cust].CustomerOrders)
            {
                if (o.OrderId == order.OrderId)
                {
                    customername = customerlist[cust].CustomerName;
                    customeremail = customerlist[cust].EmailAddress;
                }
            }
        }
        Console.WriteLine("Customer: " + customername);
        Console.WriteLine("Ordered Items:");
        int count = 0;
        foreach (OrderedFoodItem ofl in orderlist[order.OrderId].OrderedFoodItemList)
        {
            count += 1;
            Console.WriteLine(count + ". " + ofl.ItemName + " - " + ofl.QtyOrdered);
        }
        Console.WriteLine("Delivery date/time: " + order.DeliveryDateTime);
        Console.WriteLine("Total Amount: " + order.OrderTotal);
        Console.WriteLine("Order Status: " + order.OrderStatus);
        Console.WriteLine();
        string process = ValidateInputInList("[C]onfirm / [R]eject / [S]kip / [D]eliver: ", ["C", "R", "S", "D"], "Invalid process option.");
        Console.WriteLine();
        if (process == "C")
        {
            if (order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Preparing";
                Console.WriteLine("Order " + order.OrderId + " confirmed. Status: " + order.OrderStatus);
                Console.WriteLine();
                orderlist[order.OrderId] = order;
                foreach (string cust in customerlist.Keys)
                {
                    foreach (Order o in customerlist[cust].CustomerOrders.ToArray())
                    {
                        if (o.OrderId == order.OrderId)
                        {
                            customerlist[cust].CustomerOrders[customerlist[cust].CustomerOrders.IndexOf(o)] = order;
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid process");
            }
        }
        else if (process == "R")
        {
            if (order.OrderStatus == "Pending")
            {
                order.OrderStatus = "Rejected";
                Console.WriteLine("Order " + order.OrderId + " rejected. Status: " + order.OrderStatus);
                Console.WriteLine();
                orderlist[order.OrderId] = order;
                foreach (string cust in customerlist.Keys)
                {
                    foreach (Order o in customerlist[cust].CustomerOrders.ToArray())
                    {
                        if (o.OrderId == order.OrderId)
                        {
                            customerlist[cust].CustomerOrders[customerlist[cust].CustomerOrders.IndexOf(o)] = order;
                        }
                    }
                }
                refundedorders.Push(order);
                Console.WriteLine("{0} ({1}) has been refunded {2:C}", customername, customeremail, order.OrderTotal);
            }
            else { Console.WriteLine("Invalid Process"); }
        }
        else if (process == "S")
        {
            if (order.OrderStatus == "Cancelled")
            {
                Console.WriteLine("Skipped");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Invalid processs");
            }
        }
        else if (process == "D")
        {
            Console.WriteLine(order.OrderStatus == "Preparing");
            if (order.OrderStatus == "Preparing")
            {
                order.OrderStatus = "Delivered";
                Console.WriteLine("Order " + order.OrderId + " will be delivered soon. Status: " + order.OrderStatus);
                Console.WriteLine();
                orderlist[order.OrderId] = order;
                foreach (string cust in customerlist.Keys)
                {
                    foreach (Order o in customerlist[cust].CustomerOrders.ToArray())
                    {
                        if (o.OrderId == order.OrderId)
                        {
                            customerlist[cust].CustomerOrders[customerlist[cust].CustomerOrders.IndexOf(o)] = order;
                        }
                    }
                }
            }
            else
            {
                {
                    Console.WriteLine("Invalid process");
                }
            }
        }
    }
}
//basic feature 7 (student1,kayden)
void ModifyOrder()
{
    Console.WriteLine("Modify Order");
    Console.WriteLine("============");

    Customer? customer = null;
    while (true)
    {
        string email = ValidateNonEmptyInput("Enter Customer Email: ");
        if (customerlist.ContainsKey(email))
        {
            customer = customerlist[email];
            break;
        }
        Console.WriteLine("Email not found.");
    }

    Dictionary<int, Order> orders = new Dictionary<int, Order>();
    foreach (Order o in customer.CustomerOrders)
    {
        if (o.OrderStatus == "Pending")
        {
            orders.Add(o.OrderId, o);
        }
    }
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
                Console.WriteLine($"\nNew Order Total: ${orderTotal:F2} + $5.00 (delivery) = ${orderTotal + 5:F2} ({(orderTotal >= oldOrderTotal ? "+" : "-")}${Math.Abs(orderTotal - oldOrderTotal):F2})");
                Console.WriteLine($"{(orderTotal > -oldOrderTotal ? "Payment Amount" : "Refund Amount")}: ${Math.Abs(orderTotal - oldOrderTotal):F2}");
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
                }
                ;

                restaurant.RemoveOrder(oldOrder);
                restaurant.AddOrder(order);
                customer.RemoveOrder(oldOrder);
                customer.AddOrder(order);

                string[] lines = File.ReadAllLines("orders.csv");
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(order.OrderId.ToString()))
                    {
                        string[] split = lines[i].Split(',', 10);
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
                string[] split = lines[i].Split(',', 10);
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
                string[] split = lines[i].Split(',', 10);
                lines[i] = $"{split[0]},{split[1]},{split[2]},{split[3]},{time:HH:mm},{split[5]},{split[6]},{split[7]},{split[8]},{split[9]}";
            }
        }
        File.WriteAllLines("orders.csv", lines);

        Console.WriteLine($"\nOrder {order.OrderId} updated. New Delivery Time: {time:HH:mm}");
    }
}

//basic feature 8 (student 2,cayden)
void DeleteOrder()
{
    Console.WriteLine("Delete Order");
    Console.WriteLine("============");
    string email = ValidateInputInList("Enter Customer Email: ", customerlist.Keys, errorMessage: "Email not found.");
    bool pending = false;
    foreach (Order o in customerlist[email].CustomerOrders)
    {
        if (o.OrderStatus == "Pending")
        {   
            pending = true;
            break;
        }
        else if (o.OrderStatus != "Pending" && o == customerlist[email].CustomerOrders[customerlist[email].CustomerOrders.Count - 1])
        {
            pending = false;
        }
    }
    if (pending)
    {
        Console.WriteLine("Pending Orders:");
        foreach (Order o in customerlist[email].CustomerOrders)
        {
            if (o.OrderStatus == "Pending")
            {
                Console.WriteLine(o.OrderId);
            }
        }

        while (true)
        {
            try
            {
                bool flag = false;
                Console.Write("Enter Order ID: ");
                int ordid = Convert.ToInt32(Console.ReadLine());
                foreach (Order o in customerlist[email].CustomerOrders)
                {
                    if (o.OrderStatus == "Pending")
                    {
                        if (ordid == o.OrderId)
                        {
                            Console.WriteLine("Customer: " + customerlist[email].CustomerName);
                            Console.WriteLine("Ordered Items:");
                            int count = 0;
                            foreach (OrderedFoodItem ofl in o.OrderedFoodItemList)
                            {
                                count += 1;
                                Console.WriteLine(count + ". " + ofl.ItemName + " - " + ofl.QtyOrdered);
                            }
                            Console.Write("Delivery date/time: ");
                            Console.WriteLine(o.DeliveryDateTime);
                            Console.Write("Total Amount: ");
                            Console.WriteLine(o.OrderTotal);
                            Console.Write("Order Status: ");
                            Console.WriteLine(o.OrderStatus);
                            Console.Write("Confirm deletion? [Y/N]: ");
                            string option = Console.ReadLine();
                            while (true)
                            {
                                if (option == "Y")
                                {
                                    o.OrderStatus = "Cancelled";
                                    refundedorders.Push(o);
                                    orderlist[o.OrderId] = o;
                                    Console.WriteLine("Order " + o.OrderId + " cancelled. Refund of " + o.OrderTotal + " processed.");
                                    break;
                                }
                                else if (option == "N")
                                {
                                    Console.WriteLine("Order deletion terminated.");
                                    break;
                                }
                                else { Console.WriteLine("Invalid option"); }
                            }
                            flag = true;
                            break;
                        }
                    }
                    else if (ordid != o.OrderId && o == customerlist[email].CustomerOrders[customerlist[email].CustomerOrders.Count - 1])
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    break;
                }
                else
                {
                    Exception ex = new Exception();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Please type a valid order ID.");
            }
        }
    }
    else
    {
        Console.WriteLine("No pending orders.");
    }
        
}
void TotalOrderAmount()
{   
    double total;
    double refundtotal;
    double grandtotal = 0;
    double grandrefund= 0;
    foreach(string r in restaurantlist.Keys)
    {
        total = 0;
        refundtotal = 0;
        Console.WriteLine(restaurantlist[r].RestaurantName);
        foreach (Order o  in restaurantlist[r].RestaurantOrders)
        {
            if (o.OrderStatus == "Delivered")
            {
                Console.WriteLine(o);
                total += o.OrderTotal;
                grandtotal += o.OrderTotal;
            }
        }
        Console.WriteLine("Order amount: {0:C}",total);
        
        foreach (Order o in restaurantlist[r].RestaurantOrders)
        {
            if (o.OrderStatus == "Cancelled")
            {
                Console.WriteLine(o);
                refundtotal += o.OrderTotal;
                grandrefund += o.OrderTotal;
            }
        }
        Console.WriteLine("Refund amount: {0:C}",refundtotal);
        Console.WriteLine();
    }
    Console.WriteLine("Total order amount: {0:C}",grandtotal);
    Console.WriteLine("Total refund amount: {0:C}",grandrefund);
    Console.WriteLine("Gruberoo profits: {0:C}",grandtotal-grandrefund);
    Console.WriteLine();
}