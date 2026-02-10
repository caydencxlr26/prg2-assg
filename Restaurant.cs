//==========================================================
// Student Number : S10272352
// Student Name : Cheah Lok Weng Cayden
// Partner Name : Kayden Tan Yu Hang
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10272352_PRG2Assignment
{
    class Restaurant
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantEmail { get; set; }
        public Queue<Order> RestaurantOrders { get; set; } = new Queue<Order>();
        public List<SpecialOffer> RestaurantSpecialOffers { get; set; } = new List<SpecialOffer>();
        public List<Menu> RestaurantMenus { get; set; } = new List<Menu>();

        public Restaurant(string restaurantId, string restaurantName, string restaurantEmail)
        {
            RestaurantId = restaurantId;
            RestaurantName = restaurantName;
            RestaurantEmail = restaurantEmail;
        }
        public void DisplayOrders()
        {
            foreach(Order order in RestaurantOrders)
            {
                Console.WriteLine(order);
            }
        }
        public void DisplaySpecialOffers()
        {
            foreach(SpecialOffer offer in RestaurantSpecialOffers)
            {
                Console.WriteLine(offer);
            }
        }
        public void DisplayMenu()
        {
            foreach(Menu menu in RestaurantMenus)
            {
                Console.WriteLine(menu);
            }
        }
        public void AddMenu(Menu menu)
        {
            RestaurantMenus.Add(menu);
        }
        public bool RemoveMenu(Menu menu)
        {
            return RestaurantMenus.Remove(menu);
        }
        public override string ToString()
        {
            return "Restaurant ID: " + RestaurantId + "\tRestaurant Name: " + RestaurantName + "\tRestaurant Email:" + RestaurantEmail;
        }

        public void AddOrder(Order order) => RestaurantOrders.Enqueue(order);
        public bool RemoveOrder(Order order)
        {
            List<Order> orderList = [.. RestaurantOrders];
            bool result = orderList.Remove(order);
            RestaurantOrders = new Queue<Order>(orderList);

            return result;
        }
        public Order NextOrder() => RestaurantOrders.Dequeue();
    }
}
