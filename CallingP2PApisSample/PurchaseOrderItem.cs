namespace CallingP2PApisSample
{
    using System.Collections.Generic;
    using System.Dynamic;

    public class PurchaseOrderItem
    {
        public string SupplierId { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public string PurchaseOrderLineItemNumber  { get; set; }
        public Address ShipFromAddress { get; set; }
        public double Quantity { get; set; }
        public string RecipientAlias { get; set; }
        public List<Item> Items { get; set; }
          

    }
}
