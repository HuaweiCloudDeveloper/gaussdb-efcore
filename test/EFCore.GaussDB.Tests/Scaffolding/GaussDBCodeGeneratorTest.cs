using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
using HuaweiCloud.EntityFrameworkCore.GaussDB.NetTopologySuite.Scaffolding.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.NodaTime.Scaffolding.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Scaffolding.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Scaffolding;

public class GaussDBCodeGeneratorTest
{
    [Fact]
    public virtual void Use_provider_method_is_generated_correctly()
    {
        var codeGenerator = new GaussDBCodeGenerator(
            new ProviderCodeGeneratorDependencies(
                []));

        var result = codeGenerator.GenerateUseProvider("Server=test;Username=test;Password=test;Database=test", providerOptions: null);

        Assert.Equal("UseGaussDB", result.Method);
        Assert.Collection(
            result.Arguments,
            a => Assert.Equal("Server=test;Username=test;Password=test;Database=test", a));
        Assert.Null(result.ChainedCall);
    }

    [Fact]
    public virtual void Use_provider_method_is_generated_correctly_with_options()
    {
        var codeGenerator = new GaussDBCodeGenerator(
            new ProviderCodeGeneratorDependencies(
                []));

        var providerOptions = new MethodCallCodeFragment(_setProviderOptionMethodInfo);

        var result = codeGenerator.GenerateUseProvider("Server=test;Username=test;Password=test;Database=test", providerOptions);

        Assert.Equal("UseGaussDB", result.Method);
        Assert.Collection(
            result.Arguments,
            a => Assert.Equal("Server=test;Username=test;Password=test;Database=test", a),
            a =>
            {
                var nestedClosure = Assert.IsType<NestedClosureCodeFragment>(a);

                Assert.Equal("x", nestedClosure.Parameter);
                Assert.Same(providerOptions, nestedClosure.MethodCalls[0]);
            });
        Assert.Null(result.ChainedCall);
    }

    [ConditionalFact]
    public virtual void Use_provider_method_is_generated_correctly_with_NetTopologySuite()
    {
        var codeGenerator = new GaussDBCodeGenerator(
            new ProviderCodeGeneratorDependencies(
                [new GaussDBNetTopologySuiteCodeGeneratorPlugin()]));

        var result = ((IProviderConfigurationCodeGenerator)codeGenerator).GenerateUseProvider("Data Source=Test");

        Assert.Equal("UseGaussDB", result.Method);
        Assert.Collection(
            result.Arguments,
            a => Assert.Equal("Data Source=Test", a),
            a =>
            {
                var nestedClosure = Assert.IsType<NestedClosureCodeFragment>(a);

                Assert.Equal("x", nestedClosure.Parameter);
                Assert.Equal("UseNetTopologySuite", nestedClosure.MethodCalls[0].Method);
            });
        Assert.Null(result.ChainedCall);
    }

    [ConditionalFact]
    public virtual void Use_provider_method_is_generated_correctly_with_NodaTime()
    {
        var codeGenerator = new GaussDBCodeGenerator(
            new ProviderCodeGeneratorDependencies(
                [new GaussDBNodaTimeCodeGeneratorPlugin()]));

        var result = ((IProviderConfigurationCodeGenerator)codeGenerator).GenerateUseProvider("Data Source=Test");

        Assert.Equal("UseGaussDB", result.Method);
        Assert.Collection(
            result.Arguments,
            a => Assert.Equal("Data Source=Test", a),
            a =>
            {
                var nestedClosure = Assert.IsType<NestedClosureCodeFragment>(a);

                Assert.Equal("x", nestedClosure.Parameter);
                Assert.Equal("UseNodaTime", nestedClosure.MethodCalls[0].Method);
            });
        Assert.Null(result.ChainedCall);
    }

    private static readonly MethodInfo _setProviderOptionMethodInfo
        = typeof(GaussDBCodeGeneratorTest).GetRuntimeMethod(nameof(SetProviderOption), [typeof(DbContextOptionsBuilder)]);

    public static GaussDBDbContextOptionsBuilder SetProviderOption(DbContextOptionsBuilder optionsBuilder)
        => throw new NotSupportedException();
}
