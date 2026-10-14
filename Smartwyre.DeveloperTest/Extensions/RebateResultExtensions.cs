using System;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Extensions;

// Extension methods for RebateResult to calculate rebate amounts based on different incentive types
// Calculating different rebate amounts is possible using only the members on RebateResult, so this is a good candidate for an extension class
// Allows for better adherence to SOLID principles
public static class RebateResultExtensions
{
   
    extension(RebateResult rebateResult)
    {   
        public void CalculateFixedCashAmount()
        {
            if (!rebateResult.Product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
            {
                rebateResult.Success = false;
            }
            else if (rebateResult.Rebate.Amount == 0)
            {
                rebateResult.Success = false;
            }
            else
            {
                rebateResult.RebateAmount = rebateResult.Rebate.Amount;
                rebateResult.Success = true;
            }
        }

        public void CalculateFixedRateRebate(CalculateRebateRequest request)
        {
            if (!rebateResult.Product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
            {
                rebateResult.Success = false;
            }
            else if (rebateResult.Rebate.Percentage == 0 || rebateResult.Product.Price == 0 || request.Volume == 0)
            {
                rebateResult.Success = false;
            }
            else
            {
                rebateResult.RebateAmount = rebateResult.Product.Price * rebateResult.Rebate.Percentage * request.Volume;
                rebateResult.Success = true;
            }
        }

        public void CalculateAmountPerUom(CalculateRebateRequest request)
        {
            if (!rebateResult.Product.SupportedIncentives.HasFlag(SupportedIncentiveType.AmountPerUom))
            {
                rebateResult.Success = false;
            }
            else if (rebateResult.Rebate.Amount == 0 || request.Volume == 0)
            {
                rebateResult.Success = false;
            }
            else
            {
                rebateResult.RebateAmount = rebateResult.Rebate.Amount * request.Volume;
                rebateResult.Success = true;
            }
        }
    }   
}
