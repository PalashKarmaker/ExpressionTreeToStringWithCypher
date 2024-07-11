using OneOf;
using System.Collections.Generic;
using System.Linq.Expressions;
using ZSpitz.Util;

namespace ExpressionTreeToString;

public class DynamicParameterizedCypherWriterVisitor(Dictionary<string, string> cypherPropertyMap, object o, OneOf<string, Language?> languageArg, Dictionary<string, object?> parameters) :
    DynamicCypherWriterVisitor(cypherPropertyMap, o, languageArg, false) {
    public (string result, Dictionary<string, object?> parameters) GetParameterizedResult() => (GetResult().result, parameters);
    protected override void WriteMemberAccess(MemberExpression expr) {
        if (expr.Expression is { } && expr.Expression.Type.IsClosureClass() && expr.Expression is ConstantExpression cexpr) {
            var obj = expr.ExtractValue();//TODO: Parameterize
            var pName = $"p{parameters.Count}";
            parameters.Add(pName, obj);
            Write($"${pName}");
            return;
        }

        writeMemberUse("Expression", expr.Expression, expr.Member);
    }
}
