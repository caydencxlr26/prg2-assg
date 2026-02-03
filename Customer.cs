//==========================================================
// Student Number : S10273990
// Student Name : Kayden Tan
// Partner Name : Cayden Cheah
//==========================================================

using System;

namespace prg2_assg;

public class Customer(string emailAddress, string customerName)
{
    public string EmailAddress {get; set; } = emailAddress;
    public string CustomerName {get; set; } = customerName;

    public readonly List<Order> orderList = [];

    public void AddOrder(Order order)
    {
        orderList.Add(order);
    }
    public void DisplayAllOrders()
    {
        foreach (Order order in orderList)
        {
            Console.WriteLine(order.ToString());
        }
    }
    public bool RemoveOrder(Order order) => orderList.Remove(order);
}
