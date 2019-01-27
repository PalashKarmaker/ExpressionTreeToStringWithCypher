﻿//using ExpressionTreeTransform.Util;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.Editing;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Runtime.CompilerServices;
//using static Microsoft.CodeAnalysis.Formatting.Formatter;
//using static Microsoft.CodeAnalysis.LanguageNames;
//using static System.Linq.Expressions.ExpressionType;
//using CS = Microsoft.CodeAnalysis.CSharp;
//using VB = Microsoft.CodeAnalysis.VisualBasic;

//namespace ExpressionTreeTransform {
//    public class Mapper {
//        private string language;
//        private Workspace workspace;
//        private SyntaxGenerator generator;
//        private List<object> visitedObjects; // can be Expression, MemberBinding, ElementInit

//        public SyntaxNode GetSyntaxNode(Expression expr, string language) {
//            this.language = language;

//            // TODO within the visualizer, it may be possible to get the workspace / generator for the current code
//            workspace = new AdhocWorkspace();
//            generator = SyntaxGenerator.GetGenerator(workspace, language);
//            return Format(getSyntaxNode(expr).NormalizeWhitespace("    ", true), workspace);
//        }

//        // TODO keep track of closed over variables per closure, using passed-in List<(string closure, string name, Type type)>
//        public SyntaxNode GetSyntaxNode(Expression expr, string language, out ImmutableDictionary<object, SyntaxNode> mappedSyntaxNodes) {
//            visitedObjects = new List<object>();
//            var ret = GetSyntaxNode(expr, language);

//            var expressionIDs = visitedObjects.Select((x, index) => (x, index)).ToImmutableDictionary();
//            var annotatedNodes = ret.GetAnnotatedNodes("nodeID").SelectMany(node => node.GetAnnotations("nodeID").Select(x=>(int.Parse(x.Data), node))).ToImmutableDictionary();
//            mappedSyntaxNodes = visitedObjects.Select((x, index) => (x, annotatedNodes[index])).ToImmutableDictionary();

//            return ret;
//        }

//        private SyntaxNode getSyntaxNode(Expression expr) {
//            SyntaxNode ret;
//            switch (expr.NodeType) {

//                #region BinaryExpression

//                // mathematical operations
//                case Add:
//                case AddChecked:
//                case Divide:
//                case Modulo:
//                case Multiply:
//                case MultiplyChecked:
//                case Power:
//                case Subtract:
//                case SubtractChecked:

//                // bitwise / logical operations
//                case And:
//                case Or:
//                case ExclusiveOr:

//                // shift operations
//                case LeftShift:
//                case RightShift:

//                // conditional boolean operators
//                case AndAlso:
//                case OrElse:

//                // comparison operators
//                case Equal:
//                case NotEqual:
//                case GreaterThanOrEqual:
//                case GreaterThan:
//                case LessThan:
//                case LessThanOrEqual:

//                // coalescing operators
//                case Coalesce:

//                // array indexing operations
//                case ArrayIndex:
//                    ret = getSyntaxNode(expr as BinaryExpression);
//                    break;

//                #endregion

//                #region UnaryExpression

//                case ArrayLength:
//                case ExpressionType.Convert:
//                case ConvertChecked:
//                case Negate:
//                case NegateChecked:
//                case Not:
//                case Quote:
//                case TypeAs:
//                case UnaryPlus:
//                    ret = getSyntaxNode(expr as UnaryExpression);
//                    break;

//                #endregion

//                case Lambda:
//                    ret = getSyntaxNode(expr as LambdaExpression);
//                    break;

//                case Parameter:
//                    ret = getSyntaxNode(expr as ParameterExpression);
//                    break;

//                case Constant:
//                    ret = getSyntaxNode(expr as ConstantExpression);
//                    break;

//                case MemberAccess:
//                    ret = getSyntaxNode(expr as MemberExpression);
//                    break;

//                case New:
//                    ret = getSyntaxNode(expr as NewExpression);
//                    break;

//                case Call:
//                    ret = getSyntaxNode(expr as MethodCallExpression);
//                    break;

//                case MemberInit:
//                    ret = getSyntaxNode(expr as MemberInitExpression);
//                    break;

//                case ListInit:
//                    ret = getSyntaxNode(expr as ListInitExpression);
//                    break;

//                default:
//                    throw new NotImplementedException($"NodeType: {expr.NodeType}, Expression object type: {expr.GetType().Name}");

