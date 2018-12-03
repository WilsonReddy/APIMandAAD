namespace CallingP2PApisSample
{
    using System;
    using System.Collections.Generic;

    public class RootObject
    {
        public string trackingNumber { get; set; } 
        public string shippingCarrier { get; set; }
        public DateTime expectedDeliveryDate  { get; set; }
        public Address shipFromAddress { get; set; }
        public Address shipToAddress { get; set; }
        public bool isShipToAddressSameAsPO { get; set; }
        public IDictionary<string,string> additionalInfo { get; set; }
        public List<Item> items { get; set; }
    }

}
