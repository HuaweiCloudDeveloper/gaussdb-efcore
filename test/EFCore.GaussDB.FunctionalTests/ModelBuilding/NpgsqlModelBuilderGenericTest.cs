using Xunit.Sdk;

namespace Microsoft.EntityFrameworkCore.ModelBuilding;

public class GaussDBModelBuilderGenericTest : GaussDBModelBuilderTestBase
{
    public class GaussDBGenericNonRelationship(GaussDBModelBuilderFixture fixture) : GaussDBNonRelationship(fixture)
    {
        // GaussDB actually does support mapping multi-dimensional arrays, so no exception is thrown as expected
        protected override void Mapping_throws_for_non_ignored_three_dimensional_array()
            => Assert.Throws<ThrowsException>(() => base.Mapping_throws_for_non_ignored_three_dimensional_array());

        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }

    public class GaussDBGenericComplexType(GaussDBModelBuilderFixture fixture) : GaussDBComplexType(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }

    public class GaussDBGenericComplexCollection(GaussDBModelBuilderFixture fixture) : GaussDBComplexCollection(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }

    public class GaussDBGenericInheritance(GaussDBModelBuilderFixture fixture) : GaussDBInheritance(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }

    public class GaussDBGenericOneToMany(GaussDBModelBuilderFixture fixture) : GaussDBOneToMany(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }

    public class GaussDBGenericManyToOne(GaussDBModelBuilderFixture fixture) : GaussDBManyToOne(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }

    public class GaussDBGenericOneToOne(GaussDBModelBuilderFixture fixture) : GaussDBOneToOne(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }

    public class GaussDBGenericManyToMany(GaussDBModelBuilderFixture fixture) : GaussDBManyToMany(fixture)
    {
        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }

    public class GaussDBGenericOwnedTypes(GaussDBModelBuilderFixture fixture) : GaussDBOwnedTypes(fixture)
    {
        // GaussDB stored procedures do not support result columns
        public override void Can_use_sproc_mapping_with_owned_reference()
            => Assert.Throws<InvalidOperationException>(() => base.Can_use_sproc_mapping_with_owned_reference());

        protected override TestModelBuilder CreateModelBuilder(
            Action<ModelConfigurationBuilder>? configure)
            => new GenericTestModelBuilder(Fixture, configure);
    }
}
