using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.ValueGeneration.Internal;

/// <summary>
///     This API supports the Entity Framework Core infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
public interface INpgsqlSequenceValueGeneratorFactory
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    ValueGenerator? TryCreate(
        IProperty property,
        Type clrType,
        NpgsqlSequenceValueGeneratorState generatorState,
        INpgsqlRelationalConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder,
        IRelationalCommandDiagnosticsLogger commandLogger);
}
