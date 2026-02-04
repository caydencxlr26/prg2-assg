//==========================================================
// Student Number : S10272352
// Student Name : Cheah Lok Weng Cayden
// Partner Name : Kayden Tan Yu Hang
//==========================================================
using System;


namespace prg2_assg;
public class OrderedFoodItem : FoodItem
{
    public int QtyOrdered { get; set; }
    public double SubTotal { get; set; }

    public OrderedFoodItem(string itemName, string itemDesc, double itemPrice, string customise, int qtyOrdered, double subTotal) : base(itemName, itemDesc, itemPrice, customise)
    {
        QtyOrdered = qtyOrdered;
        SubTotal = subTotal;
    }
    public double CalculateSubtotal()
    {
        SubTotal = 0;
        SubTotal = QtyOrdered * ItemPrice;
        return SubTotal;
    }
}
