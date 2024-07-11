using ExpressionTreeToString.Util;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZSpitz.Util;
using static System.Linq.Expressions.ExpressionType;

namespace ExpressionTreeToString;
/// <summary>
/// 
/// </summary>
/// <seealso cref="DynamicLinqWriterVisitor" />
public class DynamicCypherWriterVisitor(Dictionary<string, string> cypherPropertyMap, object o, OneOf<string, Language?> languageArg, bool hasPathSpans) :
    DynamicLinqWriterVisitor(o, languageArg, hasPathSpans) {
    private static readonly Dictionary<ExpressionType, string> simpleBinaryOperators = new() {
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

    protected override Dictionary<ExpressionType, string> SimpleBinaryOperators => simpleBinaryOperators;

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

    protected override void WriteNew(NewExpression expr) {
        if (expr.Type.IsAnonymous()) {
            //Write("new(");
            foreach (var (name, arg, index) in expr.NamesArguments()) {
                if (index > 0) { Write(", "); }

                // if the expression being assigned from is a property access with the same name as the target property, 
                // write only the target expression.
                // Otherwise, write `property = expression`
                if (!(arg is MemberExpression mexpr && mexpr.Member.Name.Replace("$VB$Local_", "") == name)) {
                    Write($"{name} = ");
                }
                WriteNode($"Arguments[{index}]", arg);
            }
            //Write(")");
            return;
        }

        Write(typeName(expr.Type));
        Write("(");
        WriteNodes("Arguments", expr.Arguments);
        Write(")");
    }
    protected override void WriteMemberAccess(MemberExpression expr) {
        if (expr.Expression is { } && expr.Expression.Type.IsClosureClass() && expr.Expression is ConstantExpression cexpr) {
            //writeDynamicLinqParameter((cexpr.Value, expr.Member), () => expr.Member.Name);
            var obj = expr.ExtractValue();//TODO: Parameterize
            if (obj is string)
                Write($"'{obj}'");
            else
                Write(obj.ToString());
            return;
        }

        writeMemberUse("Expression", expr.Expression, expr.Member);
    }
}
