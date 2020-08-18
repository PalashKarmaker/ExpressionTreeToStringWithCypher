﻿using ZSpitz.Util;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using static ExpressionTreeToString.Globals;
using static ZSpitz.Util.Functions;
using static System.Linq.Expressions.Expression;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Linq.Enumerable;
using OneOf;
using static ZSpitz.Util.Language;

namespace ExpressionTreeToString {
    public class FactoryMethodsWriterVisitor : BuiltinsWriterVisitor {
        public const string CSharpUsing = "// using static System.Linq.Expressions.Expression";
        public const string VisualBasicUsing = "' Imports System.Linq.Expressions.Expression";

        protected override void PreWrite() {
            string @using;
            switch (language) {
                case CSharp:
                    @using = CSharpUsing;
                    break;
                case VisualBasic:
                    @using = VisualBasicUsing;
                    break;
                default:
                    return;
            }
            Write(@using);
            WriteEOL();
            WriteEOL();
        }

        public FactoryMethodsWriterVisitor(object o, OneOf<string, Language?> languageArg) : 
            base(o, languageArg.ResolveLanguage() ?? throw new ArgumentException("Invalid language")) { }
        public FactoryMethodsWriterVisitor(object o, OneOf<string, Language?> languageArg, out Dictionary<string, (int start, int length)> pathSpans) : 
            base(o, languageArg.ResolveLanguage() ?? throw new ArgumentException("Invalid language"), out pathSpans) { }

        /// <param name="args">The arguments to write. If a tuple of string and node type, will write as single node. If a tuple of string and property type, will write as multiple nodes.</param>
        private void WriteMethodCall(string name, IEnumerable args) {
            Write(name);
            Write("(");

            bool wrapPreviousInNewline = false;
            bool indented = false;

            args.Cast<object>().ForEach((x, index) => {
                var isTuple = TryTupleValues(x, out var values) && values.Length == 2;
                (string path, object? arg) = isTuple ? ((string)values![0]!, values[1]) : ("", x);
                var parameterDeclaration = name == "Lambda" && path.StartsWith("Parameters");

                bool writeNewline = false;
                var argType = arg?.GetType();
                if (wrapPreviousInNewline) {
                    writeNewline = true;
                    wrapPreviousInNewline = false;
                } else if (arg is { }) {
                    if (
                        (
                            argType!.InheritsFromOrImplementsAny(NodeTypes) ||
                            arg is MemberInfo || 
                            arg is CallSiteBinder
                        ) && !(
                            arg is ParameterExpression && !parameterDeclaration
                        ) && !(
                            arg is MemberExpression mexpr && mexpr.IsClosedVariable()
                        )
                    ) {
                        writeNewline = true;
                        wrapPreviousInNewline = true;
                    }
                }

                if (index > 0) {
                    Write(",");
                }
                if (writeNewline) {
                    if (!indented) {
                        Indent();
                        indented = true;
                    }
                    WriteEOL();
                } else if (index > 0) {
                    Write(" ");
                }

                if (argType?.InheritsFromOrImplementsAny(NodeTypes) ?? false) {
                    WriteNode(path, arg!, parameterDeclaration);
                } else if (argType?.InheritsFromOrImplementsAny(PropertyTypes) ?? false) {
                    if (language == CSharp) {
                        Write("new[] {");
                    } else { // language == VisualBasic
                        Write("{");
                    }

                    var argList = arg is IEnumerable enumerable ? enumerable.ToObjectList() : Empty<object>().ToList();
                    if (argList.Any()) {
                        if (parameterDeclaration || argList.Any(y => !(y is ParameterExpression))) {
                            Indent();
                            WriteEOL();
                            WriteNodes(path, argList, true, ", ", parameterDeclaration);
                            WriteEOL(true);
                        } else {
                            Write(" ");
                            WriteNodes(path, argList, false, ", ", parameterDeclaration);
                            Write(" ");
                        }
                    }

                    Write("}");
                } else {
                    Write(RenderLiteral(arg, language));
                }
            });

            if (indented) { Dedent(); }
            if (wrapPreviousInNewline || indented) {
                WriteEOL();
            }

            Write(")");
        }

