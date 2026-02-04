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

namespace prg2_assg
{
    public class FoodItem
    {
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public double ItemPrice { get; set; }
        public string Customise { get; set; }

        public FoodItem(string itemName, string itemDesc, double itemPrice, string customise)
        {
            ItemName = itemName;
            ItemDesc = itemDesc;
            ItemPrice = itemPrice;
            Customise = customise;
        }
        public override string ToString()
        {
            return "Item Name: " + ItemName + "\tItem Description: " + ItemDesc + "\tItem Price: " + ItemPrice + "\tCustomisation: " + Customise;
        }
    }
}
