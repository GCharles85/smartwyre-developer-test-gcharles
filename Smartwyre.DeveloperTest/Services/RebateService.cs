using System;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Extensions;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    IRebateDataStore _rebateDataStore;
    IProductDataStore _productDataStore;

    public RebateService(IRebateDataStore rebateDataStore, IProductDataStore productDataStore){
        _rebateDataStore = rebateDataStore;
        _productDataStore = productDataStore;
    }

    public RebateResult Calculate(CalculateRebateRequest request)
    {    
        RebateResult result = new RebateResult();

         // Validate the retrieved product and rebate before proceeding with the calculation
        if (request == null || request.Volume < 0)
        {
            result.Success = false;
            return result;
        }
        
        // Extract code to retrieve product and rebate from the data stores for better readability and maintainability
        Product product = RetrieveProduct(request);
        Rebate rebate = RetrieveRebate(request);

        if (rebate == null || product == null)
        {
            result.Success = false;
            return result;
        }

        result.Product = product;
        result.Rebate = rebate;
      
        // Use extension methods to calculate rebate amounts based on the incentive type, promoting separation of concerns and adherence to SOLID principles
        switch (rebate.Incentive)
        {
        case IncentiveType.FixedCashAmount:
            result.CalculateFixedCashAmount();
            break;
        case IncentiveType.FixedRateRebate:
            result.CalculateFixedRateRebate(request);
            break;
        case IncentiveType.AmountPerUom:             
            result.CalculateAmountPerUom(request);
            break;
        default:
            throw new ArgumentOutOfRangeException(nameof(rebate.Incentive), rebate.Incentive, "Unsupported incentive type");
        }
        
        if (result.Success)
        {
            _rebateDataStore.StoreCalculationResult(result);
        }

        return result;
    }

    private Product RetrieveProduct(CalculateRebateRequest request) => _productDataStore.GetProduct(request.ProductIdentifier);

    private Rebate RetrieveRebate(CalculateRebateRequest request) => _rebateDataStore.GetRebate(request.RebateIdentifier);  
}