//                    /*case AddAssign:
//                        break;
//                    case AddAssignChecked:
//                        break;
//                    case AndAssign:
//                        break;
//                    case Assign:
//                        break;
//                    case Block:
//                        break;
//                    case Conditional:
//                        break;
//                    case DebugInfo:
//                        break;
//                    case Decrement:
//                        break;
//                    case Default:
//                        break;
//                    case DivideAssign:
//                        break;
//                    case Dynamic:
//                        break;
//                    case ExclusiveOrAssign:
//                        break;
//                    case Extension:
//                        break;
//                    case Goto:
//                        break;
//                    case Increment:
//                        break;
//                    case Index:
//                        break;
//                    case Invoke:
//                        break;
//                    case IsFalse:
//                        break;
//                    case IsTrue:
//                        break;
//                    case Label:
//                        break;
//                    case LeftShiftAssign:
//                        break;
//                    case Loop:
//                        break;
//                    case ModuloAssign:
//                        break;
//                    case MultiplyAssign:
//                        break;
//                    case MultiplyAssignChecked:
//                        break;
//                    case NewArrayBounds:
//                        break;
//                    case NewArrayInit:
//                        break;
//                    case OnesComplement:
//                        break;
//                    case OrAssign:
//                        break;
//                    case PostDecrementAssign:
//                        break;
//                    case PostIncrementAssign:
//                        break;
//                    case PowerAssign:
//                        break;
//                    case PreDecrementAssign:
//                        break;
//                    case PreIncrementAssign:
//                        break;
//                    case RightShiftAssign:
//                        break;
//                    case RuntimeVariables:
//                        break;
//                    case SubtractAssign:
//                        break;
//                    case SubtractAssignChecked:
//                        break;
//                    case Switch:
//                        break;
//                    case Throw:
//                        break;
//                    case Try:
//                        break;
//                    case TypeEqual:
//                        break;
//                    case TypeIs:
//                        break;
//                    case Unbox:
//                        break;
//                    default:
//                        break;*/
//            }

//            registerVisited(expr, ref ret);
//            return ret;
//        }

//        private void registerVisited<T>(object o, ref T node) where T : SyntaxNode {
//            if (visitedObjects == null || visitedObjects.Contains(o)) { return; }
//            visitedObjects.Add(o);
//            node = node.WithAdditionalAnnotations(new SyntaxAnnotation("nodeID", (visitedObjects.Count - 1).ToString()));
//        }

//        private SyntaxNode getSyntaxNode(BinaryExpression expr) {
//            Func<SyntaxNode, SyntaxNode, SyntaxNode> method = null;
//            var left = getSyntaxNode(expr.Left);
//            var right = getSyntaxNode(expr.Right);

//            switch (expr.NodeType) {
//                // binary arithmetic operations
//                case Add:
//                case AddChecked:
//                    method = generator.AddExpression;
//                    break;
//                case Divide:
//                    method = generator.DivideExpression;
//                    break;
//                case Modulo:
//                    method = generator.ModuloExpression;
//                    break;
//                case Multiply:
//                case MultiplyChecked:
//                    method = generator.MultiplyExpression;
//                    break;

//                case Power:
//                    if (language == VisualBasic) {
//                        return VB.SyntaxFactory.ExponentiateExpression(
//                            (VB.Syntax.ExpressionSyntax)left,
//                            (VB.Syntax.ExpressionSyntax)right
//                        );
//                    }
//                    //TODO use Math.Power
//                    throw new NotImplementedException();

//                case Subtract:
//                case SubtractChecked:
//                    method = generator.SubtractExpression;
//                    break;

//                // bitwise / logical operations
//                case And:
//                    method = generator.BitwiseAndExpression;
//                    break;
//                case Or:
//                    method = generator.BitwiseOrExpression;
//                    break;

//                case ExclusiveOr:
//                    if (language == CSharp) {
//                        return CS.SyntaxFactory.BinaryExpression(
//                            CS.SyntaxKind.ExclusiveOrExpression,
//                            (CS.Syntax.ExpressionSyntax)left,
//                            (CS.Syntax.ExpressionSyntax)right
//                        );
//                    } else if (language == VisualBasic) {
//                        return VB.SyntaxFactory.ExclusiveOrExpression(
//                            (VB.Syntax.ExpressionSyntax)left,
//                            (VB.Syntax.ExpressionSyntax)right
//                        );
//                    }
//                    throw new NotImplementedException();

//                // shift operations
//                case LeftShift:
//                    throw new NotImplementedException();
//                case RightShift:
//                    throw new NotImplementedException();

//                // conditional boolean operators
//                case AndAlso:
//                    method = generator.LogicalAndExpression;
//                    break;
//                case OrElse:
//                    method = generator.LogicalOrExpression;
//                    break;

