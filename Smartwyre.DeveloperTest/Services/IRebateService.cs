using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public interface IRebateService
{
    // Updated return type to RebateResult which extends RebateCalculation and includes additional properties for the result of the rebate calculation
    RebateResult Calculate(CalculateRebateRequest request);
}
