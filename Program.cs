using prg2_assg;
using System.Numerics;

List<Restaurant> restaurantlist = new List<Restaurant>();
List<FoodItem> fooditemlist = new List<FoodItem>();
Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
LoadData();
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
            restaurantlist.Add(r);
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
            fooditemlist.Add(fi);
        }
        Console.WriteLine(total + " food items loaded!");
        sr.Close();
    }
}