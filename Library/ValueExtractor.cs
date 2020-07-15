﻿using System.Collections.Generic;
using System.Linq.Expressions;
using ZSpitz.Util;

namespace ExpressionTreeToString {
    public class ValueExtractor : ExpressionVisitor {
        private Stack<Expression> expressionStack = new Stack<Expression>();
        private Dictionary<Expression, bool> evaluables = new Dictionary<Expression, bool>();

        public override Expression Visit(Expression node) {
            expressionStack.Push(node);

            switch (node) {
                // AFAICT the only way to get a void-returning expression within an expression tree is as part of a BlockExpression
                // other expression types cannot contain a void-returning expression
                case Expression expr when expr.Type == typeof(void):
                    foreach (var x in expressionStack) {
                        // BlockExpressions may have a value even though one of the statements is a void-returning expression
                        if (x is BlockExpression && expr != x) { break; }
                        evaluables[x] = false;
                    }
                    break;

                // Nodes which contain the following expression types cannot be evaluated
                // ParameterExpression because it has no value
                // LoopExpression because it might be a never-ending loop
                case ParameterExpression _:
                case LoopExpression _:
                    foreach (var x in expressionStack) {
                        evaluables[x] = false;
                    }
                    break;


                case DefaultExpression _:
                case ConstantExpression _:
                case MethodCallExpression mcexpr when mcexpr.Arguments.None() && mcexpr.Object is null:
                case MemberExpression mexpr when mexpr.Expression is null:
                case NewExpression nexpr when nexpr.Arguments.None() && nexpr.Members.None():
                case DebugInfoExpression _:
                case GotoExpression _:
                    foreach (var x in expressionStack) {
                        if (evaluables.ContainsKey(x)) { break; }

                        // LambdaExpression's value is the same as LambdaExpression.Body
                        // BlockExpression's value is the same as the last expression in the block
                        evaluables[x] =
                            x is LambdaExpression || x is BlockExpression ?
                                false :
                                true;
                    }
                    break;
            }

            var ret = base.Visit(node);
            expressionStack.Pop();
            return ret;
        }

        //protected override Expression VisitParameter(ParameterExpression node) {
        //    markUnevaluable();
        //    return base.VisitParameter(node);
        //}

        //protected override Expression VisitDefault(DefaultExpression node) {
        //    markEvaluable();
        //    return base.VisitDefault(node);
        //}
        //protected override Expression VisitConstant(ConstantExpression node) {
        //    markEvaluable();
        //    return base.VisitConstant(node);
        //}
        //protected override Expression VisitMethodCall(MethodCallExpression node) {
        //    if (node.Arguments.None() && node.Object is null ) { 
        //        if (node.Type == typeof(void)) {
        //            markUnevaluable();
        //        } else {
        //            markEvaluable();
        //        }
        //    }
        //    return base.VisitMethodCall(node);
        //}
        //protected override Expression VisitMember(MemberExpression node) {
        //    if (node.Expression is null) { markEvaluable(); }
        //    return base.VisitMember(node);
        //}
        //protected override Expression VisitNew(NewExpression node) {
        //    if (node.Arguments.None() && node.Members.None()) { markEvaluable(); }
        //    return base.VisitNew(node);
        //}
        //protected override Expression VisitDebugInfo(DebugInfoExpression node) {
        //    markEvaluable();
        //    return base.VisitDebugInfo(node);
        //}

        //private void markEvaluable() =>
        //    expressionStack.ForEach(x => {
        //        if (evaluables.ContainsKey(x)) { return; }
        //        evaluables[x] = true;
        //    });
        //private void markUnevaluable() => expressionStack.ForEach(x => evaluables[x] = false);

        public (bool evaluated, object? value) GetValue(Expression node) {
            if (!evaluables.TryGetValue(node, out var canEvaluate)) {
                Visit(node);
                canEvaluate = evaluables[node];
            }
            (bool evaluated, object? value) = (false, null);
            if (canEvaluate) {
                evaluated = node.TryExtractValue(out value);
            }
            return (evaluated, value);
        }
    }
}