using System.Reflection;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB;

public class ReflectionExtensionsTest
{
    [Fact]
    public void IsClosedFormOf_returns_true_for_closed_generic_method()
    {
        var definition = GetMethod("Generic");
        var closed = definition.MakeGenericMethod(typeof(int));

        Assert.True(closed.IsClosedFormOf(definition));
    }

    [Fact]
    public void IsClosedFormOf_returns_true_for_different_type_arguments()
    {
        var definition = GetMethod("Generic");

        Assert.True(definition.MakeGenericMethod(typeof(string)).IsClosedFormOf(definition));
        Assert.True(definition.MakeGenericMethod(typeof(Guid)).IsClosedFormOf(definition));
    }

    [Fact]
    public void IsClosedFormOf_returns_false_for_non_generic_method()
    {
        var definition = GetMethod("Generic");
        var nonGeneric = GetMethod("NonGeneric");

        Assert.False(nonGeneric.IsClosedFormOf(definition));
    }

    [Fact]
    public void IsClosedFormOf_returns_false_for_another_generic_definition()
    {
        var definition = GetMethod("Generic");
        var anotherDefinition = GetMethod("AnotherGeneric");
        var closed = anotherDefinition.MakeGenericMethod(typeof(int));

        Assert.False(closed.IsClosedFormOf(definition));
    }

    [Fact]
    public void IsClosedFormOf_returns_false_for_definition_with_different_arity()
    {
        var definition = GetMethod("Generic");
        var twoArgumentDefinition = GetMethod("TwoArgumentGeneric");
        var closed = twoArgumentDefinition.MakeGenericMethod(typeof(int), typeof(string));

        Assert.False(closed.IsClosedFormOf(definition));
    }

    [Fact]
    public void GetMemberType_returns_property_type()
    {
        var property = typeof(MemberBase).GetProperty(nameof(MemberBase.Name));

        Assert.Same(typeof(string), property.GetMemberType());
    }

    [Fact]
    public void GetMemberType_returns_field_type()
    {
        var field = typeof(MemberBase).GetField(nameof(MemberBase.Count));

        Assert.Same(typeof(int), field.GetMemberType());
    }

    [Fact]
    public void IsSameAs_treats_two_null_members_as_same()
    {
        MemberInfo first = null;
        MemberInfo second = null;

        Assert.True(first.IsSameAs(second));
    }

    [Fact]
    public void IsSameAs_treats_null_and_non_null_members_as_different()
    {
        MemberInfo member = typeof(MemberBase).GetProperty(nameof(MemberBase.Name));

        Assert.False(member.IsSameAs(null));
        Assert.False(((MemberInfo)null).IsSameAs(member));
    }

    [Fact]
    public void IsSameAs_returns_true_for_same_property_instance()
    {
        var property = typeof(MemberBase).GetProperty(nameof(MemberBase.Name));

        Assert.True(property.IsSameAs(property));
    }

    [Fact]
    public void IsSameAs_matches_overridden_property_to_base_property()
    {
        var baseProperty = typeof(MemberBase).GetProperty(nameof(MemberBase.Name));
        var derivedProperty = typeof(MemberDerived).GetProperty(nameof(MemberDerived.Name));

        Assert.True(baseProperty.IsSameAs(derivedProperty));
        Assert.True(derivedProperty.IsSameAs(baseProperty));
    }

    [Fact]
    public void IsSameAs_matches_interface_property_to_implementation()
    {
        var interfaceProperty = typeof(IHasName).GetProperty(nameof(IHasName.Name));
        var implementationProperty = typeof(InterfaceImplementation).GetProperty(nameof(InterfaceImplementation.Name));

        Assert.True(interfaceProperty.IsSameAs(implementationProperty));
        Assert.True(implementationProperty.IsSameAs(interfaceProperty));
    }

    [Fact]
    public void IsSameAs_rejects_same_name_on_unrelated_types()
    {
        var first = typeof(MemberBase).GetProperty(nameof(MemberBase.Name));
        var second = typeof(UnrelatedMember).GetProperty(nameof(UnrelatedMember.Name));

        Assert.False(first.IsSameAs(second));
        Assert.False(second.IsSameAs(first));
    }

