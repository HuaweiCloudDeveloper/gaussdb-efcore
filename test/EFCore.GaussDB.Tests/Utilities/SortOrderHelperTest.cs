using HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Utilities;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class SortOrderHelperTest
{
    [Fact]
    public void Null_sort_orders_are_default()
        => Assert.True(SortOrderHelper.IsDefaultNullSortOrder(null, [false]));

    [Theory]
    [InlineData(false, NullSortOrder.NullsLast, true)]
    [InlineData(false, NullSortOrder.NullsFirst, false)]
    [InlineData(false, NullSortOrder.Unspecified, false)]
    [InlineData(true, NullSortOrder.NullsFirst, true)]
    [InlineData(true, NullSortOrder.NullsLast, false)]
    [InlineData(true, NullSortOrder.Unspecified, false)]
    public void Detects_default_for_single_column(
        bool isDescending,
        NullSortOrder nullSortOrder,
        bool expected)
        => Assert.Equal(
            expected,
            SortOrderHelper.IsDefaultNullSortOrder([nullSortOrder], [isDescending]));

    [Fact]
    public void Empty_descending_values_means_all_columns_are_descending()
        => Assert.True(
            SortOrderHelper.IsDefaultNullSortOrder(
                [NullSortOrder.NullsFirst, NullSortOrder.NullsFirst],
                []));

    [Fact]
    public void Detects_default_for_mixed_column_sort_orders()
        => Assert.True(
            SortOrderHelper.IsDefaultNullSortOrder(
                [NullSortOrder.NullsLast, NullSortOrder.NullsFirst, NullSortOrder.NullsLast],
                [false, true, false]));

    [Fact]
    public void Detects_non_default_in_mixed_column_sort_orders()
        => Assert.False(
            SortOrderHelper.IsDefaultNullSortOrder(
                [NullSortOrder.NullsLast, NullSortOrder.NullsFirst, NullSortOrder.NullsFirst],
                [false, true, false]));
}
