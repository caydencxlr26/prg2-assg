using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prg2_assg
{
    class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderPaymentMethod { get; set; }
        public bool OrderPaid { get; set; }

        public List<OrderedFoodItem> OrderedFoodItemList { get; set; } = new List<OrderedFoodItem>();

        public Order(int orderId, DateTime orderDateTime, double orderTotal, string orderStatus, DateTime deliveryDateTime, string deliveryAddress, string orderPaymentMethod, bool orderPaid)
        {
            OrderId = orderId;
            OrderDateTime = orderDateTime;
            OrderTotal = orderTotal;
            OrderStatus = orderStatus;
            DeliveryDateTime = deliveryDateTime;
            DeliveryAddress = deliveryAddress;
            OrderPaymentMethod = orderPaymentMethod;
            OrderPaid = orderPaid;
        }
        public double CalculateOrderTotal()
        {
            OrderTotal = 0;
            foreach (OrderedFoodItem orderedFoodItem in OrderedFoodItemList)
            {
                OrderTotal += orderedFoodItem.SubTotal;
            }
            return OrderTotal;
        }
        public void AddOrderedFoodItem(OrderedFoodItem orderedFoodItem)
        {
            OrderedFoodItemList.Add(orderedFoodItem);
        }
        public bool RemoveOrderedFoodItem(OrderedFoodItem orderedFoodItem)
        {
            return OrderedFoodItemList.Remove(orderedFoodItem);
        }
        public void DisplayOrderedFoodItems()
        {
            foreach (OrderedFoodItem orderedFoodItem in OrderedFoodItemList)
            {
                Console.WriteLine(orderedFoodItem.ToString());
            }
        }
        public override string ToString()
        {
            return "Order ID: " + OrderId + "\t Order DateTime: " + OrderDateTime + "\t Order Total: " + OrderTotal + "\t Order Status: " + OrderStatus + "\tDelivery DateTime: " + DeliveryDateTime + "\tDelivery Address: " + DeliveryAddress + "\tOrder Payment Method: " + OrderPaymentMethod + "\tOrder Paid: " + OrderPaid;
        }
    }

}
