using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class ProductDataStore : IProductDataStore
{
    public Product GetProduct(string productIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        // Return dummy product for demonstration purposes
        return new Product(){
            Id =  1, 
            Identifier = "ProductOne", 
            Price = 30m, 
            Uom = "EA", 
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        };
    }
}
