namespace Smartwyre.DeveloperTest.Types;

public class RebateResult
{
    public int Id { get; set; }
    public bool Success { get; set; }
    public decimal RebateAmount { get; set; }
    public Product Product { get; set; }
    public Rebate Rebate { get; set; }
}
