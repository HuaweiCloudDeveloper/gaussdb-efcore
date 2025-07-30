namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Migrations.Operations;

/// <summary>
///     A GaussDB-specific <see cref="MigrationOperation" /> to create a database.
/// </summary>
/// <remarks>
///     See <see href="https://aka.ms/efcore-docs-migrations">Database migrations</see>.
/// </remarks>
[DebuggerDisplay("CREATE DATABASE {Name}")]
public class GaussDBCreateDatabaseOperation : DatabaseOperation
{
    /// <summary>
    ///     The name of the database.
    /// </summary>
    public virtual string Name { get; set; } = null!;

    /// <summary>
    ///     The GaussDB database to use as a template for the new database to be created.
    /// </summary>
    public virtual string? Template { get; set; }

    /// <summary>
    ///     The GaussDB tablespace in which to create the database.
    /// </summary>
    public virtual string? Tablespace { get; set; }
}
