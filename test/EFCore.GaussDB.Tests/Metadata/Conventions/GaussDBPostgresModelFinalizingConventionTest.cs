using System.ComponentModel.DataAnnotations;
using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.Conventions;

public class GaussDBPostgresModelFinalizingConventionTest
{
    [Fact]
    public void RowVersion_properties_get_mapped_to_xmin()
    {
        var modelBuilder = GaussDBTestHelpers.Instance.CreateConventionBuilder();
        modelBuilder.Entity<Blog>().Property(b => b.RowVersion).IsRowVersion();
        var model = modelBuilder.FinalizeModel();

        var entityType = model.FindEntityType(typeof(Blog))!;
        var property = entityType.FindProperty(nameof(Blog.RowVersion))!;

        Assert.Equal("xmin", property.GetColumnName());
        Assert.Equal("xid", property.GetColumnType());
    }

    private class Blog
    {
        public int Id { get; set; }

        [Timestamp]
        public uint RowVersion { get; set; }
    }
}
