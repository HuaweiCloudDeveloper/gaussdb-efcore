﻿using HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.ValueGeneration.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public class GaussDBValueGeneratorSelector : RelationalValueGeneratorSelector
{
    private readonly IGaussDBSequenceValueGeneratorFactory _sequenceFactory;
    private readonly IGaussDBRelationalConnection _connection;
    private readonly IRawSqlCommandBuilder _rawSqlCommandBuilder;
    private readonly IRelationalCommandDiagnosticsLogger _commandLogger;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public GaussDBValueGeneratorSelector(
        ValueGeneratorSelectorDependencies dependencies,
        IGaussDBSequenceValueGeneratorFactory sequenceFactory,
        IGaussDBRelationalConnection connection,
        IRawSqlCommandBuilder rawSqlCommandBuilder,
        IRelationalCommandDiagnosticsLogger commandLogger)
        : base(dependencies)
    {
        _sequenceFactory = sequenceFactory;
        _connection = connection;
        _rawSqlCommandBuilder = rawSqlCommandBuilder;
        _commandLogger = commandLogger;
    }

    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public new virtual IGaussDBValueGeneratorCache Cache
        => (IGaussDBValueGeneratorCache)base.Cache;

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public override bool TrySelect(IProperty property, ITypeBase typeBase, out ValueGenerator? valueGenerator)
    {
        if (property.GetValueGeneratorFactory() != null
            || property.GetValueGenerationStrategy() != GaussDBValueGenerationStrategy.SequenceHiLo)
        {
            return base.TrySelect(property, typeBase, out valueGenerator);
        }

        var propertyType = property.ClrType.UnwrapNullableType().UnwrapEnumType();

        valueGenerator = _sequenceFactory.TryCreate(
            property,
            propertyType,
            Cache.GetOrAddSequenceState(property, _connection),
            _connection,
            _rawSqlCommandBuilder,
            _commandLogger);

        if (valueGenerator != null)
        {
            return true;
        }

        var converter = property.GetTypeMapping().Converter;
        if (converter != null
            && converter.ProviderClrType != propertyType)
        {
            valueGenerator = _sequenceFactory.TryCreate(
                property,
                converter.ProviderClrType,
                Cache.GetOrAddSequenceState(property, _connection),
                _connection,
                _rawSqlCommandBuilder,
                _commandLogger);

            if (valueGenerator != null)
            {
                valueGenerator = valueGenerator.WithConverter(converter);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override ValueGenerator? FindForType(IProperty property, ITypeBase typeBase, Type clrType)
        => property.ClrType.UnwrapNullableType() switch
        {
            var t when t == typeof(Guid) && property.ValueGenerated is not ValueGenerated.Never && property.GetDefaultValueSql() is null
                => new GaussDBSequentialGuidValueGenerator(),

            var t when t == typeof(string) && property.ValueGenerated is not ValueGenerated.Never && property.GetDefaultValueSql() is null
                => new GaussDBSequentialStringValueGenerator(),

            _ => base.FindForType(property, typeBase, clrType)
        };
}
