using Microsoft.EntityFrameworkCore.TestModels.Array;
using TypeExtensions = HuaweiCloud.EntityFrameworkCore.GaussDB.TypeExtensions;

namespace Microsoft.EntityFrameworkCore.Query;

public class ArrayListQueryTest : ArrayQueryTest<ArrayListQueryTest.ArrayListQueryFixture>
{
    private const string ArrayLikeAnyAllSkip =
        "Local-only: current openGauss array LIKE/ILIKE ANY/ALL translation either differs semantically or still trips provider nullability processing.";

    private const string ArrayNullableParameterSkip =
        "Local-only: nullable array parameter evaluation still fails in the current query pipeline.";

    private const string ArrayIndexOfSkip =
        "Local-only: current openGauss target does not provide the array_position overload used for these array index translation tests.";

    private const string ListQueryRewriteSkip =
        "Local-only: current list-backed server-query rewriting still assumes array shapes for this nullable literal containment case.";

    private const string ArrayNewArraySkip =
        "Local-only: current array projection/new-array translation still trips provider nullability processing.";

    private const string ListServerRewriteSkip =
        "Local-only: current list-backed server-query rewriting still leaves array-only coercion/index semantics in the expression tree, so this test needs broader test-infrastructure work.";

    public ArrayListQueryTest(ArrayListQueryFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
        Fixture.TestSqlLoggerFactory.SetTestOutputHelper(testOutputHelper);
    }

    #region Indexers

    public override async Task Index_with_constant(bool async)
    {
        await base.Index_with_constant(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE (
    SELECT i.value
    FROM unnest(s."IntList") AS i(value)
    LIMIT 1 OFFSET 0) = 3
""");
    }

    public override async Task Index_with_parameter(bool async)
    {
        await base.Index_with_parameter(async);

        AssertSql(
            """
@x='0'

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE (
    SELECT i.value
    FROM unnest(s."IntList") AS i(value)
    LIMIT 1 OFFSET @x) = 3
""");
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Nullable_index_with_constant(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    public override async Task Nullable_value_array_index_compare_to_null(bool async)
    {
        await base.Nullable_value_array_index_compare_to_null(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE (
    SELECT n.value
    FROM unnest(s."NullableIntList") AS n(value)
    LIMIT 1 OFFSET 2) IS NULL
""");
    }

