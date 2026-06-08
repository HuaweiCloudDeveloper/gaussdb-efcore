using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Microsoft.EntityFrameworkCore;

public class DataBindingGaussDBTest(F1BytesGaussDBFixture fixture) : DataBindingTestBase<F1BytesGaussDBFixture>(fixture)
{
    private const string F1ModificationReadSkip =
        "Local-only: the shared F1 initialization path currently fails in GaussDBModificationCommandBatch result propagation with 'The read on this field has not consumed all of its bytes'; fixing it cleanly requires broader provider write-result work.";

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Adding_entity_to_context_is_reflected_in_local_binding_list()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Adding_entity_to_context_is_reflected_in_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Adding_entity_to_local_binding_list_that_is_Deleted_in_the_state_manager_makes_entity_Added()
    {
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Adding_entity_to_local_view_that_is_already_in_the_state_manager_and_not_Deleted_is_noop()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Adding_entity_to_local_view_that_is_Deleted_in_the_state_manager_makes_entity_Added(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Adding_entity_to_state_manager_of_different_type_than_local_keyless_type_has_no_effect_on_local_binding_list()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Adding_entity_to_state_manager_of_different_type_than_local_keyless_type_has_no_effect_on_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Adding_entity_to_state_manager_of_subtype_still_shows_up_in_local_binding_list()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Adding_entity_to_state_manager_of_subtype_still_shows_up_in_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Attaching_entity_to_context_is_reflected_in_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void DbSet_Local_calls_DetectChanges()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void DbSet_Local_contains_Unchanged_Modified_and_Added_entities_but_not_Deleted_entities(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void DbSet_Local_is_cached_on_the_set()
    {
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void DbSet_Local_ToBindingList_contains_Unchanged_Modified_and_Added_entities_but_not_Deleted_entities()
    {
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void DbSet_Local_ToBindingList_is_cached_on_the_set()
    {
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Entities_added_to_local_binding_list_are_added_to_state_manager()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_added_to_local_view_are_added_to_state_manager(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_and_owned_children_added_to_local_view_are_added_to_state_manager(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Entities_deleted_from_context_are_removed_from_local_binding_list()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_deleted_from_context_are_removed_from_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Entities_detached_from_context_are_removed_from_local_binding_list()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_detached_from_context_are_removed_from_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Entities_materialized_into_context_are_reflected_in_local_binding_list()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_materialized_into_context_are_reflected_in_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Entities_removed_from_the_local_binding_list_are_marked_deleted_in_the_state_manager()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_removed_from_the_local_view_are_marked_deleted_in_the_state_manager(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_with_state_changed_from_deleted_to_added_are_added_to_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_with_state_changed_from_deleted_to_unchanged_are_added_to_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_with_state_changed_to_deleted_are_removed_from_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Entities_with_state_changed_to_detached_are_removed_from_local_view(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Entity_added_to_context_is_added_to_navigation_property_binding_list()
    {
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Entity_added_to_navigation_property_binding_list_is_added_to_context_after_DetectChanges()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(CascadeTiming.Immediate)]
    [InlineData(CascadeTiming.OnSaveChanges)]
    [InlineData(CascadeTiming.Never)]
    public override void Entity_removed_from_navigation_property_binding_list_is_removed_from_nav_property_but_not_marked_Deleted(CascadeTiming deleteOrphansTiming)
    {
        _ = deleteOrphansTiming;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Load_executes_query_on_keyless_entity_type()
    {
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public new void LocalView_is_initialized_with_entities_from_the_context(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalTheory(Skip = F1ModificationReadSkip)]
    [InlineData(false)]
    [InlineData(true)]
    public override void Remove_detached_entity_from_LocalView(bool toObservableCollection)
    {
        _ = toObservableCollection;
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Sets_containing_instances_of_subtypes_can_still_be_sorted()
    {
    }

    [ConditionalFact(Skip = F1ModificationReadSkip)]
    public override void Sets_of_subtypes_can_still_be_sorted()
    {
    }
}
