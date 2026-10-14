using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Data;

// Interface for a data store that provides access to rebate data, good for testing and extension purposes also
public interface IRebateDataStore
{
    public void StoreCalculationResult(RebateResult result);
    public Rebate GetRebate(string rebateIdentifier);
}