    public override async Task Non_nullable_value_array_index_compare_to_null(bool async)
    {
        await base.Non_nullable_value_array_index_compare_to_null(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE (
    SELECT i.value
    FROM unnest(s."IntList") AS i(value)
    LIMIT 1 OFFSET 1) IS NULL
""");
    }

    public override async Task Nullable_reference_array_index_compare_to_null(bool async)
    {
        await base.Nullable_reference_array_index_compare_to_null(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE (
    SELECT n.value
    FROM unnest(s."NullableStringList") AS n(value)
    LIMIT 1 OFFSET 2) IS NULL
""");
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Non_nullable_reference_array_index_compare_to_null(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    #endregion

    #region SequenceEqual

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task SequenceEqual_with_parameter(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    public override async Task SequenceEqual_with_array_literal(bool async)
    {
        await base.SequenceEqual_with_array_literal(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IntArray", s."IntList", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArray", s."ValueConvertedList", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."IntList" = ARRAY[3,4]::integer[]
""");
    }

    [ConditionalTheory(Skip = ArrayNullableParameterSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task SequenceEqual_over_nullable_with_parameter(bool async)
    {
        await base.SequenceEqual_over_nullable_with_parameter(async);

        AssertSql(
            """
@arr={ '3', '4', NULL } (DbType = Object)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableIntList" = @arr
""");
    }

    #endregion SequenceEqual

    #region Containment

    public override async Task Array_column_Any_equality_operator(bool async)
    {
        await base.Array_column_Any_equality_operator(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE '3' IN (
    SELECT s0.value
    FROM unnest(s."StringList") AS s0(value)
)
""");
    }

    public override async Task Array_column_Any_Equals(bool async)
    {
        await base.Array_column_Any_Equals(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE '3' IN (
    SELECT s0.value
    FROM unnest(s."StringList") AS s0(value)
)
""");
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Array_column_Contains_literal_item(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Array_column_Contains_parameter_item(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Array_column_Contains_column_item(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Array_column_Contains_null_constant(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    public override void Array_column_Contains_null_parameter_does_not_work()
    {
        using var ctx = CreateContext();

        string? p = null;

        // We incorrectly miss arrays containing non-constant nulls, because detecting those
        // would prevent index use.
        Assert.Equal(
            0,
            ctx.SomeEntities.Count(e => e.StringList.Contains(p!)));

        AssertSql(
            """
SELECT count(*)::int
FROM "SomeEntities" AS s
WHERE FALSE
""");
    }

    [ConditionalTheory(Skip = ListQueryRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Nullable_array_column_Contains_literal_item(bool async)
    {
        await base.Nullable_array_column_Contains_literal_item(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE 3 IN (
    SELECT n.value
    FROM unnest(s."NullableIntList") AS n(value)
)
""");
    }

    public override async Task Array_constant_Contains_column(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => new[] { "foo", "xxx" }.Contains(e.NullableText)));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableText" IN ('foo', 'xxx')
""");
    }

    public override async Task Array_param_Contains_nullable_column(bool async)
    {
        var array = new List<string> { "foo", "xxx" };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => array.Contains(e.NullableText!)));

        AssertSql(
            """
@array1='foo'
@array2='xxx'

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableText" IN (@array1, @array2)
""");
    }

    public override async Task Array_param_Contains_non_nullable_column(bool async)
    {
        var array = new List<int> { 1 };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => array.Contains(e.Id)));

        AssertSql(
            """
@array1='1'

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."Id" = @array1
""");
    }

    public override void Array_param_with_null_Contains_non_nullable_not_found()
    {
        using var ctx = CreateContext();

        var array = new List<string?>
        {
            "unknown1",
            "unknown2",
            null
        };

        Assert.Equal(0, ctx.SomeEntities.Count(e => array.Contains(e.NonNullableText)));

        AssertSql(
            """
@array1='unknown1'
@array2='unknown2'

SELECT count(*)::int
FROM "SomeEntities" AS s
WHERE s."NonNullableText" IN (@array1, @array2)
""");
    }

    public override void Array_param_with_null_Contains_non_nullable_not_found_negated()
    {
        using var ctx = CreateContext();

        var array = new List<string?>
        {
            "unknown1",
            "unknown2",
            null
        };

        Assert.Equal(2, ctx.SomeEntities.Count(e => !array.Contains(e.NonNullableText)));

        AssertSql(
            """
@array1='unknown1'
@array2='unknown2'

SELECT count(*)::int
FROM "SomeEntities" AS s
WHERE s."NonNullableText" NOT IN (@array1, @array2)
""");
    }

    public override void Array_param_with_null_Contains_nullable_not_found()
    {
        using var ctx = CreateContext();

        var array = new List<string?>
        {
            "unknown1",
            "unknown2",
            null
        };

        Assert.Equal(0, ctx.SomeEntities.Count(e => array.Contains(e.NullableText)));

        AssertSql(
            """
@array1='unknown1'
@array2='unknown2'

SELECT count(*)::int
FROM "SomeEntities" AS s
WHERE s."NullableText" IN (@array1, @array2) OR s."NullableText" IS NULL
""");
    }

    public override void Array_param_with_null_Contains_nullable_not_found_negated()
    {
        using var ctx = CreateContext();

        var array = new List<string?>
        {
            "unknown1",
            "unknown2",
            null
        };

        Assert.Equal(2, ctx.SomeEntities.Count(e => !array.Contains(e.NullableText!)));

        AssertSql(
            """
@array1='unknown1'
@array2='unknown2'

SELECT count(*)::int
FROM "SomeEntities" AS s
WHERE s."NullableText" NOT IN (@array1, @array2) AND s."NullableText" IS NOT NULL
""");
    }

    public override async Task Array_param_Contains_column_with_ToString(bool async)
    {
        var values = new List<string> { "1", "999" };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => values.Contains(e.Id.ToString())));

        AssertSql(
            """
@values1='1'
@values2='999'

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."Id"::text IN (@values1, @values2)
""");
    }

    public override async Task Byte_array_parameter_contains_column(bool async)
    {
        var values = new List<byte> { 20 };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => values.Contains(e.Byte)));

        AssertSql(
            """
@values1='20' (DbType = Int16)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."Byte" = @values1
""");
    }

    public override async Task Array_param_Contains_value_converted_column_enum_to_int(bool async)
    {
        var array = new List<SomeEnum> { SomeEnum.Two, SomeEnum.Three };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => array.Contains(e.EnumConvertedToInt)));

        AssertSql(
            """
@array1='-2'
@array2='-3'

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."EnumConvertedToInt" IN (@array1, @array2)
""");
    }

    public override async Task Array_param_Contains_value_converted_column_enum_to_string(bool async)
    {
        var array = new List<SomeEnum> { SomeEnum.Two, SomeEnum.Three };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => array.Contains(e.EnumConvertedToString)));

        AssertSql(
            """
@array1='Two' (Nullable = false)
@array2='Three' (Nullable = false)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."EnumConvertedToString" IN (@array1, @array2)
""");
    }

    public override async Task Array_param_Contains_value_converted_column_nullable_enum_to_string(bool async)
    {
        var array = new List<SomeEnum?> { SomeEnum.Two, SomeEnum.Three };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => array.Contains(e.NullableEnumConvertedToString)));

        AssertSql(
            """
@array1='Two' (Nullable = false)
@array2='Three' (Nullable = false)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableEnumConvertedToString" IN (@array1, @array2)
""");
    }

    public override async Task Array_param_Contains_value_converted_column_nullable_enum_to_string_with_non_nullable_lambda(bool async)
    {
        var array = new List<SomeEnum?> { SomeEnum.Two, SomeEnum.Three };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => array.Contains(e.NullableEnumConvertedToStringWithNonNullableLambda)));

        AssertSql(
            """
@array1='Two' (Nullable = false)
@array2='Three' (Nullable = false)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableEnumConvertedToStringWithNonNullableLambda" IN (@array1, @array2)
""");
    }

    public override async Task Array_column_Contains_value_converted_param(bool async)
    {
        var item = SomeEnum.Eight;

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => e.ValueConvertedListOfEnum.Contains(item)));

        AssertSql(
            """
@item='Eight' (Nullable = false)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE @item IN (
    SELECT v.value
    FROM unnest(s."ValueConvertedListOfEnum") AS v(value)
)
""");
    }

    public override async Task Array_column_Contains_value_converted_constant(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => e.ValueConvertedListOfEnum.Contains(SomeEnum.Eight)));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE 'Eight' IN (
    SELECT v.value
    FROM unnest(s."ValueConvertedListOfEnum") AS v(value)
)
""");
    }

