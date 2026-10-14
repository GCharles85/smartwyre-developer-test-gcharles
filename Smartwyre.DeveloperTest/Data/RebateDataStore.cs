using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

public class RebateDataStore : IRebateDataStore
{
    public Rebate GetRebate(string rebateIdentifier)
    {
        // Access database to retrieve account, code removed for brevity 
        // Return dummy rebate for demonstration purposes
        return new Rebate(){
            Identifier = "RebateOne", 
            Incentive = IncentiveType.FixedRateRebate, 
            Amount = 100m, 
            Percentage = 0.2m
        };
    }

    public void StoreCalculationResult(RebateResult result)
    {
        // Update account in database, code removed for brevity
        // Simulates storing the result of a rebate calculation
        result.Id = Random.Shared.Next(1, 1000); // Generate a random ID for the rebate result
    }
}
