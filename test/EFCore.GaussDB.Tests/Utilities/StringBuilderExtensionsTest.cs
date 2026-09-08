using System.Text;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class StringBuilderExtensionsTest
{
    [Fact]
    public void AppendJoin_returns_the_same_builder()
    {
        var builder = new StringBuilder();

        var result = builder.AppendJoin(["one", "two"]);

        Assert.Same(builder, result);
    }

    [Fact]
    public void AppendJoin_does_not_change_builder_for_empty_values()
    {
        var builder = new StringBuilder("prefix");

        builder.AppendJoin(Array.Empty<string>());

        Assert.Equal("prefix", builder.ToString());
    }

    [Fact]
    public void AppendJoin_appends_single_value_without_separator()
    {
        var builder = new StringBuilder("prefix:");

        builder.AppendJoin(["value"]);

        Assert.Equal("prefix:value", builder.ToString());
    }

    [Fact]
    public void AppendJoin_uses_default_separator()
    {
        var builder = new StringBuilder();

        builder.AppendJoin(["one", "two", "three"]);

        Assert.Equal("one, two, three", builder.ToString());
    }

    [Theory]
    [InlineData("|", "one|two|three")]
    [InlineData(" / ", "one / two / three")]
    [InlineData("", "onetwothree")]
    [InlineData("\n", "one\ntwo\nthree")]
    public void AppendJoin_uses_custom_separator(string separator, string expected)
    {
        var builder = new StringBuilder();

        builder.AppendJoin(["one", "two", "three"], separator);

        Assert.Equal(expected, builder.ToString());
    }

    [Fact]
    public void AppendJoin_preserves_empty_values()
    {
        var builder = new StringBuilder();

        builder.AppendJoin(["one", "", "three"], "|");

        Assert.Equal("one||three", builder.ToString());
    }

    [Fact]
    public void AppendJoin_supports_lazy_enumerables()
    {
        var yieldedValues = 0;
        var builder = new StringBuilder();

        builder.AppendJoin(Values(), "|");

        Assert.Equal(3, yieldedValues);
        Assert.Equal("one|two|three", builder.ToString());

        IEnumerable<string> Values()
        {
            yieldedValues++;
            yield return "one";
            yieldedValues++;
            yield return "two";
            yieldedValues++;
            yield return "three";
        }
    }

    [Fact]
    public void Generic_AppendJoin_returns_the_same_builder()
    {
        var builder = new StringBuilder();

        var result = builder.AppendJoin([1, 2], (b, value) => b.Append(value), "|");

        Assert.Same(builder, result);
    }

    [Fact]
    public void Generic_AppendJoin_uses_the_supplied_action()
    {
        var builder = new StringBuilder("values:");

        builder.AppendJoin(
            [1, 2, 3],
            (b, value) => b.Append('[').Append(value * 2).Append(']'),
            ";");

        Assert.Equal("values:[2];[4];[6]", builder.ToString());
    }

    [Fact]
    public void Generic_AppendJoin_does_not_invoke_action_for_empty_values()
    {
        var invocationCount = 0;
        var builder = new StringBuilder("unchanged");

        builder.AppendJoin(
            Array.Empty<int>(),
            (b, value) =>
            {
                invocationCount++;
                b.Append(value);
            },
            "|");

        Assert.Equal(0, invocationCount);
        Assert.Equal("unchanged", builder.ToString());
    }

    [Fact]
    public void Generic_AppendJoin_invokes_action_once_per_value_in_order()
    {
        var visited = new List<int>();
        var builder = new StringBuilder();

        builder.AppendJoin(
            [3, 1, 2],
            (b, value) =>
            {
                visited.Add(value);
                b.Append(value);
            },
            ",");

        Assert.Equal([3, 1, 2], visited);
        Assert.Equal("3,1,2", builder.ToString());
    }
}