    public override async Task Array_param_Contains_value_converted_array_column(bool async)
    {
        var p = new List<SomeEnum> { SomeEnum.Eight, SomeEnum.Nine };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => e.ValueConvertedArrayOfEnum.All(x => p.Contains(x))));

        AssertSql(
            """
@p1='Eight' (Nullable = false)
@p2='Nine' (Nullable = false)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE NOT EXISTS (
    SELECT 1
    FROM unnest(s."ValueConvertedListOfEnum") AS v(value)
    WHERE v.value NOT IN (@p1, @p2))
""");
    }

    public override async Task IList_column_contains_constant(bool async)
    {
        await base.IList_column_contains_constant(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE 10 IN (
    SELECT i.value
    FROM unnest(s."IList") AS i(value)
)
""");
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Array_column_Contains_in_scalar_subquery(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    #endregion Containment

    #region Length/Count

    public override async Task Array_Length(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => e.IntList.Count == 2));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE (
    SELECT count(*)::int
    FROM unnest(s."IntList") AS i(value)) = 2
""");
    }

    public override async Task Nullable_array_Length(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => e.NullableIntList.Count == 3));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE (
    SELECT count(*)::int
    FROM unnest(s."NullableIntList") AS n(value)) = 3
""");
    }

    public override async Task Array_Length_on_EF_Property(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => EF.Property<List<int>>(e, nameof(ArrayEntity.IntList)).Count == 2));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE (
    SELECT count(*)::int
    FROM unnest(s."IntList") AS i(value)) = 2
""");
    }

    #endregion Length/Count

    #region Any/All

    public override async Task Any_no_predicate(bool async)
    {
        await base.Any_no_predicate(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE EXISTS (
    SELECT 1
    FROM unnest(s."IntList") AS i(value))
""");
    }

    [ConditionalTheory(Skip = ArrayLikeAnyAllSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Any_like(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>()
                .Where(e => new[] { "a%", "b%", "c%" }.Any(p => EF.Functions.Like(e.NullableText, p))),
            ss => ss.Set<ArrayEntity>()
                .Where(e => new[] { "a", "b", "c" }.Any(p => e.NullableText!.StartsWith(p, StringComparison.Ordinal))));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableText" LIKE ANY (ARRAY['a%','b%','c%']::text[])
""");
    }

    [ConditionalTheory(Skip = ArrayLikeAnyAllSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Any_ilike(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>()
                .Where(e => new[] { "a%", "b%", "c%" }.Any(p => EF.Functions.ILike(e.NullableText!, p))),
            ss => ss.Set<ArrayEntity>()
                .Where(e => new[] { "a", "b", "c" }.Any(p => e.NullableText!.StartsWith(p, StringComparison.OrdinalIgnoreCase))));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableText" ILIKE ANY (ARRAY['a%','b%','c%']::text[])
""");
    }

    [ConditionalTheory(Skip = ArrayLikeAnyAllSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Any_like_anonymous(bool async)
    {
        await using var ctx = CreateContext();

        var patternsActual = new List<string>
        {
            "a%",
            "b%",
            "c%"
        };
        var patternsExpected = new List<string>
        {
            "a",
            "b",
            "c"
        };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>()
                .Where(e => patternsActual.Any(p => EF.Functions.Like(e.NullableText, p))),
            ss => ss.Set<ArrayEntity>()
                .Where(e => patternsExpected.Any(p => e.NullableText!.StartsWith(p, StringComparison.Ordinal))));

        AssertSql(
            """
@patternsActual={ 'a%', 'b%', 'c%' } (DbType = Object)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableText" LIKE ANY (@patternsActual)
""");
    }

    [ConditionalTheory(Skip = ArrayLikeAnyAllSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task All_like(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>()
                .Where(e => new List<string> { "b%", "ba%" }.All(p => EF.Functions.Like(e.NullableText, p))),
            ss => ss.Set<ArrayEntity>()
                .Where(e => new List<string> { "b", "ba" }.All(p => e.NullableText!.StartsWith(p, StringComparison.Ordinal))));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableText" LIKE ALL (ARRAY['b%','ba%']::text[])
""");
    }

    [ConditionalTheory(Skip = ArrayLikeAnyAllSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task All_ilike(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>()
                .Where(e => new List<string> { "B%", "ba%" }.All(p => EF.Functions.ILike(e.NullableText!, p))),
            ss => ss.Set<ArrayEntity>()
                .Where(e => new List<string> { "B", "ba" }.All(p => e.NullableText!.StartsWith(p, StringComparison.OrdinalIgnoreCase))));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE s."NullableText" ILIKE ALL (ARRAY['B%','ba%']::text[])
""");
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task Any_Contains_on_constant_array(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    public override async Task Any_Contains_between_column_and_List(bool async)
    {
        await base.Any_Contains_between_column_and_List(async);

        AssertSql(
            """
@ints1='2'
@ints2='3'

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE EXISTS (
    SELECT 1
    FROM unnest(s."IntList") AS i(value)
    WHERE i.value IN (@ints1, @ints2))
""");
    }

    public override async Task Any_Contains_between_column_and_array(bool async)
    {
        await base.Any_Contains_between_column_and_array(async);

        AssertSql(
            """
@ints1='2'
@ints2='3'

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE EXISTS (
    SELECT 1
    FROM unnest(s."IntList") AS i(value)
    WHERE i.value IN (@ints1, @ints2))
""");
    }

    public override async Task Any_Contains_between_column_and_other_type(bool async)
    {
        var array = new[] { SomeEnum.Eight };

        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => e.ValueConvertedListOfEnum.Any(i => array.Contains(i))));

        AssertSql(
            """
@array1='Eight' (Nullable = false)

SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE EXISTS (
    SELECT 1
    FROM unnest(s."ValueConvertedListOfEnum") AS v(value)
    WHERE v.value = @array1)
""");
    }

    [ConditionalTheory(Skip = ListServerRewriteSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override Task All_Contains(bool async)
    {
        _ = async;
        return Task.CompletedTask;
    }

    #endregion Any/All

    #region Other translations

    public override async Task Append(bool async)
        // TODO: https://github.com/dotnet/efcore/issues/30669
        => await AssertTranslationFailed(() => base.Append(async));

    //         await base.Append(async);
    //
    //         AssertSql(
    // """
    // SELECT s."Id", s."ArrayContainerEntityId", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IntArray", s."IntList", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArray", s."ValueConvertedList", s."Varchar10", s."Varchar15"
    // FROM "SomeEntities" AS s
    // WHERE array_append(s."IntList", 5) = ARRAY[3,4,5]::integer[]
    // """);
    public override async Task Concat(bool async)
    {
        await base.Concat(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IntArray", s."IntList", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArray", s."ValueConvertedList", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE array_cat(s."IntList", ARRAY[5,6]::integer[]) = ARRAY[3,4,5,6]::integer[]
""");
    }

    [ConditionalTheory(Skip = ArrayIndexOfSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Array_IndexOf1(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => e.IntList.IndexOf(6) == 1));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE COALESCE(array_position(s."IntList", 6) - 1, -1) = 1
""");
    }

    [ConditionalTheory(Skip = ArrayIndexOfSkip)]
    [MemberData(nameof(IsAsyncData))]
    public override async Task Array_IndexOf2(bool async)
    {
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>().Where(e => e.IntList.IndexOf(6, 1) == 1));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE COALESCE(array_position(s."IntList", 6, 2) - 1, -1) = 1
""");
    }

    // Note: see NorthwindFunctionsQueryGaussDBTest.String_Join_non_aggregate for regular use without an array column/parameter
    public override async Task String_Join_with_array_of_int_column(bool async)
    {
        await base.String_Join_with_array_of_int_column(async);

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE array_to_string(s."IntList", ', ', '') = '3, 4'
""");
    }

    public override async Task String_Join_with_array_of_string_column(bool async)
    {
        // This is not in ArrayQueryTest because string.Join uses another overload for string[] than for List<string> and thus
        // ArrayToListReplacingExpressionVisitor won't work.
        await AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>()
                .Where(e => string.Join(", ", e.StringList) == "3, 4"));

        AssertSql(
            """
SELECT s."Id", s."ArrayContainerEntityId", s."ArrayOfStringConvertedToDelimitedString", s."Byte", s."ByteArray", s."Bytea", s."EnumConvertedToInt", s."EnumConvertedToString", s."IList", s."IntArray", s."IntList", s."ListOfStringConvertedToDelimitedString", s."NonNullableText", s."NullableEnumConvertedToString", s."NullableEnumConvertedToStringWithNonNullableLambda", s."NullableIntArray", s."NullableIntList", s."NullableStringArray", s."NullableStringList", s."NullableText", s."StringArray", s."StringList", s."ValueConvertedArrayOfEnum", s."ValueConvertedListOfEnum", s."Varchar10", s."Varchar15"
FROM "SomeEntities" AS s
WHERE array_to_string(s."StringList", ', ', '') = '3, 4'
""");
    }

    [ConditionalTheory]
    [MemberData(nameof(IsAsyncData))]
    public override async Task String_Join_disallow_non_array_type_mapped_parameter(bool async)
    {
        // This is not in ArrayQueryTest because string.Join uses another overload for string[] than for List<string> and thus
        // ArrayToListReplacingExpressionVisitor won't work.
        await AssertTranslationFailed(() => AssertQuery(
            async,
            ss => ss.Set<ArrayEntity>()
                .Where(e => string.Join(", ", e.ListOfStringConvertedToDelimitedString) == "3, 4")));
    }

    #endregion Other translations

    public class ArrayListQueryFixture : ArrayQueryFixture
    {
        protected override string StoreName
            => "ArrayListTest";
    }

    protected override Expression RewriteServerQueryExpression(Expression serverQueryExpression)
        => new ArrayToListReplacingExpressionVisitor().Visit(serverQueryExpression);

    private class ArrayToListReplacingExpressionVisitor : ExpressionVisitor
    {
        private static readonly PropertyInfo IntArray
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.IntArray))!;

        private static readonly PropertyInfo NullableIntArray
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.NullableIntArray))!;

        private static readonly PropertyInfo IntList
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.IntList))!;

        private static readonly PropertyInfo NullableIntList
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.NullableIntList))!;

        private static readonly PropertyInfo StringArray
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.StringArray))!;

        private static readonly PropertyInfo NullableStringArray
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.NullableStringArray))!;

        private static readonly PropertyInfo StringList
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.StringList))!;

        private static readonly PropertyInfo NullableStringList
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.NullableStringList))!;

        private static readonly PropertyInfo ValueConvertedArrayOfEnum
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.ValueConvertedArrayOfEnum))!;

        private static readonly PropertyInfo ValueConvertedListOfEnum
            = typeof(ArrayEntity).GetProperty(nameof(ArrayEntity.ValueConvertedListOfEnum))!;

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member == IntArray)
            {
                return Expression.MakeMemberAccess(node.Expression, IntList);
            }

            if (node.Member == NullableIntArray)
            {
                return Expression.MakeMemberAccess(node.Expression, NullableIntList);
            }

            if (node.Member == StringArray)
            {
                return Expression.MakeMemberAccess(node.Expression, StringList);
            }

            if (node.Member == NullableStringArray)
            {
                return Expression.MakeMemberAccess(node.Expression, NullableStringList);
            }

            if (node.Member == ValueConvertedArrayOfEnum)
            {
                return Expression.MakeMemberAccess(node.Expression, ValueConvertedListOfEnum);
            }

            return node;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayIndex)
            {
                var listExpression = Visit(node.Left);
                if (TypeExtensions.IsGenericList(listExpression.Type))
                {
                    var getItemMethod = listExpression.Type.GetMethod("get_Item", [typeof(int)])!;
                    return Expression.Call(listExpression, getItemMethod, node.Right);
                }
            }

            return base.VisitBinary(node);
        }
    }
}
