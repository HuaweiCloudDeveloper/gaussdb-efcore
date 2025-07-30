using HuaweiCloud.EntityFrameworkCore.GaussDB.Storage.Internal.Mapping;

namespace HuaweiCloud.EntityFrameworkCore.GaussDB.Query.Expressions.Internal;

/// <summary>
///     An expression that represents a GaussDB-specific binary operation in a SQL tree.
/// </summary>
public class GaussDBBinaryExpression : SqlExpression
{
    private static ConstructorInfo? _quotingConstructor;

    /// <summary>
    ///     Creates a new instance of the <see cref="GaussDBBinaryExpression" /> class.
    /// </summary>
    /// <param name="operatorType">The operator to apply.</param>
    /// <param name="left">An expression which is left operand.</param>
    /// <param name="right">An expression which is right operand.</param>
    /// <param name="type">The <see cref="Type" /> of the expression.</param>
    /// <param name="typeMapping">The <see cref="RelationalTypeMapping" /> associated with the expression.</param>
    public GaussDBBinaryExpression(
        GaussDBExpressionType operatorType,
        SqlExpression left,
        SqlExpression right,
        Type type,
        RelationalTypeMapping? typeMapping)
        : base(type, typeMapping)
    {
        Check.NotNull(left, nameof(left));
        Check.NotNull(right, nameof(right));

        OperatorType = operatorType;
        Left = left;
        Right = right;
    }

    /// <summary>
    ///     The operator of this GaussDB binary operation.
    /// </summary>
    public virtual GaussDBExpressionType OperatorType { get; }

    /// <summary>
    ///     The left operand.
    /// </summary>
    public virtual SqlExpression Left { get; }

    /// <summary>
    ///     The right operand.
    /// </summary>
    public virtual SqlExpression Right { get; }

    /// <inheritdoc />
    protected override Expression VisitChildren(ExpressionVisitor visitor)
    {
        Check.NotNull(visitor, nameof(visitor));

        var left = (SqlExpression)visitor.Visit(Left);
        var right = (SqlExpression)visitor.Visit(Right);

        return Update(left, right);
    }

    /// <summary>
    ///     Creates a new expression that is like this one, but using the supplied children. If all of the children are the same, it will
    ///     return this expression.
    /// </summary>
    /// <param name="left">The <see cref="Left" /> property of the result.</param>
    /// <param name="right">The <see cref="Right" /> property of the result.</param>
    /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
    public virtual GaussDBBinaryExpression Update(SqlExpression left, SqlExpression right)
    {
        Check.NotNull(left, nameof(left));
        Check.NotNull(right, nameof(right));

        return left != Left || right != Right
            ? new GaussDBBinaryExpression(OperatorType, left, right, Type, TypeMapping)
            : this;
    }

    /// <inheritdoc />
    public override Expression Quote()
        => New(
            _quotingConstructor ??= typeof(GaussDBBinaryExpression).GetConstructor(
                [typeof(GaussDBExpressionType), typeof(SqlExpression), typeof(SqlExpression), typeof(Type), typeof(RelationalTypeMapping)])!,
            Constant(OperatorType),
            Left.Quote(),
            Right.Quote(),
            Constant(Type),
            RelationalExpressionQuotingUtilities.QuoteTypeMapping(TypeMapping));

    /// <inheritdoc />
    protected override void Print(ExpressionPrinter expressionPrinter)
    {
        Check.NotNull(expressionPrinter, nameof(expressionPrinter));

        var requiresBrackets = RequiresBrackets(Left);

        if (requiresBrackets)
        {
            expressionPrinter.Append("(");
        }

        expressionPrinter.Visit(Left);

        if (requiresBrackets)
        {
            expressionPrinter.Append(")");
        }

        expressionPrinter
            .Append(" ")
            .Append(
                OperatorType switch
                {
                    GaussDBExpressionType.Contains => "@>",
                    GaussDBExpressionType.ContainedBy => "<@",
                    GaussDBExpressionType.Overlaps => "&&",

                    GaussDBExpressionType.NetworkContainedByOrEqual => "<<=",
                    GaussDBExpressionType.NetworkContainsOrEqual => ">>=",
                    GaussDBExpressionType.NetworkContainsOrContainedBy => "&&",

                    GaussDBExpressionType.RangeIsStrictlyLeftOf => "<<",
                    GaussDBExpressionType.RangeIsStrictlyRightOf => ">>",
                    GaussDBExpressionType.RangeDoesNotExtendRightOf => "&<",
                    GaussDBExpressionType.RangeDoesNotExtendLeftOf => "&>",
                    GaussDBExpressionType.RangeIsAdjacentTo => "-|-",
                    GaussDBExpressionType.RangeUnion => "+",
                    GaussDBExpressionType.RangeIntersect => "*",
                    GaussDBExpressionType.RangeExcept => "-",

                    GaussDBExpressionType.TextSearchMatch => "@@",
                    GaussDBExpressionType.TextSearchAnd => "&&",
                    GaussDBExpressionType.TextSearchOr => "||",

                    GaussDBExpressionType.JsonExists => "?",
                    GaussDBExpressionType.JsonExistsAny => "?|",
                    GaussDBExpressionType.JsonExistsAll => "?&",

                    GaussDBExpressionType.LTreeMatches
                        when Right.TypeMapping is { StoreType: "lquery" } or GaussDBArrayTypeMapping
                        {
                            ElementTypeMapping.StoreType: "lquery"
                        }
                        => "~",
                    GaussDBExpressionType.LTreeMatches when Right.TypeMapping?.StoreType == "ltxtquery" => "@",
                    GaussDBExpressionType.LTreeMatchesAny => "?",
                    GaussDBExpressionType.LTreeFirstAncestor => "?@>",
                    GaussDBExpressionType.LTreeFirstDescendent => "?<@",
                    GaussDBExpressionType.LTreeFirstMatches when Right.TypeMapping?.StoreType == "lquery" => "?~",
                    GaussDBExpressionType.LTreeFirstMatches when Right.TypeMapping?.StoreType == "ltxtquery" => "?@",

                    GaussDBExpressionType.Distance => "<->",

                    _ => throw new ArgumentOutOfRangeException($"Unhandled operator type: {OperatorType}")
                })
            .Append(" ");

        requiresBrackets = RequiresBrackets(Right);

        if (requiresBrackets)
        {
            expressionPrinter.Append("(");
        }

        expressionPrinter.Visit(Right);

        if (requiresBrackets)
        {
            expressionPrinter.Append(")");
        }

        static bool RequiresBrackets(SqlExpression expression)
            => expression is GaussDBBinaryExpression or LikeExpression;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is not null
            && (ReferenceEquals(this, obj)
                || obj is GaussDBBinaryExpression sqlBinaryExpression
                && Equals(sqlBinaryExpression));

    private bool Equals(GaussDBBinaryExpression sqlBinaryExpression)
        => base.Equals(sqlBinaryExpression)
            && OperatorType == sqlBinaryExpression.OperatorType
            && Left.Equals(sqlBinaryExpression.Left)
            && Right.Equals(sqlBinaryExpression.Right);

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), OperatorType, Left, Right);
}
