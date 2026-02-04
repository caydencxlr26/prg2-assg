using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg2_assg
{
    class Restaurant
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantEmail { get; set; }
        public List<Order> RestaurantOrders { get; set; } = new List<Order>();
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
            foreach (Order order in RestaurantOrders)
            {
                Console.WriteLine(order);
            }
        }
        public void DisplaySpecialOffers()
        {
            foreach (SpecialOffer offer in RestaurantSpecialOffers)
            {
                Console.WriteLine(offer);
            }
        }
        public void DisplayMenu()
        {
            foreach (Menu menu in RestaurantMenus)
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
    }
}
