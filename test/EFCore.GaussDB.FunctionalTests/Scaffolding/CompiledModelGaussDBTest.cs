// using System.Runtime.CompilerServices;
// using HuaweiCloud.EntityFrameworkCore.GaussDB.Design.Internal;
// using HuaweiCloud.EntityFrameworkCore.GaussDB.Infrastructure;
// using HuaweiCloud.EntityFrameworkCore.GaussDB.TestUtilities;
//
// namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Scaffolding;
//
// public class CompiledModelGaussDBTest : CompiledModelRelationalTestBase
// {
//     protected override TestHelpers TestHelpers => GaussDBTestHelpers.Instance;
//     protected override ITestStoreFactory TestStoreFactory => GaussDBTestStoreFactory.Instance;
//
//     // #3087
//     public override void BigModel()
//         => Assert.Throws<InvalidOperationException>(() => base.BigModel());
//
//     // #3087
//     public override void BigModel_with_JSON_columns()
//         => Assert.Throws<InvalidOperationException>(() => base.BigModel());
//
//     // #3087
//     public override void CheckConstraints()
//         => Assert.Throws<InvalidOperationException>(() => base.BigModel());
//
//     // #3087
//     public override void DbFunctions()
//         => Assert.Throws<InvalidOperationException>(() => base.BigModel());
//
//     // #3087
//     public override void Triggers()
//         => Assert.Throws<InvalidOperationException>(() => base.BigModel());
//
//     // https://github.com/dotnet/efcore/pull/32341/files#r1485603038
//     public override void Tpc()
//         => Assert.Throws<InvalidOperationException>(() => base.Tpc());
//
//     // https://github.com/dotnet/efcore/pull/32341/files#r1485603038
//     public override void ComplexTypes()
//         => Assert.Throws<InvalidOperationException>(() => base.ComplexTypes());
//
//     protected override DbContextOptionsBuilder AddOptions(DbContextOptionsBuilder builder)
//     {
//         new GaussDBDbContextOptionsBuilder(builder).UseNetTopologySuite();
//         return builder;
//     }
//
//     protected override void AddDesignTimeServices(IServiceCollection services)
//         => new GaussDBNetTopologySuiteDesignTimeServices().ConfigureDesignTimeServices(services);
//
//     protected override BuildSource AddReferences(BuildSource build, [CallerFilePath] string filePath = "")
//     {
//         base.AddReferences(build);
//         build.References.Add(BuildReference.ByName("HuaweiCloud.EntityFrameworkCore.GaussDB"));
//         build.References.Add(BuildReference.ByName("HuaweiCloud.EntityFrameworkCore.GaussDB.NetTopologySuite"));
//         build.References.Add(BuildReference.ByName("GaussDB"));
//         build.References.Add(BuildReference.ByName("NetTopologySuite"));
//         return build;
//     }
// }
