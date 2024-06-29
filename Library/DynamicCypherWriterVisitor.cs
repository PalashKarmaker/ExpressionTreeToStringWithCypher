using OneOf;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZSpitz.Util;
using static System.Linq.Expressions.ExpressionType;

namespace ExpressionTreeToString;

public class DynamicCypherWriterVisitor(Dictionary<string, string> cypherPropertyMap, object o, OneOf<string, Language?> languageArg, bool hasPathSpans) : 
    DynamicLinqWriterVisitor(o, languageArg, hasPathSpans) {
    private static readonly Dictionary<ExpressionType, string> _simpleBinaryOperators = new() {
        [Add] = "+",
        [AddChecked] = "+",
        [Divide] = "/",
        [Modulo] = "%",
        [Multiply] = "*",
        [MultiplyChecked] = "*",
        [Subtract] = "-",
        [SubtractChecked] = "-",
        [AndAlso] = "AND",
        [OrElse] = "OR",
        [Equal] = "=",
        [NotEqual] = "!=",
        [GreaterThanOrEqual] = ">=",
        [GreaterThan] = ">",
        [LessThan] = "<",
        [LessThanOrEqual] = "<=",
        [Coalesce] = "??",
    };

    protected override Dictionary<ExpressionType, string> SimpleBinaryOperators => _simpleBinaryOperators;

    protected override void WriteProperty(string? s) => base.Write(cypherPropertyMap[s]);
    protected override void WriteLambda(LambdaExpression expr) {
        var exitLambda = false;
        if (!insideLambda) {
            //Write("\"");
            insideLambda = true;
            exitLambda = true;
        }

        var count = expr.Parameters.Count;
        if (count > 1) {
            WriteNotImplemented("Multiple parameters in lambda expression.");
            return;
        } else if (count == 1) {
            currentScoped = expr.Parameters[0];
        }
        WriteNode("Body", expr.Body);

        if (exitLambda) {
            //Write("\"");
            insideLambda = false;
        }
    }
}
