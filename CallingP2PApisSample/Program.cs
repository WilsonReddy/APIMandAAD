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

                string supplierId = "0002113766"; // TODO: Replace this sample with your supplier Id. 
                string shipmentId = "sampleShipmentId"; // TODO: Replace this sample with your shipment Id. 
                var requestUrl = $"https://p2p.azure-api.net/receipting/v1/api/suppliers/{supplierId}/shipments/{shipmentId}";

                // Call the put api 
                var result = await client.PutAsJsonAsync(requestUrl, rootObject);

                // See the response of the call. 
                var response  = await result.Content.ReadAsStringAsync();
            }
        }

        private static RootObject CreateRootObject()
        {
            // Declare root object. 
            RootObject rootObject  = new RootObject();

            // Populate the object. 
            rootObject.trackingNumber = "TrackSample123";
            rootObject.shippingCarrier = "FedEx";
            rootObject.expectedDeliveryDate = DateTime.Today;
            rootObject.isShipToAddressSameAsPO = true; 
            
            // Populate ship from and ship to address
            var shipFromAddress = new Address
                                      {
                                          address1 = "Wamon-Lockr-Avondale food",
                                          address2 = "11448 Avondale Rd",
                                          city = "Redmond",
                                          country = "USA",
                                          postalCode = "98052",
                                          state = "WA"
                                      };

            var shipToAddress = new Address
                                      {
                                          address1 = "Microsoft Redmond Woods Campus",
                                          address2 = "Building C 5000 148th Ave NE",
                                          city = "Redmond",
                                          country = "USA",
                                          postalCode = "98052",
                                          state = "WA"
                                      };

            rootObject.shipFromAddress = shipFromAddress;
            rootObject.shipToAddress = shipToAddress; 

            Item item = new Item
                            {
                                purchaseOrderNumber = "0007709431", // 10 digit number (with padded 0's)
                                purchaseOrderLineItemNumber = "00010", // 5 digit number (with padded 0's)
                                assetTag = "TestTag",
                                serialNumber = "SerialNumberTest",
                                quantity = 0.0,
                                recipientAlias = "alias"
                            };

            // Add items to object. 
            rootObject.items = new List<Item> { item };

            // Create your additional info dictionary. 
            IDictionary<string, string> additionalInfo = new Dictionary<string, string>();
            
            // Populating this dictionary is optional.
            additionalInfo.Add("optionalKey1", "optionalKey2");

            // Add additionalInfo dictionary to object. 
            rootObject.additionalInfo = additionalInfo;
            
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
