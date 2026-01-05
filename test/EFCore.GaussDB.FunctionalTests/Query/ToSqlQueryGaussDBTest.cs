namespace Microsoft.EntityFrameworkCore.Query;

public class ToSqlQuerySqlServerTest(NonSharedFixture fixture) : ToSqlQueryTestBase(fixture)
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    [ConditionalFact]
    public virtual void Check_all_tests_overridden()
        => TestHelpers.AssertAllMethodsOverridden(GetType());

    // Base test implementation does not properly use identifier delimiters in raw SQL and isn't usable on GaussDB
    public override Task Entity_type_with_navigation_mapped_to_SqlQuery(bool async)
        => Task.CompletedTask;

    private void AssertSql(params string[] expected)
        => TestSqlLoggerFactory.AssertBaseline(expected);
}
