using Microsoft.EntityFrameworkCore.Metadata.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.Internal;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Metadata.Conventions;

/// <summary>
///     A convention that configures store value generation as <see cref="ValueGenerated.OnAdd" /> on properties that are
///     part of the primary key and not part of any foreign keys, were configured to have a database default value
///     or were configured to use a <see cref="GaussDBValueGenerationStrategy" />.
///     It also configures properties as <see cref="ValueGenerated.OnAddOrUpdate" /> if they were configured as computed columns.
/// </summary>
public class GaussDBValueGenerationConvention : RelationalValueGenerationConvention
{
    /// <summary>
    ///     Creates a new instance of <see cref="GaussDBValueGenerationConvention" />.
    /// </summary>
    /// <param name="dependencies">Parameter object containing dependencies for this convention.</param>
    /// <param name="relationalDependencies">Parameter object containing relational dependencies for this convention.</param>
    public GaussDBValueGenerationConvention(
        ProviderConventionSetBuilderDependencies dependencies,
        RelationalConventionSetBuilderDependencies relationalDependencies)
        : base(dependencies, relationalDependencies)
    {
    }

    /// <summary>
    ///     Called after an annotation is changed on a property.
    /// </summary>
    /// <param name="propertyBuilder">The builder for the property.</param>
    /// <param name="name">The annotation name.</param>
    /// <param name="annotation">The new annotation.</param>
    /// <param name="oldAnnotation">The old annotation.</param>
    /// <param name="context">Additional information associated with convention execution.</param>
    public override void ProcessPropertyAnnotationChanged(
        IConventionPropertyBuilder propertyBuilder,
        string name,
        IConventionAnnotation? annotation,
        IConventionAnnotation? oldAnnotation,
        IConventionContext<IConventionAnnotation> context)
    {
        if (name == GaussDBAnnotationNames.ValueGenerationStrategy)
        {
            propertyBuilder.ValueGenerated(GetValueGenerated(propertyBuilder.Metadata));
            return;
        }

        if (name == GaussDBAnnotationNames.TsVectorConfig && propertyBuilder.Metadata.GetTsVectorConfig() is not null)
        {
            propertyBuilder.ValueGenerated(ValueGenerated.OnAddOrUpdate);
            return;
        }

        base.ProcessPropertyAnnotationChanged(propertyBuilder, name, annotation, oldAnnotation, context);
    }

    /// <summary>
    ///     Returns the store value generation strategy to set for the given property.
    /// </summary>
    /// <param name="property">The property.</param>
    /// <returns>The store value generation strategy to set for the given property.</returns>
    protected override ValueGenerated? GetValueGenerated(IConventionProperty property)
    {
        // TODO: move to relational?
        if (property.DeclaringType.IsMappedToJson()
#pragma warning disable EF1001 // Internal EF Core API usage.
            && property.IsOrdinalKeyProperty()
#pragma warning restore EF1001 // Internal EF Core API usage.
            && (property.DeclaringType as IReadOnlyEntityType)?.FindOwnership()!.IsUnique == false)
        {
            return ValueGenerated.OnAdd;
        }

        var declaringTable = property.GetMappedStoreObjects(StoreObjectType.Table).FirstOrDefault();
        if (declaringTable.Name == null)
        {
            return null;
        }

        // If the first mapping can be value generated then we'll consider all mappings to be value generated
        // as this is a client-side configuration and can't be specified per-table.
        return GetValueGenerated(property, declaringTable, Dependencies.TypeMappingSource);
    }

    /// <summary>
    ///     Returns the store value generation strategy to set for the given property.
    /// </summary>
    /// <param name="property"> The property. </param>
    /// <param name="storeObject"> The identifier of the store object. </param>
    /// <returns>The store value generation strategy to set for the given property.</returns>
    public static new ValueGenerated? GetValueGenerated(IReadOnlyProperty property, in StoreObjectIdentifier storeObject)
        => RelationalValueGenerationConvention.GetValueGenerated(property, storeObject)
            ?? (property.GetValueGenerationStrategy(storeObject) != GaussDBValueGenerationStrategy.None
                ? ValueGenerated.OnAdd
                : null);

    private ValueGenerated? GetValueGenerated(
        IReadOnlyProperty property,
        in StoreObjectIdentifier storeObject,
        ITypeMappingSource typeMappingSource)
        => RelationalValueGenerationConvention.GetValueGenerated(property, storeObject)
            ?? (property.GetValueGenerationStrategy(storeObject, typeMappingSource) != GaussDBValueGenerationStrategy.None
                ? ValueGenerated.OnAdd
                : null);
}