//                // comparison operators
//                case Equal:
//                    method = generator.ValueEqualsExpression;
//                    break;
//                case NotEqual:
//                    method = generator.ValueNotEqualsExpression;
//                    break;
//                case GreaterThanOrEqual:
//                    method = generator.GreaterThanOrEqualExpression;
//                    break;
//                case GreaterThan:
//                    method = generator.GreaterThanExpression;
//                    break;
//                case LessThan:
//                    method = generator.LessThanExpression;
//                    break;
//                case LessThanOrEqual:
//                    method = generator.LessThanOrEqualExpression;
//                    break;

//                // coalescing operators
//                case Coalesce:
//                    method = generator.CoalesceExpression;
//                    break;

//                // array indexing operations
//                case ArrayIndex:
//                    if (language == CSharp) {
//                        return CS.SyntaxFactory.ElementAccessExpression(
//                            (CS.Syntax.ExpressionSyntax)left,
//                            CS.SyntaxFactory.BracketedArgumentList(
//                                CS.SyntaxFactory.SingletonSeparatedList(
//                                    CS.SyntaxFactory.Argument(
//                                        (CS.Syntax.ExpressionSyntax)right
//                                    )
//                                )
//                            )
//                        );
//                    } else if (language == VisualBasic) {
//                        return VB.SyntaxFactory.InvocationExpression(
//                            (VB.Syntax.ExpressionSyntax)left,
//                            VB.SyntaxFactory.ArgumentList(
//                                VB.SyntaxFactory.SeparatedList<VB.Syntax.ArgumentSyntax>(
//                                    VB.SyntaxFactory.SingletonSeparatedList(
//                                        VB.SyntaxFactory.SimpleArgument(
//                                            (VB.Syntax.ExpressionSyntax)right
//                                        )
//                                    )
//                                )
//                            )
//                        );
//                    }
//                    throw new NotImplementedException();

//                default:
//                    throw new NotImplementedException();
//            }

//            return method(left, right);
//        }

//        private SyntaxNode getSyntaxNode(UnaryExpression expr) {
//            Func<SyntaxNode, SyntaxNode> method = null;
//            var operand = getSyntaxNode(expr.Operand);

//            Func<SyntaxNode, SyntaxNode, SyntaxNode> typeMethod = null;

//            switch (expr.NodeType) {
//                case ArrayLength:
//                    generator.MemberAccessExpression(operand, "Length");
//                    break;
//                case ExpressionType.Convert:
//                case ConvertChecked:
//                    typeMethod = generator.CastExpression;
//                    break;
//                case Negate:
//                case NegateChecked:
//                    method = generator.NegateExpression;
//                    break;
//                case Not:
//                    if (expr.Type == typeof(bool)) {
//                        method = generator.LogicalNotExpression;
//                    } else if (expr.Type.IsNumeric()) {
//                        method = generator.BitwiseNotExpression;
//                    } else {
//                        throw new NotImplementedException();
//                    }
//                    break;
//                case Quote:
//                    throw new NotImplementedException();
//                case TypeAs:
//                    typeMethod = generator.TryCastExpression;
//                    break;
//                case UnaryPlus:
//                    throw new NotImplementedException();
//                default:
//                    throw new NotImplementedException();
//            }

//            if (typeMethod != null) {
//                return typeMethod(getSyntaxNode(expr.Type), operand);
//            }
//            return method(operand);
//        }

//        private SyntaxNode getSyntaxNode(LambdaExpression expr) {
//            var parameters = expr.Parameters.Select(x => generator.LambdaParameter(x.Name));
//            // TODO handle multple-statement expressions
//            var body = getSyntaxNode(expr.Body);
//            if (expr.ReturnType == typeof(void)) {
//                return generator.VoidReturningLambdaExpression(parameters, body);
//            } else {
//                return generator.ValueReturningLambdaExpression(parameters, body);
//            }
//        }

//        private SyntaxNode getSyntaxNode(ParameterExpression expr) => generator.IdentifierName(expr.Name);

//        // TODO the typename needs to be resolved based on the current imports
//        private SyntaxNode getSyntaxNode(Type t) {
            
            
//            return generator.IdentifierName(t.FriendlyName(language));
//        }

//        private (SyntaxNode, SyntaxAnnotation) parseValue(object value) {
//            SyntaxNode ret;
//            switch (value) {
//                case bool b:
//                    ret = b ? generator.TrueLiteralExpression() : generator.FalseLiteralExpression();
//                    break;
//                case null:
//                    ret = generator.NullLiteralExpression();
//                    break;
//                default:
//                    ret = generator.LiteralExpression(value);
//                    if (ret.IsKind(CS.SyntaxKind.NullLiteralExpression) || ret.IsKind(VB.SyntaxKind.NothingLiteralExpression)) {
//                        // constant cannot be represented as a literal
//                        ret = generator.IdentifierName($"#{value.GetType().Name}");
//                    }
//                    break;
//            }

