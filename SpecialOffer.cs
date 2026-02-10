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
    class SpecialOffer
    {
        public string OfferCode { get; set; } 
        public string OfferDesc { get; set; } 
        public double Discount { get; set; } 
        public List<Order> OfferOrders { get; set; } = new List<Order>();

        public SpecialOffer(string offerCode, string offerDesc, double discount)
        {
            OfferCode = offerCode;
            OfferDesc = offerDesc;
            Discount = discount;
        }
        public override string ToString()
        {
            return "Offer Code: " + OfferCode + "\tOffer Description: " + OfferDesc + "\tDiscount: " + Discount;
        }
    }
}
