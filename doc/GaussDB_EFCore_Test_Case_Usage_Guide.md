# GaussDB EFCore Test Case Usage Guide

## 1. Overview

This document describes common test workflows for the current repository, including provider unit tests, functional tests, distributed functional full tests, single-test filtering, TRX parsing, and test database cleanup.

This document does not contain real database hosts, accounts, passwords, local machine paths, or other sensitive information. Values inside `<...>` are placeholders. Replace them temporarily on your machine and do not commit real values to the repository.

## 2. Prerequisites

1. Install the .NET 9 SDK.
2. Run commands from the repository root, written as `<repo-root>` in this document.
3. Prepare an available GaussDB/openGauss database if functional tests are needed.
4. Prepare `gsql.exe` or another SQL client if remote test databases need to be cleaned up.

Check the SDK:

```powershell
dotnet --info
```

Check the current target framework and dependency versions:

```powershell
Get-Content .\Directory.Build.props
Get-Content .\Directory.Packages.props
Get-Content .\global.json
```

## 3. Test Projects

| Project | Path | Purpose |
| --- | --- | --- |
| Provider unit tests | `test/EFCore.GaussDB.Tests/EFCore.GaussDB.Tests.csproj` | Verifies provider services, type mapping, SQL generation, downgrade controls, and related internals |
| Functional tests | `test/EFCore.GaussDB.FunctionalTests/EFCore.GaussDB.FunctionalTests.csproj` | Verifies EF Core relational behavior against a real database |

## 4. Provider Unit Tests

Run from `<repo-root>`:

```powershell
dotnet test .\test\EFCore.GaussDB.Tests\EFCore.GaussDB.Tests.csproj -f net9.0
```

These tests usually do not require a real database connection and are suitable as the first quick verification step.

## 5. Centralized Functional Tests

### 5.1 Configure the Connection String

Pass the test connection through environment variables. Example:

```powershell
$env:Test__GaussDB__DefaultConnection='Server=<centralized-host>;Username=<db-user>;Password=<db-password>;Port=<db-port>;SSL Mode=disable;Timeout=15;Include Error Detail=true'
$env:Test__GaussDB__EnableExtensionSessionParameter='false'
Remove-Item Env:\Test__GaussDB__IsDistributed -ErrorAction SilentlyContinue
```

Notes:

- Replace `<centralized-host>` with the centralized database host.
- Replace `<db-user>` and `<db-password>` with temporary test account information.
- Do not set `Test__GaussDB__IsDistributed=true` for centralized tests.
- Do not write real connection strings into repository files.

### 5.2 Build and Test

```powershell
dotnet build .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj -f net9.0

dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --logger "trx;LogFileName=functional-centralized-local.trx" `
  --results-directory .\test\EFCore.GaussDB.FunctionalTests\TestResults
```

## 6. Distributed Functional Full Tests

### 6.1 Configure the Connection String

Distributed tests must explicitly enable the distributed switch:

```powershell
$env:Test__GaussDB__DefaultConnection='Server=<distributed-host-1>,<distributed-host-2>,<distributed-host-3>;Username=<db-user>;Password=<db-password>;Port=<db-port>;SSL Mode=disable;Timeout=15;Include Error Detail=true'
$env:Test__GaussDB__EnableExtensionSessionParameter='false'
$env:Test__GaussDB__IsDistributed='true'
```

Notes:

- `Server` can contain multiple distributed nodes separated by commas.
- `Test__GaussDB__IsDistributed=true` enables distributed test adaptations, including FK DDL removal, replication distribution when needed, and deterministic ordering adjustments.
- This switch should only be used for distributed databases. Do not set it for centralized tests.

### 6.2 Build and Test

```powershell
dotnet build .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj -f net9.0

dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --logger "trx;LogFileName=functional-distributed-local.trx" `
  --results-directory .\test\EFCore.GaussDB.FunctionalTests\TestResults
```

Latest distributed full-test reference result for this branch:

```text
total=14553
executed=13826
passed=13826
failed=0
skipped=727
```

Use the latest TRX result from your local run as the source of truth.

## 7. Run a Single Test or a Test Category

Use `--filter` to reduce verification cost.

Filter by class name:

```powershell
dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --filter "FullyQualifiedName~NorthwindIncludeQueryGaussDBTest"
```

Filter by method name:

```powershell
dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --filter "FullyQualifiedName~Include_collection_skip_no_order_by"
```

Filter by the migrations test class:

```powershell
dotnet test .\test\EFCore.GaussDB.FunctionalTests\EFCore.GaussDB.FunctionalTests.csproj `
  -f net9.0 `
  --no-build `
  --filter "FullyQualifiedName~MigrationsGaussDBTest"
