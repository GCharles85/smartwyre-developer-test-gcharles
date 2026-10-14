using System;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        CalculateRebateRequest request = new CalculateRebateRequest()
        {
            RebateIdentifier = "RebateOne",
            ProductIdentifier = "ProductOne",
            Volume = 10
        };
        RebateService rebateService = new RebateService(new RebateDataStore(), new ProductDataStore());
        RebateResult result = rebateService.Calculate(request);

        Console.WriteLine($"""
            Rebate Result Details:
                Rebate Calculation Success?: {result.Success}
                Rebate Id: {result.Id}
                Rebate amount: {result.RebateAmount}
                Product: {result.Product.Identifier}
                Rebate: {result.Rebate.Identifier}
            """);   
    }
}
