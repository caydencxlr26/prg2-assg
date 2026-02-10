using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg2_assg
{
    class Customer
    {
        public string EmailAddress { get; set; }
        public string CustomerName { get; set; }

        public Dictionary<int, Order> CustomerOrders { get; set; } = [];

        public Customer(string emailAddress, string customerName)
        {
            EmailAddress = emailAddress;
            CustomerName = customerName;
        }
        public void AddOrder(Order order)
        {
            CustomerOrders[order.OrderId] = order;
        }
        public void DisplayAllOrders()
        {
            foreach (Order order in CustomerOrders.Values)
            {
                Console.WriteLine(order.ToString());
            }
        }
        public bool RemoveOrder(Order order)
        {
            return CustomerOrders.Remove(order.OrderId);
        }
        public override string ToString()
        {
            return "Email address: " + EmailAddress + "\t Customer Name: " + CustomerName;
        }
    }
}
