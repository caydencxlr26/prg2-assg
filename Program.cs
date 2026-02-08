using prg2_assg;
using System.Numerics;

Dictionary<string, Restaurant> restaurantlist = new Dictionary<string, Restaurant>();
Dictionary<string, FoodItem> fooditemlist = new Dictionary<string, FoodItem>();
Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
LoadData();


Dictionary<string, Customer> customerList = [];
LoadCustomers();
LoadOrders();

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
        Console.WriteLine("All Restaurants and Menu Items");
        Console.WriteLine("==============================");
        foreach (Restaurant restaurant in restaurantlist.Values)
        {
            Console.WriteLine(restaurant.ToString());
            foreach(Menu menu in restaurant.RestaurantMenus)
            {
                Console.WriteLine(menu.ToString());
                foreach(FoodItem foodItem in menu.FoodItemList)
                {
                    Console.WriteLine(foodItem.ToString());
                }
            }
            Console.WriteLine("");
        }
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
void LoadOrders()
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
    }
    Console.WriteLine($"{total} orders loaded!");
}