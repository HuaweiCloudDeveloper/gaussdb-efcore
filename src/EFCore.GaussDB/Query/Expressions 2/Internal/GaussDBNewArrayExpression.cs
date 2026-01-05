namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Query.Expressions.Internal;

/// <summary>
///     Represents creating a new GaussDB array.
/// </summary>
public class GaussDBNewArrayExpression : SqlExpression
{
    private static ConstructorInfo? _quotingConstructor;

    /// <summary>
    ///     Creates a new instance of the <see cref="GaussDBNewArrayExpression" /> class.
    /// </summary>
    /// <param name="expressions">The values to initialize the elements of the new array.</param>
    /// <param name="type">The <see cref="Type" /> of the expression.</param>
    /// <param name="typeMapping">The <see cref="RelationalTypeMapping" /> associated with the expression.</param>
    public GaussDBNewArrayExpression(
        IReadOnlyList<SqlExpression> expressions,
        Type type,
        RelationalTypeMapping? typeMapping)
        : base(type, typeMapping)
    {
        Check.NotNull(expressions, nameof(expressions));

        if (type.TryGetElementType(typeof(IEnumerable<>)) is null)
        {
            throw new ArgumentException($"{nameof(GaussDBNewArrayExpression)} must have an IEnumerable<T> type");
        }

        Expressions = expressions;
    }

    /// <summary>
    ///     The operator of this GaussDB binary operation.
    /// </summary>
    public virtual IReadOnlyList<SqlExpression> Expressions { get; }

    /// <inheritdoc />
    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        Check.NotNull(visitor, nameof(visitor));

        List<SqlExpression>? newExpressions = null;
        for (var i = 0; i < Expressions.Count; i++)
        {
            var expression = Expressions[i];
            var visitedExpression = (SqlExpression)visitor.Visit(expression);
            if (visitedExpression != expression && newExpressions is null)
            {
                newExpressions = [];
                for (var j = 0; j < i; j++)
                {
                    newExpressions.Add(Expressions[j]);
                }
            }

            newExpressions?.Add(visitedExpression);
        }

        return newExpressions is null
            ? this
            : new GaussDBNewArrayExpression(newExpressions, Type, TypeMapping);
    }

    /// <summary>
    ///     Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will
    ///     return this expression.
    /// </summary>
    /// <param name="expressions">The values to initialize the elements of the new array.</param>
    /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
    public virtual GaussDBNewArrayExpression Update(IReadOnlyList<SqlExpression> expressions)
    {
        Check.NotNull(expressions, nameof(expressions));

        return expressions == Expressions
            ? this
            : new GaussDBNewArrayExpression(expressions, Type, TypeMapping);
    }

    /// <inheritdoc />
    public override Expression Quote()
        => New(
            _quotingConstructor ??= typeof(GaussDBNewArrayExpression).GetConstructor(
                [typeof(IReadOnlyList<SqlExpression>), typeof(Type), typeof(RelationalTypeMapping)])!,
            NewArrayInit(typeof(SqlExpression), initializers: Expressions.Select(a => a.Quote())),
            Constant(Type),
            RelationalExpressionQuotingUtilities.QuoteTypeMapping(TypeMapping));

    /// <inheritdoc />
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        Check.NotNull(expressionPrinter, nameof(expressionPrinter));

        expressionPrinter.Append("ARRAY[");

        var first = true;
        foreach (var expression in Expressions)
        {
            if (!first)
            {
                expressionPrinter.Append(", ");
            }

            first = false;

            expressionPrinter.Visit(expression);
        }

        expressionPrinter.Append("]");
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is not null
            && (ReferenceEquals(this, obj)
                || obj is GaussDBNewArrayExpression sqlBinaryExpression
                && Equals(sqlBinaryExpression));

    private bool Equals(GaussDBNewArrayExpression pgNewArrayExpression)
        => base.Equals(pgNewArrayExpression)
            && Expressions.SequenceEqual(pgNewArrayExpression.Expressions);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();

        hash.Add(base.GetHashCode());
        for (var i = 0; i < Expressions.Count; i++)
        {
            hash.Add(Expressions[i]);
        }

        return hash.ToHashCode();
    }
}
