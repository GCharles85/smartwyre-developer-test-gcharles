using System;
using System.Collections.Generic;
using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Extensions;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Xunit;

namespace Smartwyre.DeveloperTest.Tests;

public class RebateServiceTests
{
    private class FakeProductDataStore(Product product) : IProductDataStore
    {
        public Product GetProduct(string productIdentifier) => product;
    }

    private class FakeRebateDataStore(Rebate rebate) : IRebateDataStore
    {
        public List<RebateResult> Stored { get; } = new();

        public Rebate GetRebate(string rebateIdentifier) => rebate;

        public void StoreCalculationResult(RebateResult rebate) => Stored.Add(rebate);
    }

    private static Product CreateProduct(SupportedIncentiveType supported, decimal price = 10m) =>
        new() { Id = 1, Identifier = "Product", Price = price, Uom = "EA", SupportedIncentives = supported };

    private static Rebate CreateRebate(IncentiveType incentive, decimal amount = 0m, decimal percentage = 0m) =>
        new() { Identifier = "Rebate", Incentive = incentive, Amount = amount, Percentage = percentage };

    private static CalculateRebateRequest CreateRequest(decimal volume) =>
        new() { RebateIdentifier = "Rebate", ProductIdentifier = "Product", Volume = volume };

    private static RebateResult CreateResult(Product product, Rebate rebate) =>
        new() { Product = product, Rebate = rebate };


