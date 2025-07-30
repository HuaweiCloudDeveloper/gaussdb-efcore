using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;

// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Storage;

internal static class RelationalTypeMappingExtensions
{
    internal static string GenerateEmbeddedSqlLiteral(this RelationalTypeMapping mapping, object? value)
        => mapping is GaussDBTypeMapping npgsqlTypeMapping
            ? npgsqlTypeMapping.GenerateEmbeddedSqlLiteral(value)
            : mapping.GenerateSqlLiteral(value);
}
