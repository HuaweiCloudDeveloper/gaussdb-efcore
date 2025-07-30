namespace HuaweiCloud.EntityFrameworkCore.GaussDB.ValueGeneration;

/// <summary>
///     This API supports the Entity Framework Core infrastructure and is not intended to be used
///     directly from your code. This API may change or be removed in future releases.
/// </summary>
public class GaussDBSequentialStringValueGenerator : ValueGenerator<string>
{
    private readonly GaussDBSequentialGuidValueGenerator _guidGenerator = new();

    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public override bool GeneratesTemporaryValues => false;

    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public override string Next(EntityEntry entry) => _guidGenerator.Next(entry).ToString();
}
