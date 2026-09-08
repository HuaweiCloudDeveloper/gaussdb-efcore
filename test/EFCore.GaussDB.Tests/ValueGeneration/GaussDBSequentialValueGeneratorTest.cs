using HuaweiCloud.EntityFrameworkCore.GaussDB.ValueGeneration;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class GaussDBSequentialValueGeneratorTest
{
    [Fact]
    public void Guid_generator_creates_non_empty_value()
    {
        var generator = new GaussDBSequentialGuidValueGenerator();

        var value = generator.Next(null);

        Assert.NotEqual(Guid.Empty, value);
    }

    [Fact]
    public void Guid_generator_creates_version_7_value()
    {
        var generator = new GaussDBSequentialGuidValueGenerator();

        var value = generator.Next(null);

        Assert.Equal(7, value.Version);
    }

    [Fact]
    public void Guid_generator_creates_rfc_4122_variant()
    {
        var generator = new GaussDBSequentialGuidValueGenerator();

        var value = generator.Next(null);
        var bytes = value.ToByteArray(bigEndian: true);

        Assert.Equal(0x80, bytes[8] & 0xC0);
    }

    [Fact]
    public void Guid_generator_creates_unique_values()
    {
        var generator = new GaussDBSequentialGuidValueGenerator();
        var values = new HashSet<Guid>();

        for (var i = 0; i < 1_000; i++)
        {
            Assert.True(values.Add(generator.Next(null)));
        }

        Assert.Equal(1_000, values.Count);
    }

    [Fact]
    public void Guid_generator_values_are_permanent()
    {
        var generator = new GaussDBSequentialGuidValueGenerator();

        Assert.False(generator.GeneratesTemporaryValues);
    }

    [Fact]
    public void String_generator_creates_non_empty_value()
    {
        var generator = new GaussDBSequentialStringValueGenerator();

        var value = generator.Next(null);

        Assert.False(string.IsNullOrWhiteSpace(value));
    }

    [Fact]
    public void String_generator_creates_canonical_guid_text()
    {
        var generator = new GaussDBSequentialStringValueGenerator();

        var value = generator.Next(null);

        Assert.Equal(36, value.Length);
        Assert.Equal(Guid.Parse(value).ToString(), value);
    }

    [Fact]
    public void String_generator_wraps_version_7_guid()
    {
        var generator = new GaussDBSequentialStringValueGenerator();

        var value = Guid.Parse(generator.Next(null));

        Assert.Equal(7, value.Version);
    }

    [Fact]
    public void String_generator_creates_unique_values()
    {
        var generator = new GaussDBSequentialStringValueGenerator();
        var values = new HashSet<string>();

        for (var i = 0; i < 1_000; i++)
        {
            Assert.True(values.Add(generator.Next(null)));
        }

        Assert.Equal(1_000, values.Count);
    }

    [Fact]
    public void String_generator_values_are_permanent()
    {
        var generator = new GaussDBSequentialStringValueGenerator();

        Assert.False(generator.GeneratesTemporaryValues);
    }
}