//            string stringValue = null;
//            if (ret.IsLiteral()) {
//                stringValue = ret.ToFullString();
//            } else if (value.GetType().UnderlyingIfNullable().In(typeof(DateTime), typeof(TimeSpan))) {
//                stringValue = value.ToString();
//            } else {
//                stringValue = ret.ToFullString();
//            }
//            SyntaxAnnotation annotation = null;
//            if (!stringValue.IsNullOrWhitespace()) {
//                annotation = new SyntaxAnnotation("stringValue", stringValue);
//            }

//            return (ret, annotation);
//        }

//        private SyntaxNode getSyntaxNode(ConstantExpression expr) {
//            var (ret, annotation) = parseValue(expr.Value);
//            if (annotation != null) { ret = ret.WithAdditionalAnnotations(annotation); }
//            return ret;
//        }

//        private SyntaxNode getSyntaxNode(MemberExpression expr) {
//            if (expr.Expression is ConstantExpression cexpr && cexpr.Type.IsClosureClass()) {
//                var ret = generator.IdentifierName(expr.Member.Name.Replace("$VB$Local_", ""));
//                var value = expr.ExtractValue();
//                var (_, annotation) = parseValue(value);
//                if (annotation != null) { ret = ret.WithAdditionalAnnotations(annotation); }
//                return ret;
//            }

//            if (expr.Expression == null) {
//                // static member
//                return generator.IdentifierName($"{expr.Member.DeclaringType.Name}.{expr.Member.Name}");
//            }

//            return generator.MemberAccessExpression(getSyntaxNode(expr.Expression), expr.Member.Name);
//        }

//        private CS.Syntax.ExpressionSyntax getCSSyntaxNode(MemberBinding binding) {
//            CS.Syntax.ExpressionSyntax ret;
//            switch (binding) {
//                case MemberAssignment assignmentBinding:
//                    ret = CS.SyntaxFactory.AssignmentExpression(
//                        CS.SyntaxKind.SimpleAssignmentExpression,
//                        CS.SyntaxFactory.IdentifierName(assignmentBinding.Member.Name),
//                        (CS.Syntax.ExpressionSyntax)getSyntaxNode(assignmentBinding.Expression)
//                    );
//                    break;
//                case MemberListBinding listBinding:
//                    throw new NotImplementedException("C# - ListBinding");
//                case MemberMemberBinding memberBinding:
//                    throw new NotImplementedException("C# - MemberMemberBinding");
//                default:
//                    throw new NotImplementedException();
//            }
//            registerVisited(binding, ref ret);
//            return ret;
//        }

//        private VB.Syntax.FieldInitializerSyntax getVBSyntaxNode(MemberBinding binding) {
//            VB.Syntax.FieldInitializerSyntax ret;
//            switch (binding) {
//                case MemberAssignment assignmentBinding:
//                    ret = VB.SyntaxFactory.NamedFieldInitializer(
//                        VB.SyntaxFactory.IdentifierName(binding.Member.Name), 
//                        (VB.Syntax.ExpressionSyntax)getSyntaxNode(assignmentBinding.Expression)
//                    );
//                    break;
//                case MemberListBinding listBinding:
//                    throw new NotImplementedException("VB - ListBinding");
//                case MemberMemberBinding memberBinding:
//                    throw new NotImplementedException("VB - MemberMemberBinding");
//                default:
//                    throw new NotImplementedException();
//            }
//            registerVisited(binding, ref ret);
//            return ret;
//        }

//        // TODO handle collection initializers

//        private SyntaxNode getSyntaxNode(MemberInitExpression expr) {
//            var ret = getSyntaxNode(expr.NewExpression);
//            if (expr.Bindings.Any()) {
//                switch (language) {
//                    case CSharp:
//                        return ((CS.Syntax.ObjectCreationExpressionSyntax)ret).WithInitializer(
//                            CS.SyntaxFactory.InitializerExpression(
//                                CS.SyntaxKind.ObjectInitializerExpression,
//                                CS.SyntaxFactory.SeparatedList(expr.Bindings.Select(binding => getCSSyntaxNode(binding)))
//                            ));
//                    case VisualBasic:
//                        return ((VB.Syntax.ObjectCreationExpressionSyntax)ret).WithInitializer(
//                                VB.SyntaxFactory.ObjectMemberInitializer(expr.Bindings.Select(binding => getVBSyntaxNode(binding)).ToArray())
//                            );
//                    default:
//                        throw new NotImplementedException();
//                }
//            }
//            return ret;
//        }

