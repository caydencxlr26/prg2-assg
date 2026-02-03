//==========================================================
// Student Number : S10273990
// Student Name : Kayden Tan
// Partner Name : Cayden Cheah
//==========================================================

using System;

namespace prg2_assg;

public class SpecialOffer(string offerCode, string offerDesc, double discount)
{
    public string OfferCode {get; set; } = offerCode;
    public string OfferDesc {get; set; } = offerDesc;
    public double Discount {get; set; } = discount;

    public List<Order> orders = [];
}
