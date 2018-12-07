namespace CallingP2PApisSample
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;

    public class Program
    {
        public static void Main(string[] args)
        {
            CallP2PApi().Wait();
        }

        public static async Task CallP2PApi()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));

                // Add your subscription key to request header.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "<<your subscription key>>"); // TODO: Fill subscription key.

                // Add a new correlation id to the request header. 
                client.DefaultRequestHeaders.Add("Correlation-Id", Guid.NewGuid().ToString());

                // Add a bearer token in authorization header of the call. 
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken().Result);

                // Create your data object.  
                RootObject rootObject = CreateRootObject();

                // Set the request url.
                var requestUrl = $"https://p2p.azure-api.net/receipt/v1/api/shipments";

                // Call the put api 
                var result = await client.PostAsJsonAsync(requestUrl, rootObject); 

                // See the response of the call. 
                var response  = await result.Content.ReadAsStringAsync();
            }
        }

        private static RootObject CreateRootObject()
        {
            // Declare root object. 
            RootObject rootObject  = new RootObject();

            // Populate the object. These are only example values. 
            rootObject.ShipmentId = "sampleShipmentId"; 
            rootObject.TrackingNumber = "TrackSample123";
            rootObject.ShippingCarrier = "FedEx";
            rootObject.ExpectedDeliveryDate = DateTime.Today;
            rootObject.IsShipToAddressSameAsPO = true; 
           
            var shipToAddress = new Address
                                      {
                                          Address1 = "Microsoft Redmond Woods Campus",
                                          Address2 = "Building C 5000 148th Ave NE",
                                          City = "Redmond",
                                          Country = "USA",
                                          PostalCode = "98052",
                                          State = "WA"
                                      };

            rootObject.ShipToAddress = shipToAddress;

            var shipFromAddress = new Address
                                      {
                                          Address1 = "Wamon-Lockr-Avondale food",
                                          Address2 = "11448 Avondale Rd",
                                          City = "Redmond",
                                          Country = "USA",
                                          PostalCode = "98052",
                                          State = "WA"
                                      };

            PurchaseOrderItem purchaseOrderItem = new PurchaseOrderItem
                            {
                                SupplierId = "0002113766",
                                PurchaseOrderNumber = "0007709431", // 10 digit number (with padded 0's)
                                PurchaseOrderLineItemNumber = "00010", // 5 digit number (with padded 0's)
                                Quantity = 0.0,
                                ShipFromAddress = shipFromAddress,
                                RecipientAlias = "alias",
                                Items = new List<Item>()
                            };

            // Create the item.
            Item item = new Item()
                            {
                                AssetTag = "TestTag",
                                SerialNumber = "SerialNumberTest",
                                ItemRecipient = "alias"
                            };

            // Add the asset item.  
            purchaseOrderItem.Items.Add(item); 
            
            // Add items to object. 
            rootObject.PurchaseOrderItems = new List<PurchaseOrderItem> { purchaseOrderItem };

            // Create your additional info dictionary. 
            IDictionary<string, string> additionalInfo = new Dictionary<string, string>();
            
            // Populating this dictionary is optional.
            additionalInfo.Add("optionalKey1", "optionalKey2");

            // Add additionalInfo dictionary to object. 
            rootObject.AdditionalInfo = additionalInfo;
            
            return rootObject;
        }

        public static async Task<string> GetToken()
        {

            // Add your tenant to the authentication context. 
            string authority = string.Format(CultureInfo.InvariantCulture, "https://login.microsoftonline.com/{0}", "<< your tenant id>>"); // TODO: Add tenant id. 
            var authContext = new AuthenticationContext(authority);

            // Add your application Id and App Key credentials.  
            var clientCredentials = new ClientCredential("<<your application id>>", "<<your app key>>"); // TODO: Add application Id and App Key. 
            
            // Acquire token for pre-prod microsoft resource : deaa4cb1-6028-460f-b289-ceb478edb492
            AuthenticationResult result = await authContext.AcquireTokenAsync("https://microsoft.onmicrosoft.com/deaa4cb1-6028-460f-b289-ceb478edb492", clientCredentials);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            // return token.  
            return result.AccessToken;
        }

    }
}
