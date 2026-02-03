//==========================================================
// Student Number : S10273990
// Student Name : Kayden Tan
// Partner Name : Cayden Cheah
//==========================================================

using System;

namespace prg2_assg;

public class Order(int orderId, DateTime orderDateTime, double orderTotal, string orderStatus, DateTime deliveryDateTime, string deliveryAddress, string orderPaymentMethod, bool orderPaid)
{
    public int OrderId {get; set; } = orderId;
    public DateTime OrderDateTime {get; set; } = orderDateTime;
    public double OrderTotal {get; set; } = orderTotal;
    public string OrderStatus {get; set; } = orderStatus;
    public DateTime DeliveryDateTime {get; set; } = deliveryDateTime;
    public string DeliveryAddress {get; set; } = deliveryAddress;
    public string OrderPaymentMethod {get; set; } = orderPaymentMethod;
    public bool OrderPaid {get; set; } = orderPaid;

    public readonly List<OrderedFoodItem> orderedFoodItems = [];

    public double CalculateOrderTotal()
    {
        OrderTotal = 0;
        foreach (OrderedFoodItem orderedFoodItem in orderedFoodItems)
        {
            OrderTotal += orderedFoodItem.SubTotal;
        }
        return OrderTotal;
    }
    public void AddOrderedFoodItem(OrderedFoodItem orderedFoodItem) => orderedFoodItems.Add(orderedFoodItem);
    public bool RemoveOrderedFoodItem(OrderedFoodItem orderedFoodItem) => orderedFoodItems.Remove(orderedFoodItem);
    public void DisplayOrderedFoodItems()
    {
        foreach (OrderedFoodItem orderedFoodItem in orderedFoodItems)
        {
            Console.WriteLine(orderedFoodItem.ToString());
        }
    }
}
