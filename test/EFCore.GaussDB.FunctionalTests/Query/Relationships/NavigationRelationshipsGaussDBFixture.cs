// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.EntityFrameworkCore.Query.Relationships;

public class NavigationRelationshipsGaussDBFixture : NavigationRelationshipsRelationalFixtureBase, ITestSqlLoggerFactory
{
    protected override ITestStoreFactory TestStoreFactory
        => GaussDBTestStoreFactory.Instance;

    public TestSqlLoggerFactory TestSqlLoggerFactory
        => (TestSqlLoggerFactory)ListLoggerFactory;
}