        private void WriteMethodCall(Expression<Action> expr) {
            if (!(expr.Body is MethodCallExpression callExpr)) { throw new ArgumentException("Not a MethodCallExpression"); }

            var args = NewArrayInit(typeof(object), callExpr.Arguments.Select(x => {
                var xType = x.Type;
                if (!xType.IsValueType) { return x; }
                return Convert(x, typeof(object));
            })).ExtractValue() as object[];
            var names = callExpr.Arguments.Select(x => {
                if (x is MethodCallExpression callExpr1 && callExpr1.Method.Name == "ToArray") { x = callExpr1.Arguments[0]; }
                if (x is UnaryExpression unary && unary.NodeType == ExpressionType.Convert) { x = unary.Operand; }
                return (x as MemberExpression)?.Member.Name ?? "";
            });
            var pairs = names.Zip(args, (name, arg) => (name, arg)).ToList();
            var lastPair = pairs.LastOrDefault();
            if ((lastPair.arg?.GetType().IsArray ?? false) && callExpr.Method.GetParameters().Last().HasAttribute<ParamArrayAttribute>()) {
                pairs.RemoveLast();
                (lastPair.arg as IEnumerable).Cast<object>().Select((innerArg, index) => ($"{lastPair.name}[{index}]", innerArg)).AddRangeTo(pairs);
            }
            WriteMethodCall(callExpr.Method.Name, pairs.ToList());
        }

        protected override void WriteBinary(BinaryExpression expr) {
            if (!BinaryUnaryMethods.TryGetValue(expr.NodeType, out var name)) { throw new InvalidOperationException($"Method not found for '{expr.NodeType}' node type"); }
            WriteMethodCall(name, new[] { ("Left", expr.Left), ("Right", expr.Right) });
        }

        protected override void WriteUnary(UnaryExpression expr) {
            if (!BinaryUnaryMethods.TryGetValue(expr.NodeType, out var name)) { throw new InvalidOperationException($"Method not found for '{expr.NodeType}' node type"); }
            switch (expr.NodeType) {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.TypeAs:
                    WriteMethodCall(name, new object[] { ("Operand", expr.Operand), expr.Type });
                    break;
                default:
                    WriteMethodCall(name, new[] { ("Operand", expr.Operand) });
                    break;
            }
        }

        protected override void WriteLambda(LambdaExpression expr) {
            if (!expr.Name.IsNullOrWhitespace() && expr.TailCall) {
                WriteMethodCall(() => Lambda(expr.Body, expr.Name, expr.TailCall, expr.Parameters.ToArray()));
            } else if (expr.TailCall) {
                WriteMethodCall(() => Lambda(expr.Body, expr.TailCall, expr.Parameters.ToArray()));
            } else if (!expr.Name.IsNullOrWhitespace()) {
                WriteMethodCall(() => Lambda(expr.Body, expr.Name, expr.Parameters));
            } else {
                WriteMethodCall(() => Lambda(expr.Body, expr.Parameters.ToArray()));
            }
        }

        protected override void WriteParameter(ParameterExpression expr) => Write(expr.Name);

        protected override void WriteConstant(ConstantExpression expr) {
            if (
                (expr.Value == null && expr.Type != typeof(object)) ||
                (expr.Value != null && expr.Value.GetType() != expr.Type)
            ) {
                WriteMethodCall(() => Constant(expr.Value, expr.Type));
                return;
            }
            WriteMethodCall(() => Constant(expr.Value));
        }

        protected override void WriteMemberAccess(MemberExpression expr) {
            // closed over variable from oute scope
            switch (expr.Expression) {
                case ConstantExpression cexpr when cexpr.Type.IsClosureClass():
                case MemberExpression mexpr when mexpr.Type.IsClosureClass():
                    Write(expr.Member.Name.Replace("$VB$Local_", ""));
                    return;
            }

            WriteMethodCall(() => MakeMemberAccess(expr.Expression, expr.Member));
        }

        protected override void WriteNew(NewExpression expr) => 
            WriteMethodCall(() => New(expr.Constructor, expr.Arguments.ToArray()));

        protected override void WriteCall(MethodCallExpression expr) {
            if ((expr.Object?.Type.IsArray ?? false) && expr.Method.Name == "Get") {
                WriteMethodCall(() => ArrayIndex(expr.Object, expr.Arguments.ToArray()));
                return;
            } else if (expr.Method.IsIndexerMethod(out var pi)) {
                WriteMethodCall(() => Property(expr.Object, pi, expr.Arguments.ToArray()));
                return;
            }

            if (expr.Object == null) {
                WriteMethodCall(() => Call(expr.Method, expr.Arguments.ToArray()));
            } else {
                WriteMethodCall(() => Call(expr.Object, expr.Method, expr.Arguments.ToArray()));
            }
        }

        protected override void WriteMemberInit(MemberInitExpression expr) => 
            WriteMethodCall(() => MemberInit(expr.NewExpression, expr.Bindings.ToArray()));

        protected override void WriteListInit(ListInitExpression expr) => 
            WriteMethodCall(() => ListInit(expr.NewExpression, expr.Initializers.ToArray()));

