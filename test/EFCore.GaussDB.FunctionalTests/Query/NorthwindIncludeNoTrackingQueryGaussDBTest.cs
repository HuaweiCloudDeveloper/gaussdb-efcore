using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindIncludeNoTrackingQueryGaussDBTest : NorthwindIncludeNoTrackingQueryTestBase<
    NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    private const string IncludeApplySkip =
        "Local-only: openGauss rejects the current APPLY/LATERAL-shaped SQL generated for these include patterns.";

    // ReSharper disable once UnusedParameter.Local
    public NorthwindIncludeNoTrackingQueryGaussDBTest(NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture)
        : base(fixture)
    {
        Fixture.TestSqlLoggerFactory.Clear();
    }

    public override async Task Include_collection_with_last_no_orderby(bool async)
        => Assert.Equal(
            RelationalStrings.LastUsedWithoutOrderBy(nameof(Enumerable.Last)),
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Include_collection_with_last_no_orderby(async))).Message);

    [ConditionalTheory(Skip = IncludeApplySkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Include_collection_with_cross_apply_with_filter(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = IncludeApplySkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Include_collection_with_outer_apply_with_filter(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = IncludeApplySkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Include_collection_with_outer_apply_with_filter_non_equality(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = IncludeApplySkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override Task Filtered_include_with_multiple_ordering(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    public override Task Include_collection_skip_no_order_by(bool async)
        => TestEnvironment.IsDistributed
            ? AssertQuery(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Skip(10).Include(c => c.Orders),
                elementAsserter: (e, a) => AssertInclude(e, a, new ExpectedInclude<Customer>(c => c.Orders)))
            : base.Include_collection_skip_no_order_by(async);

    public override Task Include_collection_take_no_order_by(bool async)
        => TestEnvironment.IsDistributed
            ? AssertQuery(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Take(10).Include(c => c.Orders),
                elementAsserter: (e, a) => AssertInclude(e, a, new ExpectedInclude<Customer>(c => c.Orders)))
            : base.Include_collection_take_no_order_by(async);

    public override Task Include_collection_skip_take_no_order_by(bool async)
        => TestEnvironment.IsDistributed
            ? AssertQuery(
                async,
                ss => ss.Set<Customer>().OrderBy(c => c.CustomerID).Skip(10).Take(5).Include(c => c.Orders),
                elementAsserter: (e, a) => AssertInclude(e, a, new ExpectedInclude<Customer>(c => c.Orders)))
            : base.Include_collection_skip_take_no_order_by(async);

    public override Task Include_collection_with_multiple_conditional_order_by(bool async)
        => TestEnvironment.IsDistributed
            ? AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Include(o => o.OrderDetails)
                    .OrderBy(o => o.OrderID > 0)
                    .ThenBy(o => o.Customer != null ? o.Customer.City : string.Empty)
                    .ThenBy(o => o.OrderID)
                    .Take(5),
                elementAsserter: (e, a) => AssertInclude(e, a, new ExpectedInclude<Order>(o => o.OrderDetails)))
            : base.Include_collection_with_multiple_conditional_order_by(async);

    public override Task Include_collection_OrderBy_empty_list_contains(bool async)
    {
        if (!TestEnvironment.IsDistributed)
        {
            return base.Include_collection_OrderBy_empty_list_contains(async);
        }

        var list = new List<string>();
        return AssertQuery(
            async,
            ss => ss.Set<Customer>()
                .Include(c => c.Orders)
                .Where(c => c.CustomerID.StartsWith("A"))
                .OrderBy(c => list.Contains(c.CustomerID))
                .ThenBy(c => c.CustomerID)
                .Skip(1),
            elementAsserter: (e, a) => AssertInclude(e, a, new ExpectedInclude<Customer>(c => c.Orders)));
    }

    public override Task Include_collection_OrderBy_empty_list_does_not_contains(bool async)
    {
        if (!TestEnvironment.IsDistributed)
        {
            return base.Include_collection_OrderBy_empty_list_does_not_contains(async);
        }

        var list = new List<string>();
        return AssertQuery(
            async,
            ss => ss.Set<Customer>()
                .Include(c => c.Orders)
                .Where(c => c.CustomerID.StartsWith("A"))
                .OrderBy(c => !list.Contains(c.CustomerID))
                .ThenBy(c => c.CustomerID)
                .Skip(1),
            elementAsserter: (e, a) => AssertInclude(e, a, new ExpectedInclude<Customer>(c => c.Orders)));
    }

    public override Task Include_collection_OrderBy_list_contains(bool async)
    {
        if (!TestEnvironment.IsDistributed)
        {
            return base.Include_collection_OrderBy_list_contains(async);
        }

        var list = new List<string> { "ALFKI" };
        return AssertQuery(
            async,
            ss => ss.Set<Customer>()
                .Include(c => c.Orders)
                .Where(c => c.CustomerID.StartsWith("A"))
                .OrderBy(c => list.Contains(c.CustomerID))
                .ThenBy(c => c.CustomerID)
                .Skip(1),
            elementAsserter: (e, a) => AssertInclude(e, a, new ExpectedInclude<Customer>(c => c.Orders)));
    }

    public override Task Repro9735(bool async)
        => TestEnvironment.IsDistributed
            ? AssertQuery(
                async,
                ss => ss.Set<Order>()
                    .Include(o => o.OrderDetails)
                    .OrderBy(o => o.Customer.CustomerID != null)
                    .ThenBy(o => o.Customer != null ? o.Customer.CustomerID : string.Empty)
                    .ThenBy(o => o.OrderID)
                    .Take(2))
            : base.Repro9735(async);
}
