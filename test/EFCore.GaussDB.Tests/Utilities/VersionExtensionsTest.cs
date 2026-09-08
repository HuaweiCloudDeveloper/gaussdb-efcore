namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class VersionExtensionsTest
{
    [Fact]
    public void Null_version_is_at_least_every_version()
    {
        Version version = null;

        Assert.True(version.AtLeast(0));
        Assert.True(version.AtLeast(18));
        Assert.True(version.AtLeast(int.MaxValue, int.MaxValue));
    }

    [Theory]
    [InlineData("14.0", 14, 0, true)]
    [InlineData("14.0", 13, 9, true)]
    [InlineData("14.0", 14, 1, false)]
    [InlineData("14.1", 14, 0, true)]
    [InlineData("14.1", 14, 1, true)]
    [InlineData("14.1", 15, 0, false)]
    [InlineData("15.2.3", 15, 2, true)]
    [InlineData("15.2.3", 15, 3, false)]
    [InlineData("18.0.0.1", 18, 0, true)]
    public void AtLeast_compares_major_and_minor_components(
        string value,
        int major,
        int minor,
        bool expected)
    {
        var version = Version.Parse(value);

        Assert.Equal(expected, version.AtLeast(major, minor));
    }

    [Theory]
    [InlineData("14.0", 14, true)]
    [InlineData("14.9", 14, true)]
    [InlineData("13.9", 14, false)]
    [InlineData("15.0", 14, true)]
    public void AtLeast_defaults_minor_to_zero(string value, int major, bool expected)
    {
        var version = Version.Parse(value);

        Assert.Equal(expected, version.AtLeast(major));
    }

    [Fact]
    public void Null_version_is_never_under_a_version()
    {
        Version version = null;

        Assert.False(version.IsUnder(0));
        Assert.False(version.IsUnder(18));
        Assert.False(version.IsUnder(int.MaxValue, int.MaxValue));
    }

    [Theory]
    [InlineData("14.0", 14, 0, false)]
    [InlineData("14.0", 14, 1, true)]
    [InlineData("14.0", 13, 9, false)]
    [InlineData("14.1", 14, 0, false)]
    [InlineData("14.1", 14, 2, true)]
    [InlineData("14.1", 15, 0, true)]
    [InlineData("15.2.3", 15, 2, false)]
    [InlineData("15.2.3", 15, 3, true)]
    [InlineData("18.0.0.1", 18, 0, false)]
    public void IsUnder_compares_major_and_minor_components(
        string value,
        int major,
        int minor,
        bool expected)
    {
        var version = Version.Parse(value);

        Assert.Equal(expected, version.IsUnder(major, minor));
    }

    [Theory]
    [InlineData("14.0", 14, false)]
    [InlineData("14.9", 14, false)]
    [InlineData("13.9", 14, true)]
    [InlineData("15.0", 14, false)]
    public void IsUnder_defaults_minor_to_zero(string value, int major, bool expected)
    {
        var version = Version.Parse(value);

        Assert.Equal(expected, version.IsUnder(major));
    }

    [Theory]
    [InlineData("12.9", 13, 0)]
    [InlineData("13.0", 13, 0)]
    [InlineData("13.1", 13, 0)]
    public void AtLeast_and_IsUnder_are_complementary_for_non_null_versions(
        string value,
        int major,
        int minor)
    {
        var version = Version.Parse(value);

        Assert.NotEqual(version.AtLeast(major, minor), version.IsUnder(major, minor));
    }
}