    [Fact]
    public void IsSameAs_rejects_different_member_names()
    {
        var first = typeof(MemberBase).GetProperty(nameof(MemberBase.Name));
        var second = typeof(MemberBase).GetField(nameof(MemberBase.Count));

        Assert.False(first.IsSameAs(second));
    }

    [Fact]
    public void GetSimpleMemberName_returns_regular_member_name()
    {
        var property = typeof(MemberBase).GetProperty(nameof(MemberBase.Name));

        Assert.Equal("Name", property.GetSimpleMemberName());
    }

    [Fact]
    public void GetSimpleMemberName_removes_explicit_interface_prefix()
    {
        var property = typeof(ExplicitInterfaceImplementation)
            .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single();

        Assert.EndsWith(".Name", property.Name);
        Assert.Equal("Name", property.GetSimpleMemberName());
    }

    [Fact]
    public void IsStatic_detects_static_getter()
    {
        var property = typeof(PropertyHost).GetProperty(nameof(PropertyHost.StaticValue));

        Assert.True(property.IsStatic());
    }

    [Fact]
    public void IsStatic_detects_static_setter_only_property()
    {
        var property = typeof(PropertyHost).GetProperty(
            "StaticWriteOnly",
            BindingFlags.Static | BindingFlags.NonPublic);

        Assert.True(property.IsStatic());
    }

    [Fact]
    public void IsStatic_rejects_instance_property()
    {
        var property = typeof(PropertyHost).GetProperty(nameof(PropertyHost.InstanceValue));

        Assert.False(property.IsStatic());
    }

    [Fact]
    public void IsIndexerProperty_accepts_single_string_indexer()
    {
        var property = typeof(IndexerHost).GetProperty("Item", [typeof(string)]);

        Assert.True(property.IsIndexerProperty());
    }

    [Fact]
    public void IsIndexerProperty_rejects_single_integer_indexer()
    {
        var property = typeof(IndexerHost).GetProperty("Item", [typeof(int)]);

        Assert.False(property.IsIndexerProperty());
    }

    [Fact]
    public void IsIndexerProperty_rejects_multi_parameter_indexer()
    {
        var property = typeof(IndexerHost).GetProperty("Item", [typeof(string), typeof(string)]);

        Assert.False(property.IsIndexerProperty());
    }

    [Fact]
    public void IsIndexerProperty_rejects_regular_property()
    {
        var property = typeof(IndexerHost).GetProperty(nameof(IndexerHost.Value));

        Assert.False(property.IsIndexerProperty());
    }

    [Fact]
    public void IsGenericList_handles_null_type()
    {
        Type type = null;

        Assert.False(type.IsGenericList());
    }

    [Theory]
    [InlineData(typeof(List<int>), true)]
    [InlineData(typeof(List<string>), true)]
    [InlineData(typeof(List<>), true)]
    [InlineData(typeof(IList<int>), false)]
    [InlineData(typeof(IEnumerable<int>), false)]
    [InlineData(typeof(DerivedList), false)]
    [InlineData(typeof(int[]), false)]
    [InlineData(typeof(string), false)]
    public void IsGenericList_recognizes_only_List_of_T(Type type, bool expected)
        => Assert.Equal(expected, type.IsGenericList());

    [Theory]
    [InlineData(typeof(int[]), true)]
    [InlineData(typeof(int[,]), true)]
    [InlineData(typeof(List<int>), true)]
    [InlineData(typeof(IList<int>), false)]
    [InlineData(typeof(IEnumerable<int>), false)]
    [InlineData(typeof(DerivedList), false)]
    [InlineData(typeof(string), false)]
    public void IsArrayOrGenericList_recognizes_supported_collections(Type type, bool expected)
        => Assert.Equal(expected, type.IsArrayOrGenericList());

    [Theory]
    [InlineData(typeof(int[]), typeof(int))]
    [InlineData(typeof(int[,]), typeof(int))]
    [InlineData(typeof(string[]), typeof(string))]
    [InlineData(typeof(List<Guid>), typeof(Guid))]
    [InlineData(typeof(List<List<int>>), typeof(List<int>))]
    public void TryGetElementType_returns_supported_element_type(Type type, Type expected)
    {
        var result = type.TryGetElementType(out var elementType);

        Assert.True(result);
        Assert.Same(expected, elementType);
    }

