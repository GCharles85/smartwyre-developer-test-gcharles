using System;

namespace Smartwyre.DeveloperTest.Types;

// Flags allow for bitwise operations on the enum values, enabling combinations of supported incentive types
[Flags]
public enum SupportedIncentiveType
{
    FixedRateRebate = 1 << 0,
    AmountPerUom = 1 << 1,
    FixedCashAmount = 1 << 2,
}
