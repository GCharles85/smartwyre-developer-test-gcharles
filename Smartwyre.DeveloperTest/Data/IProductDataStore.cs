using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

// Interface for a data store that provides access to product data, good for testing and extension purposes also
public interface IProductDataStore
{
     public Product GetProduct(string productIdentifier);
}
