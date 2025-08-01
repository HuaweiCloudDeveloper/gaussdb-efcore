using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.ValueConversion;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Storage;

public class GaussDBArrayValueConverterTest
{
    private static readonly ValueConverter<Beatles[], int[]> EnumArrayToNumberArray
        = new GaussDBArrayConverter<Beatles[], Beatles[], int[]>(new EnumToNumberConverter<Beatles, int>());

    [ConditionalFact]
    public void Can_convert_enum_arrays_to_number_arrays()
    {
        var converter = EnumArrayToNumberArray.ConvertToProviderExpression.Compile();

        Assert.Equal(new[] { 7 }, converter([Beatles.John]));
        Assert.Equal(new[] { 4 }, converter([Beatles.Paul]));
        Assert.Equal(new[] { 1 }, converter([Beatles.George]));
        Assert.Equal(new[] { -1 }, converter([Beatles.Ringo]));
        Assert.Equal(new[] { 77 }, converter([(Beatles)77]));
        Assert.Equal(new[] { 0 }, converter([default(Beatles)]));
        Assert.Null(converter(null));
    }

    [ConditionalFact]
    public void Can_convert_enum_arrays_to_number_arrays_object()
    {
        var converter = EnumArrayToNumberArray.ConvertToProvider;

        Assert.Equal(new[] { 7 }, converter(new[] { Beatles.John }));
        Assert.Equal(new[] { 4 }, converter(new[] { Beatles.Paul }));
        Assert.Equal(new[] { 1 }, converter(new[] { Beatles.George }));
        Assert.Equal(new[] { -1 }, converter(new[] { Beatles.Ringo }));
        Assert.Equal(new[] { 77 }, converter(new[] { (Beatles)77 }));
        Assert.Equal(new[] { 0 }, converter(new[] { default(Beatles) }));
        Assert.Null(converter(null));
    }

    [ConditionalFact]
    public void Can_convert_number_arrays_to_enum_arrays()
    {
        var converter = EnumArrayToNumberArray.ConvertFromProviderExpression.Compile();

        Assert.Equal([Beatles.John], converter([7]));
        Assert.Equal([Beatles.Paul], converter([4]));
        Assert.Equal([Beatles.George], converter([1]));
        Assert.Equal([Beatles.Ringo], converter([-1]));
        Assert.Equal([(Beatles)77], converter([77]));
        Assert.Equal([default(Beatles)], converter([0]));
        Assert.Null(converter(null));
    }

    [ConditionalFact]
    public void Can_convert_number_arrays_to_enum_arrays_object()
    {
        var converter = EnumArrayToNumberArray.ConvertFromProvider;

        Assert.Equal(new[] { Beatles.John }, converter(new[] { 7 }));
        Assert.Equal(new[] { Beatles.Paul }, converter(new[] { 4 }));
        Assert.Equal(new[] { Beatles.George }, converter(new[] { 1 }));
        Assert.Equal(new[] { Beatles.Ringo }, converter(new[] { -1 }));
        Assert.Equal(new[] { (Beatles)77 }, converter(new[] { 77 }));
        Assert.Equal(new[] { default(Beatles) }, converter(new[] { 0 }));
        Assert.Null(converter(null));
    }

    private enum Beatles
    {
        John = 7,
        Paul = 4,
        George = 1,
        Ringo = -1
    }
}
