﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using static ExpressionTreeTestObjects.Functions;
using static ExpressionTreeTestObjects.Categories;

namespace ExpressionTreeTestObjects {
    public class Foo {
        public string? Bar { get; set; }
        public string? Baz { get; set; }
        public Foo() { }
        public Foo(string baz) { }
    }

    internal class Wrapper : List<string> {
        internal void Add(string s1, string s2) { }
    }

    // class used for MemberMemberBinding and ListBinding
    public class Node {
        internal NodeData Data { get; set; } = new NodeData();
        internal IList<Node?> Children { get; set; } = new List<Node?>() { null };
    }

    public class NodeData {
        internal string? Name { get; set; }
    }

    partial class CSCompiler {

        [TestObject(NewObject)]
        internal static readonly Expression NamedType = Expr(() => new Random());

        [TestObject(NewObject)]
        internal static readonly Expression NamedTypeWithInitializer = Expr(() => new Foo { Bar = "abcd" });

        [TestObject(NewObject)]
        internal static readonly Expression NamedTypeWithInitializers = Expr(() => new Foo { Bar = "abcd", Baz = "efgh" });

        [TestObject(NewObject)]
        internal static readonly Expression NamedTypeConstructorParameters = Expr(() => new Foo("ijkl"));

        [TestObject(NewObject)]
        internal static readonly Expression NamedTypeConstructorParametersWithInitializers = Expr(() => new Foo("ijkl") { Bar = "abcd", Baz = "efgh" });

        [TestObject(NewObject)]
        internal static readonly Expression AnonymousType = Expr(() => new { Bar = "abcd", Baz = "efgh" });

        [TestObject(NewObject)]
        internal static readonly Expression AnonymousTypeFromVariables = IIFE(() => {
            var Bar = "abcd";
            var Baz = "efgh";
            return Expr(() => new { Bar, Baz });
        });

        [TestObject(NewObject)]
        internal static readonly Expression CollectionTypeWithInitializer = Expr(() => new List<string> { "abcd", "efgh" });

        [TestObject(NewObject)]
        internal static readonly Expression CollectionTypeWithMultipleElementsInitializers = Expr(() => new Wrapper { { "ab", "cd" }, { "ef", "gh" } });

        [TestObject(NewObject)]
        internal static readonly Expression CollectionTypeWithSingleOrMultipleElementsInitializers = Expr(() => new Wrapper { { "ab", "cd" }, "ef" });

        [TestObject(NewObject)]
        internal static readonly Expression MemberMemberBinding = Expr(() => new Node { Data = { Name = "abcd" } });

        [TestObject(NewObject)]
        internal static readonly Expression ListBinding = Expr(() => new Node { Children = { new Node(), new Node() } });
    }
}