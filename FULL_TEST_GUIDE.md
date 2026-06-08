# Standard Full Test Guide

This guide standardizes two test entry points:

- the repository-wide test run
- the designated remote GaussDB database-backed run

## Standard connection fields

Use the normal connection string fields below:

- `Host`
- `Port`
- `Database`
- `Username`
- `Password`

## Repository-wide full test run

If you want to run the whole repository against the designated GaussDB environment, set the connection variable first and then run the solution:

```powershell
$env:Test__GaussDB__DefaultConnection="Host=<host>;Port=<port>;Database=<database>;Username=<username>;Password=<password>"
dotnet test EFCore.GaussDB.slnx
```

This is the closest command to "full repository validation". The variable is consumed by the database-backed tests; the unit tests ignore it.

## Designated GaussDB test run

`EFCore.GaussDB.FunctionalTests` is the full database-backed test suite for the provider. Use it when you want to validate behavior against a designated remote GaussDB instance.

### Environment variable

The full test suite reads the connection string from:

- `Test__GaussDB__DefaultConnection`

Example:

```powershell
$env:Test__GaussDB__DefaultConnection="Host=<host>;Port=<port>;Database=<database>;Username=<username>;Password=<password>"
dotnet test test/EFCore.GaussDB.FunctionalTests/EFCore.GaussDB.FunctionalTests.csproj -c Release
```

Use the same parameter names for every designated test environment; only the values should change.

### Full test requirements

- The account must be able to connect to `postgres`.
- The account must be able to create and drop databases.
- The suite creates isolated databases for individual scenarios, so read-only access is not enough.
- Keep `Test__GaussDB__EnableExtensionConnectionOption` enabled when testing GaussDB.
- Only disable `Test__GaussDB__EnableExtensionConnectionOption` when intentionally validating openGauss-compatible behavior.

### Verified full test result

The latest full run against the designated GaussDB test environment passed.

- Result: `0` failed, `24254` passed, `1916` skipped, `26170` total
- Duration: `30m19s`
- Verification date: `2026-05-07`