    [Fact]
    public void CalculateFixedCashAmount_SupportedWithAmount_ReturnsRebateAmount()
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.FixedCashAmount), CreateRebate(IncentiveType.FixedCashAmount, amount: 50m));

        result.CalculateFixedCashAmount();

        Assert.True(result.Success);
        Assert.Equal(50m, result.RebateAmount);
    }

    [Fact]
    public void CalculateFixedCashAmount_NotSupported_Fails()
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.FixedRateRebate), CreateRebate(IncentiveType.FixedCashAmount, amount: 50m));

        result.CalculateFixedCashAmount();

        Assert.False(result.Success);
        Assert.Equal(0m, result.RebateAmount);
    }

    [Fact]
    public void CalculateFixedCashAmount_ZeroAmount_Fails()
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.FixedCashAmount), CreateRebate(IncentiveType.FixedCashAmount, amount: 0m));

        result.CalculateFixedCashAmount();

        Assert.False(result.Success);
    }

    [Fact]
    public void CalculateFixedRateRebate_Supported_ReturnsPriceTimesPercentageTimesVolume()
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.FixedRateRebate, price: 30m), CreateRebate(IncentiveType.FixedRateRebate, percentage: 0.2m));

        result.CalculateFixedRateRebate(CreateRequest(10m));

        Assert.True(result.Success);
        Assert.Equal(60m, result.RebateAmount);
    }

    [Fact]
    public void CalculateFixedRateRebate_NotSupported_Fails()
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.AmountPerUom, price: 30m), CreateRebate(IncentiveType.FixedRateRebate, percentage: 0.2m));

        result.CalculateFixedRateRebate(CreateRequest(10m));

        Assert.False(result.Success);
        Assert.Equal(0m, result.RebateAmount);
    }

    [Theory]
    [InlineData(0, 30, 10)]
    [InlineData(0.2, 0, 10)]
    [InlineData(0.2, 30, 0)]
    public void CalculateFixedRateRebate_ZeroInput_Fails(decimal percentage, decimal price, decimal volume)
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.FixedRateRebate, price), CreateRebate(IncentiveType.FixedRateRebate, percentage: percentage));

        result.CalculateFixedRateRebate(CreateRequest(volume));

        Assert.False(result.Success);
    }

    [Fact]
    public void CalculateAmountPerUom_Supported_ReturnsAmountTimesVolume()
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.AmountPerUom), CreateRebate(IncentiveType.AmountPerUom, amount: 5m));

        result.CalculateAmountPerUom(CreateRequest(4m));

        Assert.True(result.Success);
        Assert.Equal(20m, result.RebateAmount);
    }

    [Fact]
    public void CalculateAmountPerUom_NotSupported_Fails()
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.FixedCashAmount), CreateRebate(IncentiveType.AmountPerUom, amount: 5m));

        result.CalculateAmountPerUom(CreateRequest(4m));

        Assert.False(result.Success);
        Assert.Equal(0m, result.RebateAmount);
    }

    [Theory]
    [InlineData(0, 4)]
    [InlineData(5, 0)]
    public void CalculateAmountPerUom_ZeroInput_Fails(decimal amount, decimal volume)
    {
        var result = CreateResult(CreateProduct(SupportedIncentiveType.AmountPerUom), CreateRebate(IncentiveType.AmountPerUom, amount: amount));

        result.CalculateAmountPerUom(CreateRequest(volume));

        Assert.False(result.Success);
    }

    [Theory]
    // FixedCashAmount
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount, 50, 0, 10, 1, true, 50)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.AmountPerUom, 50, 0, 10, 1, false, 0)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedCashAmount, 0, 0, 10, 1, false, 0)]
    // FixedRateRebate
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, 0, 0.2, 30, 10, true, 60)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedCashAmount, 0, 0.2, 30, 10, false, 0)]
    [InlineData(IncentiveType.FixedRateRebate, SupportedIncentiveType.FixedRateRebate, 0, 0.2, 30, 0, false, 0)]
    // AmountPerUom
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, 5, 0, 10, 4, true, 20)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedRateRebate, 5, 0, 10, 4, false, 0)]
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, 0, 0, 10, 4, false, 0)]
    // Product supporting multiple incentives
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom, 5, 0, 10, 4, true, 20)]
    [InlineData(IncentiveType.FixedCashAmount, SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom, 50, 0, 10, 1, false, 0)]
    // Negative volume
    [InlineData(IncentiveType.AmountPerUom, SupportedIncentiveType.AmountPerUom, 5, 0, 10, -4, false, 0)]
    public void Calculate_ReturnsExpectedResult(
        IncentiveType incentive,
        SupportedIncentiveType supported,
        decimal amount,
        decimal percentage,
        decimal price,
        decimal volume,
        bool expectedSuccess,
        decimal expectedAmount)
    {
        var rebateStore = new FakeRebateDataStore(CreateRebate(incentive, amount, percentage));
        var service = new RebateService(rebateStore, new FakeProductDataStore(CreateProduct(supported, price)));

        var result = service.Calculate(CreateRequest(volume));

        Assert.Equal(expectedSuccess, result.Success);
        Assert.Equal(expectedAmount, result.RebateAmount);
        Assert.Equal(expectedSuccess ? 1 : 0, rebateStore.Stored.Count);
    }

    [Fact]
    public void Calculate_RebateNotFound_FailsAndDoesNotStore()
    {
        var rebateStore = new FakeRebateDataStore(null);
        var service = new RebateService(rebateStore, new FakeProductDataStore(CreateProduct(SupportedIncentiveType.FixedCashAmount)));

        var result = service.Calculate(CreateRequest(10m));

        Assert.False(result.Success);
        Assert.Empty(rebateStore.Stored);
    }

    [Fact]
    public void Calculate_ProductNotFound_FailsAndDoesNotStore()
    {
        var rebateStore = new FakeRebateDataStore(CreateRebate(IncentiveType.FixedCashAmount, amount: 50m));
        var service = new RebateService(rebateStore, new FakeProductDataStore(null));

        var result = service.Calculate(CreateRequest(10m));

        Assert.False(result.Success);
        Assert.Empty(rebateStore.Stored);
    }

    [Fact]
    public void Calculate_NullRequest_Fails()
    {
        var rebateStore = new FakeRebateDataStore(CreateRebate(IncentiveType.FixedCashAmount, amount: 50m));
        var service = new RebateService(rebateStore, new FakeProductDataStore(CreateProduct(SupportedIncentiveType.FixedCashAmount)));

        var result = service.Calculate(null);

        Assert.False(result.Success);
        Assert.Empty(rebateStore.Stored);
    }

    [Fact]
    public void Calculate_Success_StoresResult()
    {
        var rebateStore = new FakeRebateDataStore(CreateRebate(IncentiveType.FixedCashAmount, amount: 50m));
        var service = new RebateService(rebateStore, new FakeProductDataStore(CreateProduct(SupportedIncentiveType.FixedCashAmount)));

        var result = service.Calculate(CreateRequest(1m));

        Assert.True(result.Success);
        Assert.Same(result, Assert.Single(rebateStore.Stored));
    }

    [Fact]
    public void StoreCalculationResult_AssignsId()
    {
        var result = new RebateResult();

        new RebateDataStore().StoreCalculationResult(result);

        Assert.InRange(result.Id, 1, 999);
    }

    [Fact]
    public void Calculate_UnknownIncentiveType_Throws()
    {
        var rebateStore = new FakeRebateDataStore(CreateRebate((IncentiveType)99, amount: 50m, percentage: 0.2m));
        var allSupported = SupportedIncentiveType.FixedRateRebate | SupportedIncentiveType.AmountPerUom | SupportedIncentiveType.FixedCashAmount;
        var service = new RebateService(rebateStore, new FakeProductDataStore(CreateProduct(allSupported)));

        Assert.Throws<ArgumentOutOfRangeException>(() => service.Calculate(CreateRequest(10m)));
        Assert.Empty(rebateStore.Stored);
    }
}
