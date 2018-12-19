namespace CallingP2PApisSample
{
    using System;
    using System.Collections.Generic;

    public class RootObject
    {
        public string ShipmentId { get; set; }
        public string TrackingNumber { get; set; } 
        public string ShippingCarrier { get; set; }
        public string OrganizationName { get; set; }
        public Address ShipToAddress { get; set; }
        public bool IsShipToAddressSameAsPO { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public IDictionary<string,string> AdditionalInfo { get; set; }
        public List<PurchaseOrderItem> PurchaseOrderItems { get; set; }
    }

}
