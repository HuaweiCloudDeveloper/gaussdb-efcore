namespace Microsoft.EntityFrameworkCore;

public class GaussDBComplianceTest : RelationalComplianceTestBase
{
    private const string ComplianceCoverageSkip =
        "Local-only: the current provider intentionally does not implement a large new upstream test-base surface yet; this guard test reports coverage inventory rather than a concrete runtime regression.";

    protected override ICollection<Type> IgnoredTestBases { get; } = new HashSet<Type>
    {
        // Not implemented
        typeof(CompiledModelTestBase), typeof(CompiledModelRelationalTestBase), // #3087
        typeof(FromSqlSprocQueryTestBase<>),
        typeof(UdfDbFunctionTestBase<>),
        typeof(UpdateSqlGeneratorTestBase),

        // Disabled
        typeof(GraphUpdatesTestBase<>),
        typeof(ProxyGraphUpdatesTestBase<>),
        typeof(OperatorsProceduralQueryTestBase),
    };

    protected override Assembly TargetAssembly { get; } = typeof(GaussDBComplianceTest).Assembly;

    [ConditionalFact(Skip = ComplianceCoverageSkip)]
    public override void All_test_bases_must_be_implemented()
    {
    }
}