        protected override void WriteNewArray(NewArrayExpression expr) {
            switch (expr.NodeType) {
                case ExpressionType.NewArrayInit:
                    WriteMethodCall(() => NewArrayInit(expr.Type.GetElementType(), expr.Expressions.ToArray()));
                    break;
                case ExpressionType.NewArrayBounds:
                    WriteMethodCall(() => NewArrayBounds(expr.Type.GetElementType(), expr.Expressions.ToArray()));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void WriteConditional(ConditionalExpression expr, object? metadata) {
            if (expr.Type != typeof(void)) {
                WriteMethodCall(() => Condition(expr.Test, expr.IfTrue, expr.IfFalse));
            } else if (expr.IfFalse.IsEmpty()) {
                WriteMethodCall(() => IfThen(expr.Test, expr.IfTrue));
            } else {
                WriteMethodCall(() => IfThenElse(expr.Test, expr.IfTrue, expr.IfFalse));
            }
        }

        protected override void WriteDefault(DefaultExpression expr) {
            if (expr.Type == typeof(void)) {
                WriteMethodCall(() => Empty());
                return;
            }
            WriteMethodCall(() => Default(expr.Type));
        }

        protected override void WriteTypeBinary(TypeBinaryExpression expr) {
            switch (expr.NodeType) {
                case ExpressionType.TypeIs:
                    WriteMethodCall(() => TypeIs(expr.Expression, expr.TypeOperand));
                    break;
                case ExpressionType.TypeEqual:
                    WriteMethodCall(() => TypeEqual(expr.Expression, expr.TypeOperand));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void WriteInvocation(InvocationExpression expr) => 
            WriteMethodCall(() => Invoke(expr.Expression, expr.Arguments.ToArray()));

        protected override void WriteIndex(IndexExpression expr) {
            if (expr.Indexer != null) {
                WriteMethodCall(() => MakeIndex(expr.Object, expr.Indexer, expr.Arguments));
                return;
            }
            WriteMethodCall(() => ArrayAccess(expr.Object, expr.Arguments.ToArray()));
        }

        protected override void WriteBlock(BlockExpression expr, object? metadata) {
            if (expr.Type != expr.Expressions.Last().Type) {
                if (expr.Variables.Any()) {
                    WriteMethodCall(() => Block(expr.Type, expr.Variables, expr.Expressions.ToArray()));
                } else {
                    WriteMethodCall(() => Block(expr.Type, expr.Expressions.ToArray()));
                }
            } else {
                if (expr.Variables.Any()) {
                    WriteMethodCall(() => Block(expr.Variables, expr.Expressions.ToArray()));
                } else {
                    WriteMethodCall(() => Block(expr.Expressions.ToArray()));
                }
            }
        }

        protected override void WriteSwitch(SwitchExpression expr) {
            if (expr.DefaultBody == null) {
                WriteMethodCall(() => Switch(expr.SwitchValue, expr.Cases.ToArray()));
            } else {
                WriteMethodCall(() => Switch(expr.SwitchValue, expr.DefaultBody, expr.Cases.ToArray()));
            }
        }

        protected override void WriteTry(TryExpression expr) {
            if (expr.Fault != null) {
                if (expr.Finally != null || expr.Handlers.Any()) {
                    WriteMethodCall(() => MakeTry(expr.Type, expr.Body, expr.Finally, expr.Fault, expr.Handlers));
                } else {
                    WriteMethodCall(() => TryFault(expr.Body, expr.Fault));
                }
            } else if (expr.Finally != null) {
                if (expr.Handlers.Any()) {
                    WriteMethodCall(() => TryCatchFinally(expr.Body, expr.Finally, expr.Handlers.ToArray()));
                } else {
                    WriteMethodCall(() => TryFinally(expr.Body, expr.Finally));
                }
            } else {
                WriteMethodCall(() => TryCatch(expr.Body, expr.Handlers.ToArray()));
            }
        }

        protected override void WriteLabel(LabelExpression expr) {
            if (expr.DefaultValue.IsEmpty()) {
                WriteMethodCall(() => Label(expr.Target));
            } else {
                WriteMethodCall(() => Label(expr.Target, expr.DefaultValue));
            }
        }

        protected override void WriteGoto(GotoExpression expr) {
            var methodName = expr.Kind switch {
                GotoExpressionKind.Break => "Break",
                GotoExpressionKind.Continue => "Continue",
                GotoExpressionKind.Return => "Return",
                GotoExpressionKind.Goto => "Goto",
                _ => throw new NotImplementedException(),
            };
            var args = new List<(string, object)> { ("Target", expr.Target) };
            if (expr.Value != null) { args.Add(("Value", expr.Value)); }
            WriteMethodCall(methodName, args);
        }

        protected override void WriteLoop(LoopExpression expr) {
            if (expr.BreakLabel != null && expr.ContinueLabel != null) {
                WriteMethodCall(() => Loop(expr.Body, expr.BreakLabel, expr.ContinueLabel));
            } else if (expr.BreakLabel != null) {
                WriteMethodCall(() => Loop(expr.Body, expr.BreakLabel));
            } else {
                WriteMethodCall(() => Loop(expr.Body));
            }
        }

        protected override void WriteRuntimeVariables(RuntimeVariablesExpression expr) => 
            WriteMethodCall(() => RuntimeVariables(expr.Variables.ToArray()));
        protected override void WriteDebugInfo(DebugInfoExpression expr) {
            if (expr.IsClear) {
                WriteMethodCall(() => ClearDebugInfo(expr.Document));
            } else {
                WriteMethodCall(() => DebugInfo(expr.Document, expr.StartLine, expr.StartColumn, expr.EndLine, expr.EndColumn));
            }
        }

        protected override void WriteElementInit(ElementInit elementInit) => 
            WriteMethodCall(() => ElementInit(elementInit.AddMethod, elementInit.Arguments.ToArray()));

        protected override void WriteBinding(MemberBinding binding) {
            switch (binding) {
                case MemberAssignment assignmentBinding:
                    WriteMethodCall(() => Bind(assignmentBinding.Member, assignmentBinding.Expression));
                    break;
                case MemberListBinding listBinding:
                    WriteMethodCall(() => ListBind(listBinding.Member, listBinding.Initializers.ToArray()));
                    break;
                case MemberMemberBinding memberMemberBinding:
                    WriteMethodCall(() => MemberBind(memberMemberBinding.Member, memberMemberBinding.Bindings.ToArray()));
                    break;
            }
        }

        protected override void WriteSwitchCase(SwitchCase switchCase) => 
            WriteMethodCall(() => SwitchCase(switchCase.Body, switchCase.TestValues.ToArray()));

        protected override void WriteCatchBlock(CatchBlock catchBlock) {
            if (catchBlock.Variable != null) {
                if (catchBlock.Variable.Type != catchBlock.Test) {
                    WriteMethodCall(() => MakeCatchBlock(catchBlock.Test, catchBlock.Variable, catchBlock.Body, catchBlock.Filter));
                } else if (catchBlock.Filter != null) {
                    WriteMethodCall(() => Catch(catchBlock.Variable, catchBlock.Body, catchBlock.Filter));
                } else {
                    WriteMethodCall(() => Catch(catchBlock.Variable, catchBlock.Body));
                }
            } else {
                if (catchBlock.Filter != null) {
                    WriteMethodCall(() => Catch(catchBlock.Test, catchBlock.Body, catchBlock.Filter));
                } else {
                    WriteMethodCall(() => Catch(catchBlock.Test, catchBlock.Body));
                }
            }
        }

        protected override void WriteLabelTarget(LabelTarget labelTarget) {
            if (labelTarget.Type == typeof(void)) {
                if (labelTarget.Name == null) {
                    WriteMethodCall(() => Label());
                } else {
                    WriteMethodCall(() => Label(labelTarget.Name));
                }
            } else {
                if (labelTarget.Name == null) {
                    WriteMethodCall(() => Label(labelTarget.Type));
                } else {
                    WriteMethodCall(() => Label(labelTarget.Type, labelTarget.Name));
                }
            }
        }

        protected override void WriteDynamic(DynamicExpression expr) => WriteMethodCall(() => Dynamic(expr.Binder, expr.Type, expr.Arguments));

        protected override void WriteBinaryOperationBinder(BinaryOperationBinder binaryOperationBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteConvertBinder(ConvertBinder convertBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteCreateInstanceBinder(CreateInstanceBinder createInstanceBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteDeleteIndexBinder(DeleteIndexBinder deleteIndexBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteDeleteMemberBinder(DeleteMemberBinder deleteMemberBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteGetIndexBinder(GetIndexBinder getIndexBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteGetMemberBinder(GetMemberBinder getMemberBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteInvokeBinder(InvokeBinder invokeBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteInvokeMemberBinder(InvokeMemberBinder invokeMemberBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteSetIndexBinder(SetIndexBinder setIndexBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteSetMemberBinder(SetMemberBinder setMemberBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteUnaryOperationBinder(UnaryOperationBinder unaryOperationBinder, IList<Expression> args) => throw new NotImplementedException();
        protected override void WriteParameterDeclaration(ParameterExpression prm) {
            if (language == CSharp) {
                Write($"var {prm.Name} = ");
            } else { // language == VisualBasic
                Write($"Dim {prm.Name} = ");
            }
            if (prm.IsByRef) {
                var type = prm.Type.MakeByRefType();
                (string, object)[] args = prm.Name.IsNullOrWhitespace() ?
                    new (string, object)[] { ("Type", type)} :
                    new (string, object)[] { ("Type", type), ("Name", prm.Name) };
                WriteMethodCall("Parameter", args);
            } else {
                if (prm.Name.IsNullOrWhitespace()) {
                    WriteMethodCall(() => Parameter(prm.Type));
                } else {
                    WriteMethodCall(() => Parameter(prm.Type, prm.Name));
                }
            }
        }
    }
}