//        // TODO what about generic constructors?
//        private SyntaxNode getSyntaxNode(NewExpression expr) =>
//            generator.ObjectCreationExpression(getSyntaxNode(expr.Type), expr.Arguments.Select(x => getSyntaxNode(x)));

//        private SyntaxNode getSyntaxNode(MethodCallExpression expr) {
//            Expression instance = null;
//            IEnumerable<Expression> arguments = expr.Arguments;

//            if (expr.Object != null) {
//                // instance method
//                instance = expr.Object;
//            } else if (expr.Method.HasAttribute<ExtensionAttribute>()) {
//                // extension method
//                instance = expr.Arguments[0];
//                arguments = expr.Arguments.Skip(1);
//            }

//            SyntaxNode invokeSubject =
//                instance != null ?
//                getSyntaxNode(instance) :
//                generator.IdentifierName(expr.Method.ReflectedType.Name);

//            return generator.InvocationExpression(
//                generator.MemberAccessExpression(
//                    invokeSubject, expr.Method.Name
//                ), arguments.Select(x => getSyntaxNode(x))
//            );
//        }

//        private CS.Syntax.ExpressionSyntax getCSSyntaxNode(ElementInit elementInit) {
//            var args = elementInit.Arguments.Select(arg => getSyntaxNode(arg)).Cast<CS.Syntax.ExpressionSyntax>();
//            CS.Syntax.ExpressionSyntax ret = null;
//            switch (elementInit.Arguments.Count) {
//                case 0:
//                    throw new NotImplementedException();
//                case 1:
//                    ret = args.First();
//                    break;
//                default:
//                    ret = CS.SyntaxFactory.InitializerExpression(
//                        CS.SyntaxKind.ComplexElementInitializerExpression,
//                        CS.SyntaxFactory.SeparatedList(args)
//                    );
//                    break;
//            }
//            registerVisited(elementInit, ref ret);
//            return ret;
//        }

//        private VB.Syntax.ExpressionSyntax getVBSyntaxNode(ElementInit elementInit) {
//            var args = elementInit.Arguments.Select(arg => getSyntaxNode(arg)).Cast<VB.Syntax.ExpressionSyntax>();
//            VB.Syntax.ExpressionSyntax ret;
//            switch (elementInit.Arguments.Count) {
//                case 0:
//                    throw new NotImplementedException();
//                case 1:
//                    ret = args.First();
//                    break;
//                default:
//                    ret = VB.SyntaxFactory.CollectionInitializer(
//                        VB.SyntaxFactory.SeparatedList(args)
//                    );
//                    break;
//            }
//            registerVisited(elementInit, ref ret);
//            return ret;
//        }

//        private SyntaxNode getSyntaxNode(ListInitExpression expr) {
//            var ret = getSyntaxNode(expr.NewExpression);
//            switch (language) {
//                case CSharp:
//                    ret= ((CS.Syntax.ObjectCreationExpressionSyntax)ret).WithInitializer(
//                         CS.SyntaxFactory.InitializerExpression(
//                             CS.SyntaxKind.CollectionInitializerExpression,
//                             CS.SyntaxFactory.SeparatedList(expr.Initializers.Select(init => getCSSyntaxNode(init)))
//                         ));
//                    break;
//                case VisualBasic:
//                    ret= ((VB.Syntax.ObjectCreationExpressionSyntax)ret).WithInitializer(
//                        VB.SyntaxFactory.ObjectCollectionInitializer(
//                            VB.SyntaxFactory.CollectionInitializer(
//                                VB.SyntaxFactory.SeparatedList(expr.Initializers.Select(init => getVBSyntaxNode(init)))
//                            )
//                        )
//                    );
//                    break;
//                default:
//                    throw new NotImplementedException();
//            }
//            registerVisited(expr, ref ret);
//            return ret;
//        }

//        // TODO handle anonymous type construction


//        //private SyntaxNode getSyntaxNode(BlockExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(ConditionalExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(DebugInfoExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(DefaultExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(DynamicExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(GotoExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(IndexExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(InvocationExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(LabelExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(LoopExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(NewArrayExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(RuntimeVariablesExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(SwitchExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(TryExpression expr) => throw new NotImplementedException();
//        //private SyntaxNode getSyntaxNode(TypeBinaryExpression expr) => throw new NotImplementedException();
//    }
//}