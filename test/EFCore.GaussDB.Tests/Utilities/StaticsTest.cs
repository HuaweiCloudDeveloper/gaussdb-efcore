using HuaweiCloud.EntityFrameworkCore.GaussDB.Utilities;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class StaticsTest
{
    [Fact]
    public void TrueArrays_contains_expected_lengths()
    {
        Assert.Equal(9, Statics.TrueArrays.Length);

        for (var length = 0; length < Statics.TrueArrays.Length; length++)
        {
            Assert.Equal(length, Statics.TrueArrays[length].Length);
        }
    }

    [Fact]
    public void Every_value_in_TrueArrays_is_true()
    {
        foreach (var values in Statics.TrueArrays)
        {
            Assert.All(values, Assert.True);
        }
    }

    [Fact]
    public void TrueArrays_entries_are_distinct_instances()
    {
        for (var i = 0; i < Statics.TrueArrays.Length; i++)
        {
            for (var j = i + 1; j < Statics.TrueArrays.Length; j++)
            {
                Assert.NotSame(Statics.TrueArrays[i], Statics.TrueArrays[j]);
            }
        }
    }

    [Fact]
    public void FalseArrays_contains_expected_lengths()
    {
        Assert.Equal(4, Statics.FalseArrays.Length);

        for (var length = 0; length < Statics.FalseArrays.Length; length++)
        {
            Assert.Equal(length, Statics.FalseArrays[length].Length);
        }
    }

    [Fact]
    public void Every_value_in_FalseArrays_is_false()
    {
        foreach (var values in Statics.FalseArrays)
        {
            Assert.All(values, value => Assert.False(value));
        }
    }

    [Fact]
    public void FalseArrays_entries_are_distinct_instances()
    {
        for (var i = 0; i < Statics.FalseArrays.Length; i++)
        {
            for (var j = i + 1; j < Statics.FalseArrays.Length; j++)
            {
                Assert.NotSame(Statics.FalseArrays[i], Statics.FalseArrays[j]);
            }
        }
    }

    [Fact]
    public void Empty_true_and_false_arrays_reuse_empty_array_singleton()
        => Assert.Same(Statics.TrueArrays[0], Statics.FalseArrays[0]);
}