```

## 8. Parse TRX Results

After the test run completes, TRX files are located under:

```text
<repo-root>\test\EFCore.GaussDB.FunctionalTests\TestResults
```

Parse counters:

```powershell
$trx='.\test\EFCore.GaussDB.FunctionalTests\TestResults\functional-distributed-local.trx'
[xml]$x=Get-Content $trx
$ns=New-Object System.Xml.XmlNamespaceManager($x.NameTable)
$ns.AddNamespace('t','http://microsoft.com/schemas/VisualStudio/TeamTest/2010')
$x.SelectSingleNode('//t:ResultSummary/t:Counters',$ns).Attributes | ForEach-Object {
  "$($_.Name)=$($_.Value)"
}
```

List failed tests:

```powershell
$x.SelectNodes('//t:UnitTestResult[@outcome="Failed"]',$ns) | ForEach-Object {
  $messageNode=$_.SelectSingleNode('t:Output/t:ErrorInfo/t:Message',$ns)
  [pscustomobject]@{
    Test=$_.testName
    Message=if ($messageNode) { $messageNode.InnerText } else { '' }
  }
} | Format-List
```

## 9. Test Database Cleanup

Functional tests create many temporary databases. After a full run, an interrupted run, or a network interruption, clean up test databases to avoid remote database storage growth.

The following command keeps only the allow-listed databases. Replace `<gsql-path>`, `<admin-host>`, `<db-user>`, `<db-password>`, and `<db-port>` with temporary local values.

```powershell
$gsql='<gsql-path>'
$adminHost='<admin-host>'
$dbUser='<db-user>'
$dbPassword='<db-password>'
$dbPort='<db-port>'

$keep = @('postgres','template0','template1','mytest','templatea','templatem')
$keepSql = ($keep | ForEach-Object { "'$_'" }) -join ','

$dbs = & $gsql -d postgres -h $adminHost -U $dbUser -p $dbPort -W $dbPassword -At -c "SELECT datname FROM pg_database WHERE datname NOT IN ($keepSql) ORDER BY datname;"
$dbs = @($dbs | Where-Object { $_ -and $_.Trim().Length -gt 0 })

foreach ($db in $dbs) {
  $escaped = $db.Trim().Replace('"','""')
  "DROP DATABASE IF EXISTS `"$escaped`";" | & $gsql -d postgres -h $adminHost -U $dbUser -p $dbPort -W $dbPassword
}

& $gsql -d postgres -h $adminHost -U $dbUser -p $dbPort -W $dbPassword -At -c "SELECT count(*) FROM pg_database WHERE datname NOT IN ($keepSql);"
```

The expected final output is `0`.

If some databases cannot be dropped because connections are still open, stop the test process first and run cleanup again. Use database-provided session termination commands only when necessary, and do not write real session information into documents or commits.

## 10. FAQ

### 10.1 `FOREIGN KEY ... REFERENCES constraint is not yet supported`

This error means the distributed database does not support FK DDL. Distributed tests must set:

```powershell
$env:Test__GaussDB__IsDistributed='true'
```

This switch makes the test table creation path remove FK DDL. Centralized tests should not enable this switch.

### 10.2 `Column ... is not a hash distributable data type`

This is a distributed-table distribution-key limitation. The test layer uses `DISTRIBUTE BY REPLICATION` when needed so test tables can avoid hash distribution key restrictions.

If this error still appears, check:

- Whether the latest test code is being used.
- Whether `Test__GaussDB__IsDistributed=true` is set.
- Whether the failing SQL comes from handwritten SQL outside the test table creation path.

### 10.3 Assertion Order Differences

Distributed databases do not have a stable physical return order. When assertion differences involve `Skip`, `Take`, `FirstOrDefault`, or `Include`, first check whether the query has an explicit `OrderBy`.

If the test semantics require a fixed result set, add deterministic ordering in the distributed branch instead of relying on database physical order.

### 10.4 Database Storage Grows After Tests

This usually means temporary test databases remain. Run the cleanup script in section 9 and confirm that the remaining count is `0`.

## 11. Sensitive Information Rules

Do not commit:

- Real IP addresses, domains, accounts, or passwords.
- Complete connection strings.
- Personal local paths or usernames.
- Generated TRX files, logs, or temporary scripts.

Prefer setting environment variables only in the current PowerShell session. After tests, clean them up:

```powershell
Remove-Item Env:\Test__GaussDB__DefaultConnection -ErrorAction SilentlyContinue
Remove-Item Env:\Test__GaussDB__EnableExtensionSessionParameter -ErrorAction SilentlyContinue
Remove-Item Env:\Test__GaussDB__IsDistributed -ErrorAction SilentlyContinue
```
