using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities;
using HuaweiCloud.EntityFrameworkCore.GaussDB.ValueGeneration;
using HuaweiCloud.EntityFrameworkCore.GaussDB.ValueGeneration.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class GaussDBValueGeneratorSelectorTest
{
    [Fact]
    public void Returns_built_in_generators_for_types_setup_for_value_generation()
    {
        AssertGenerator<TemporaryIntValueGenerator>("Id");
        AssertGenerator<CustomValueGenerator>("Custom");
        AssertGenerator<TemporaryLongValueGenerator>("Long");
        AssertGenerator<TemporaryShortValueGenerator>("Short");
        AssertGenerator<TemporaryByteValueGenerator>("Byte");
        AssertGenerator<TemporaryIntValueGenerator>("NullableInt");
        AssertGenerator<TemporaryLongValueGenerator>("NullableLong");
        AssertGenerator<TemporaryShortValueGenerator>("NullableShort");
        AssertGenerator<TemporaryByteValueGenerator>("NullableByte");
        AssertGenerator<TemporaryDecimalValueGenerator>("Decimal");
        AssertGenerator<GaussDBSequentialStringValueGenerator>("String");
        AssertGenerator<GaussDBSequentialGuidValueGenerator>("Guid");
        AssertGenerator<BinaryValueGenerator>("Binary");
    }

    private void AssertGenerator<TExpected>(string propertyName, bool setSequences = false)
    {
        var builder = GaussDBTestHelpers.Instance.CreateConventionBuilder();
        builder.Entity<AnEntity>(
            b =>
            {
                b.Property(e => e.Custom).HasValueGenerator<CustomValueGenerator>();
                b.Property(propertyName).ValueGeneratedOnAdd();
                b.HasKey(propertyName);
            });

        if (setSequences)
        {
            builder.UseHiLo();
            Assert.NotNull(builder.Model.FindSequence(GaussDBModelExtensions.DefaultHiLoSequenceName));
        }

        var model = builder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(AnEntity));

        var selector = GaussDBTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

        var property = entityType.FindProperty(propertyName)!;
        Assert.True(selector.TrySelect(property, property.DeclaringType, out var generator));

        Assert.IsType<TExpected>(generator);
    }

    [ConditionalFact]
    public void Returns_temp_guid_generator_when_default_sql_set()
    {
        var builder = GaussDBTestHelpers.Instance.CreateConventionBuilder();
        builder.Entity<AnEntity>(
            b =>
            {
                b.Property(e => e.Guid).HasDefaultValueSql("newid()");
                b.HasKey(e => e.Guid);
            });
        var model = builder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(AnEntity));

        var selector = GaussDBTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

        var property = entityType.FindProperty("Guid")!;
        Assert.True(selector.TrySelect(property, property.DeclaringType, out var generator));
        Assert.IsType<TemporaryGuidValueGenerator>(generator);
    }

    [ConditionalFact]
    public void Returns_temp_string_generator_when_default_sql_set()
    {
        var builder = GaussDBTestHelpers.Instance.CreateConventionBuilder();
        builder.Entity<AnEntity>(
            b =>
            {
                b.Property(e => e.String).ValueGeneratedOnAdd().HasDefaultValueSql("Foo");
                b.HasKey(e => e.String);
            });
        var model = builder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(AnEntity));

        var selector = GaussDBTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

        var property = entityType.FindProperty("String")!;
        Assert.True(selector.TrySelect(property, property.DeclaringType, out var generator));

        Assert.IsType<TemporaryStringValueGenerator>(generator);
        Assert.True(generator.GeneratesTemporaryValues);
    }

    [ConditionalFact]
    public void Returns_temp_binary_generator_when_default_sql_set()
    {
        var builder = GaussDBTestHelpers.Instance.CreateConventionBuilder();
        builder.Entity<AnEntity>(
            b =>
            {
                b.HasKey(e => e.Binary);
                b.Property(e => e.Binary).HasDefaultValueSql("Foo").ValueGeneratedOnAdd();
            });
        var model = builder.FinalizeModel();
        var entityType = model.FindEntityType(typeof(AnEntity));

        var selector = GaussDBTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

        var property = entityType.FindProperty("Binary")!;
        Assert.True(selector.TrySelect(property, property.DeclaringType, out var generator));

        Assert.IsType<TemporaryBinaryValueGenerator>(generator);
        Assert.True(generator.GeneratesTemporaryValues);
    }

    [Fact]
    public void Returns_sequence_value_generators_when_configured_for_model()
    {
        AssertGenerator<GaussDBSequenceHiLoValueGenerator<int>>("Id", setSequences: true);
        AssertGenerator<CustomValueGenerator>("Custom", setSequences: true);
        AssertGenerator<GaussDBSequenceHiLoValueGenerator<long>>("Long", setSequences: true);
        AssertGenerator<GaussDBSequenceHiLoValueGenerator<short>>("Short", setSequences: true);
        AssertGenerator<GaussDBSequenceHiLoValueGenerator<int>>("NullableInt", setSequences: true);
        AssertGenerator<GaussDBSequenceHiLoValueGenerator<long>>("NullableLong", setSequences: true);
        AssertGenerator<GaussDBSequenceHiLoValueGenerator<short>>("NullableShort", setSequences: true);
        AssertGenerator<GaussDBSequentialStringValueGenerator>("String", setSequences: true);
        AssertGenerator<GaussDBSequentialGuidValueGenerator>("Guid", setSequences: true);
        AssertGenerator<BinaryValueGenerator>("Binary", setSequences: true);
    }

