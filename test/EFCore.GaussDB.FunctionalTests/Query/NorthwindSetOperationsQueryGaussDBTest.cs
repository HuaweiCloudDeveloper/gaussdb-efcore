using Microsoft.EntityFrameworkCore.TestModels.Northwind;

namespace Microsoft.EntityFrameworkCore.Query;

public class NorthwindSetOperationsQueryGaussDBTest
    : NorthwindSetOperationsQueryRelationalTestBase<NorthwindQueryGaussDBFixture<NoopModelCustomizer>>
{
    // ReSharper disable once UnusedParameter.Local
    public NorthwindSetOperationsQueryGaussDBTest(
        NorthwindQueryGaussDBFixture<NoopModelCustomizer> fixture,
        ITestOutputHelper testOutputHelper)
        : base(fixture)
    {
        ClearLog();
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Leftmost_nulls_with_two_unions(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID })
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)o.OrderID, o.CustomerID })));

        AssertSql(
            """
SELECT NULL AS "OrderID", o."CustomerID"
FROM "Orders" AS o
UNION
SELECT o0."OrderID", o0."CustomerID"
FROM "Orders" AS o0
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Leftmost_nulls_with_three_unions(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID })
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID }))
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)o.OrderID, o.CustomerID })));

        AssertSql(
            """
SELECT NULL::int AS "OrderID", o."CustomerID"
FROM "Orders" AS o
UNION
SELECT NULL AS "OrderID", o0."CustomerID"
FROM "Orders" AS o0
UNION
SELECT o1."OrderID", o1."CustomerID"
FROM "Orders" AS o1
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Leftmost_nulls_with_four_unions(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID })
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID }))
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID }))
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)o.OrderID, o.CustomerID })));

        AssertSql(
            """
SELECT NULL::int AS "OrderID", o."CustomerID"
FROM "Orders" AS o
UNION
SELECT NULL AS "OrderID", o0."CustomerID"
FROM "Orders" AS o0
UNION
SELECT NULL AS "OrderID", o1."CustomerID"
FROM "Orders" AS o1
UNION
SELECT o2."OrderID", o2."CustomerID"
FROM "Orders" AS o2
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Leftmost_nulls_with_five_unions(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID })
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID }))
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID }))
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID }))
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)o.OrderID, o.CustomerID })));

        AssertSql(
            """
SELECT NULL::int AS "OrderID", o."CustomerID"
FROM "Orders" AS o
UNION
SELECT NULL AS "OrderID", o0."CustomerID"
FROM "Orders" AS o0
UNION
SELECT NULL AS "OrderID", o1."CustomerID"
FROM "Orders" AS o1
UNION
SELECT NULL AS "OrderID", o2."CustomerID"
FROM "Orders" AS o2
UNION
SELECT o3."OrderID", o3."CustomerID"
FROM "Orders" AS o3
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public virtual async Task Leftmost_nulls_in_tables_and_predicate(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID })
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID }))
                .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)o.OrderID, o.CustomerID }))
                .Where(
                    o => o.CustomerID
                        == ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID })
                            .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)null, o.CustomerID }))
                            .Union(ss.Set<Order>().Select(o => new { OrderID = (int?)o.OrderID, o.CustomerID }))
                            .OrderBy(o => o.CustomerID)
                            .First()
                            .CustomerID));

        AssertSql(
            """
SELECT u0."OrderID", u0."CustomerID"
FROM (
    SELECT NULL::int AS "OrderID", o."CustomerID"
    FROM "Orders" AS o
    UNION
    SELECT NULL AS "OrderID", o0."CustomerID"
    FROM "Orders" AS o0
    UNION
    SELECT o1."OrderID", o1."CustomerID"
    FROM "Orders" AS o1
) AS u0
WHERE u0."CustomerID" = (
    SELECT u2."CustomerID"
    FROM (
        SELECT NULL::int AS "OrderID", o2."CustomerID"
        FROM "Orders" AS o2
        UNION
        SELECT NULL AS "OrderID", o3."CustomerID"
        FROM "Orders" AS o3
        UNION
        SELECT o4."OrderID", o4."CustomerID"
        FROM "Orders" AS o4
    ) AS u2
    ORDER BY u2."CustomerID" NULLS FIRST
    LIMIT 1) OR (u0."CustomerID" IS NULL AND (
    SELECT u2."CustomerID"
    FROM (
        SELECT NULL::int AS "OrderID", o2."CustomerID"
        FROM "Orders" AS o2
        UNION
        SELECT NULL AS "OrderID", o3."CustomerID"
        FROM "Orders" AS o3
        UNION
        SELECT o4."OrderID", o4."CustomerID"
        FROM "Orders" AS o4
    ) AS u2
    ORDER BY u2."CustomerID" NULLS FIRST
    LIMIT 1) IS NULL)
""");
    }

    public override async Task Client_eval_Union_FirstOrDefault(bool async)
    {
        // Client evaluation in projection. Issue #16243.
        Assert.Equal(
            RelationalStrings.SetOperationsNotAllowedAfterClientEvaluation,
            (await Assert.ThrowsAsync<InvalidOperationException>(
                () => base.Client_eval_Union_FirstOrDefault(async))).Message);

        AssertSql();
    }

    private void AssertSql(params string[] expected)
        => Fixture.TestSqlLoggerFactory.AssertBaseline(expected);

    protected override void ClearLog()
        => Fixture.TestSqlLoggerFactory.Clear();
}