    [Theory]
    [InlineData(typeof(string))]
    [InlineData(typeof(IList<int>))]
    [InlineData(typeof(IEnumerable<int>))]
    [InlineData(typeof(DerivedList))]
    [InlineData(typeof(Dictionary<string, int>))]
    public void TryGetElementType_rejects_unsupported_type(Type type)
    {
        var result = type.TryGetElementType(out var elementType);

        Assert.False(result);
        Assert.Null(elementType);
    }

    [Theory]
    [InlineData(typeof(GaussDBRange<int>), true)]
    [InlineData(typeof(GaussDBRange<DateTime>), true)]
    [InlineData(typeof(global::NodaTime.Interval), true)]
    [InlineData(typeof(global::NodaTime.DateInterval), true)]
    [InlineData(typeof(int), false)]
    [InlineData(typeof(List<int>), false)]
    [InlineData(typeof(GaussDBRange<>), true)]
    public void IsRange_recognizes_supported_range_types(Type type, bool expected)
        => Assert.Equal(expected, type.IsRange());

    [Theory]
    [InlineData(typeof(GaussDBRange<int>[]), true)]
    [InlineData(typeof(List<GaussDBRange<int>>), true)]
    [InlineData(typeof(global::NodaTime.Interval[]), true)]
    [InlineData(typeof(List<global::NodaTime.DateInterval>), true)]
    [InlineData(typeof(int[]), false)]
    [InlineData(typeof(List<int>), false)]
    [InlineData(typeof(IEnumerable<GaussDBRange<int>>), false)]
    [InlineData(typeof(GaussDBRange<int>[][]), false)]
    public void IsMultirange_recognizes_arrays_and_lists_of_ranges(Type type, bool expected)
        => Assert.Equal(expected, type.IsMultirange());

    [Fact]
    public void FindIndexerProperty_returns_default_indexer()
    {
        var property = typeof(IndexerHost).FindIndexerProperty();

        Assert.NotNull(property);
        Assert.Equal("Item", property.Name);
        Assert.Single(property.GetIndexParameters());
    }

    [Fact]
    public void FindIndexerProperty_supports_framework_dictionary()
    {
        var property = typeof(Dictionary<string, int>).FindIndexerProperty();

        Assert.NotNull(property);
        Assert.Equal("Item", property.Name);
        Assert.Equal(typeof(string), Assert.Single(property.GetIndexParameters()).ParameterType);
    }

    [Fact]
    public void FindIndexerProperty_returns_null_without_default_member()
        => Assert.Null(typeof(MemberBase).FindIndexerProperty());

    private static MethodInfo GetMethod(string name)
        => typeof(MethodHost).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);

    private static class MethodHost
    {
        private static T Generic<T>(T value) => value;

        private static T AnotherGeneric<T>(T value) => value;

        private static TFirst TwoArgumentGeneric<TFirst, TSecond>(TFirst first, TSecond second) => first;

        private static int NonGeneric(int value) => value;
    }

    private interface IHasName
    {
        string Name { get; }
    }

    private class MemberBase
    {
        public virtual string Name { get; set; } = "base";

        public int Count = 0;
    }

    private sealed class MemberDerived : MemberBase
    {
        public override string Name { get; set; } = "derived";
    }

    private sealed class InterfaceImplementation : IHasName
    {
        public string Name => "implementation";
    }

    private sealed class ExplicitInterfaceImplementation : IHasName
    {
        string IHasName.Name => "explicit";
    }

    private sealed class UnrelatedMember
    {
        public string Name => "unrelated";
    }

    private sealed class PropertyHost
    {
        public static string StaticValue => "static";

        public string InstanceValue => "instance";

        private static string StaticWriteOnly
        {
            set => _ = value;
        }
    }

    private sealed class IndexerHost
    {
        public string Value => "value";

        public string this[string key] => key;

        public int this[int index] => index;

        public string this[string first, string second] => first + second;
    }

    private sealed class DerivedList : List<int>;
}