//        [ConditionalFact]
//        public void Throws_for_unsupported_combinations()
//        {
//            var builder = InMemoryTestHelpers.Instance.CreateConventionBuilder();
//            builder.Entity<AnEntity>(
//                b =>
//                {
//                    b.Property(e => e.Random).ValueGeneratedOnAdd();
//                    b.HasKey(e => e.Random);
//                });
//            var model = builder.FinalizeModel();
//            var entityType = model.FindEntityType(typeof(AnEntity));
//
//            var selector = InMemoryTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();
//
//            Assert.Equal(
//                CoreStrings.NoValueGenerator("Random", "AnEntity", "Something"),
//                Assert.Throws<NotSupportedException>(() => selector.Select(entityType.FindProperty("Random"), entityType)).Message);
//        }

    [ConditionalFact]
    public void Returns_generator_configured_on_model_when_property_is_identity()
    {
        var builder = GaussDBTestHelpers.Instance.CreateConventionBuilder();

        builder.Entity<AnEntity>();

        builder
            .UseHiLo()
            .HasSequence<int>(GaussDBModelExtensions.DefaultHiLoSequenceName);

        var model = builder.UseHiLo().FinalizeModel();
        var entityType = model.FindEntityType(typeof(AnEntity));

        var selector = GaussDBTestHelpers.Instance.CreateContextServices(model).GetRequiredService<IValueGeneratorSelector>();

        var property = entityType.FindProperty("Id")!;
        Assert.True(selector.TrySelect(property, property.DeclaringType, out var generator));

        Assert.IsType<GaussDBSequenceHiLoValueGenerator<int>>(generator);
    }

    private class AnEntity
    {
        // ReSharper disable UnusedMember.Local
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public int Id { get; set; }
        public int Custom { get; set; }
        public long Long { get; set; }
        public short Short { get; set; }
        public byte Byte { get; set; }
        public char Char { get; set; }
        public int? NullableInt { get; set; }
        public long? NullableLong { get; set; }
        public short? NullableShort { get; set; }
        public byte? NullableByte { get; set; }
        public char? NullableChar { get; set; }
        public string String { get; set; }
        public Guid Guid { get; set; }
        public byte[] Binary { get; set; }
        public float Float { get; set; }
        public decimal Decimal { get; set; }
        public int AlwaysIdentity { get; set; }
        public int AlwaysSequence { get; set; }

        [NotMapped]
        public Random Random { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class CustomValueGenerator : ValueGenerator<int>
    {
        public override int Next(EntityEntry entry)
            => throw new NotImplementedException();

        public override bool GeneratesTemporaryValues
            => false;
    }
}
