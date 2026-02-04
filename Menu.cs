using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace prg2_assg
{
    class Menu
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public List<FoodItem> FoodItemList { get; set; } = new List<FoodItem>();
        public Menu(string menuId, string menuName)
        {
            MenuId = menuId;
            MenuName = menuName;
        }
        public void AddFoodItem(FoodItem foodItem)
        {
            FoodItemList.Add(foodItem);
        }
        public bool RemoveFoodItem(FoodItem fooditem)
        {
            return FoodItemList.Remove(fooditem);
        }
        public void DisplayFoodItems()
        {
            foreach (FoodItem item in FoodItemList)
            {
                Console.WriteLine(item);
            }
        }
        public override string ToString()
        {
            return "Menu Id: " + MenuId + "\tMenu Name: " + MenuName;
        }
    }
}
