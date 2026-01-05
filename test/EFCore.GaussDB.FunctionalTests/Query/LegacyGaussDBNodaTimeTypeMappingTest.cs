using Microsoft.EntityFrameworkCore.Storage.Json;
using NodaTime;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal;
using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;

#if DEBUG

namespace Microsoft.EntityFrameworkCore.Query
{
    [Collection("LegacyNodaTimeTest")]
    public class LegacyGaussDBNodaTimeTypeMappingTest
        : IClassFixture<LegacyGaussDBNodaTimeTypeMappingTest.LegacyGaussDBNodaTimeTypeMappingFixture>
    {
        [Fact]
        public void Timestamp_maps_to_Instant_by_default()
            => Assert.Same(typeof(Instant), GetMapping("timestamp without time zone")!.ClrType);

        [Fact]
        public void Timestamptz_maps_to_Instant_by_default()
            => Assert.Same(typeof(Instant), GetMapping("timestamp with time zone")!.ClrType);

        [Fact]
        public void LocalDateTime_does_not_map_to_timestamptz()
            => Assert.Null(GetMapping(typeof(LocalDateTime), "timestamp with time zone"));

        [Fact]
        public void GenerateSqlLiteral_returns_instant_literal()
        {
            var mapping = GetMapping(typeof(Instant))!;
            Assert.Equal("timestamp without time zone", mapping.StoreType);

            var instant = (new LocalDateTime(2018, 4, 20, 10, 31, 33, 666) + Period.FromTicks(6660)).InUtc().ToInstant();
            Assert.Equal("TIMESTAMP '2018-04-20T10:31:33.666666Z'", mapping.GenerateSqlLiteral(instant));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_instant_infinity_literal()
        {
            var mapping = GetMapping(typeof(Instant))!;
            Assert.Equal(typeof(Instant), mapping.ClrType);
            Assert.Equal("timestamp without time zone", mapping.StoreType);

            Assert.Equal("TIMESTAMP '-infinity'", mapping.GenerateSqlLiteral(Instant.MinValue));
            Assert.Equal("TIMESTAMP 'infinity'", mapping.GenerateSqlLiteral(Instant.MaxValue));
        }

        [Fact]
        public void GenerateSqlLiteral_returns_instant_range_in_legacy_mode()
        {
            var mapping = (GaussDBCidrTypeMapping)GetMapping(typeof(GaussDBRange<Instant>))!;
            Assert.Equal("tsrange", mapping.StoreType);
            Assert.Equal("timestamp without time zone", mapping.SubtypeMapping.StoreType);

            var value = new GaussDBRange<Instant>(
                new LocalDateTime(2020, 1, 1, 12, 0, 0).InUtc().ToInstant(),
                new LocalDateTime(2020, 1, 2, 12, 0, 0).InUtc().ToInstant());
            Assert.Equal("""'["2020-01-01T12:00:00Z","2020-01-02T12:00:00Z"]'::tsrange""", mapping.GenerateSqlLiteral(value));
        }

        #region Support

        private static readonly GaussDBTypeMappingSource Mapper = new(
            new TypeMappingSourceDependencies(
                new ValueConverterSelector(new ValueConverterSelectorDependencies()),
                new JsonValueReaderWriterSource(new JsonValueReaderWriterSourceDependencies()),
                []),
            new RelationalTypeMappingSourceDependencies(
            [
                new GaussDBNodaTimeTypeMappingSourcePlugin(
                        new GaussDBSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()))
            ]),
            new GaussDBSqlGenerationHelper(new RelationalSqlGenerationHelperDependencies()),
            new GaussDBSingletonOptions()
        );

        private static RelationalTypeMapping? GetMapping(string storeType)
            => Mapper.FindMapping(storeType);

        private static RelationalTypeMapping? GetMapping(Type clrType)
            => Mapper.FindMapping(clrType);

        private static RelationalTypeMapping? GetMapping(Type clrType, string storeType)
            => Mapper.FindMapping(clrType, storeType);

        private class LegacyGaussDBNodaTimeTypeMappingFixture : IDisposable
        {
            public LegacyGaussDBNodaTimeTypeMappingFixture()
            {
                GaussDBNodaTimeTypeMappingSourcePlugin.LegacyTimestampBehavior = true;
            }

            public void Dispose()
                => GaussDBNodaTimeTypeMappingSourcePlugin.LegacyTimestampBehavior = false;
        }

        #endregion Support
    }

    [CollectionDefinition("LegacyNodaTimeTest", DisableParallelization = true)]
    public class EventSourceTestCollection;
}

#endif
