using Microsoft.EntityFrameworkCore.Metadata.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.Conventions;

public class GaussDBValueGenerationStrategyConventionTest
{
    [Fact]
    public void Annotations_are_added_when_conventional_model_builder_is_used()
    {
        var model = GaussDBTestHelpers.Instance.CreateConventionBuilder().Model;

        var annotations = model.GetAnnotations().OrderBy(a => a.Name).ToList();
        Assert.Equal(3, annotations.Count);

        Assert.Equal(GaussDBAnnotationNames.ValueGenerationStrategy, annotations.First().Name);
        Assert.Equal(GaussDBValueGenerationStrategy.IdentityByDefaultColumn, annotations.First().Value);
    }

    [Fact]
    public void Annotations_are_added_when_conventional_model_builder_is_used_with_sequences()
    {
        var model = GaussDBTestHelpers.Instance.CreateConventionBuilder()
            .UseHiLo()
            .Model;

        model.RemoveAnnotation(CoreAnnotationNames.ProductVersion);

        var annotations = model.GetAnnotations().OrderBy(a => a.Name).ToList();
        Assert.Equal(4, annotations.Count);

        Assert.Equal(GaussDBAnnotationNames.HiLoSequenceName, annotations[0].Name);
        Assert.Equal(GaussDBModelExtensions.DefaultHiLoSequenceName, annotations[0].Value);

        Assert.Equal(GaussDBAnnotationNames.ValueGenerationStrategy, annotations[1].Name);
        Assert.Equal(GaussDBValueGenerationStrategy.SequenceHiLo, annotations[1].Value);

        Assert.Equal(RelationalAnnotationNames.MaxIdentifierLength, annotations[2].Name);

        Assert.Equal(
            RelationalAnnotationNames.Sequences,
            annotations[3].Name);
        Assert.NotNull(annotations[3].Value);
    }
}
