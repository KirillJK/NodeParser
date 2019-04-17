using System;
using System.Collections.Generic;
using System.Linq;
using ClassLibrary3;
using NUnit.Framework;

namespace ClassLibrary1.Tests
{
    [TestFixture]
    public class Class1
    {
        private Node ExpressionBuild(string expression)
        {
            var parts = expression.Split(">");
            return new Node()
            {
                Id = parts[0],
                Dependencies = parts.Length == 2 && !string.IsNullOrEmpty(parts[1])?parts[1].Split(",").ToList(): new List<string>()
            };
        }

        private List<Node> ExpressionsBuild(params string[] expressions)
        {
            return expressions.Select(ExpressionBuild).ToList();
        }

        [Test]
        public void SimpleCase()
        {
            var nodes = ExpressionsBuild(
                "A>B,C",
                "B>C,D",
                "D>",
                "C>"
            );
            var result = NodeParser.Parse(nodes);
            Assert.AreEqual("D,C,B,A", result.Select(a=>a.Id).Aggregate((a,b)=>a+","+b));
        }

        [Test]
        public void CycleDependency()
        {
            var nodes = ExpressionsBuild(
                "A>B,C",
                "B>C,D",
                "D>",
                "C>A"
            );
            try
            {
                var result = NodeParser.Parse(nodes);
            }
            catch (CycleDependencyException e)
            {
                CollectionAssert.AreEquivalent(new List<string>(){"A","B","C"}, e.UnresolvedNodes );
            }
        }

        [Test]
        public void EmptyCase()
        {
            var result = NodeParser.Parse(new List<Node>());
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void ExceedExpression()
        {
            var nodes = ExpressionsBuild(
                "A>B,E",
                "B>C,D",
                "D>F",
                "C>"
            );
            try
            {
                var result = NodeParser.Parse(nodes);
            }
            catch (ExceedDependencyException e)
            {
                CollectionAssert.AreEquivalent(new List<string>() { "E","F" }, e.ExceedNodes);
            }
        }
    }
}
