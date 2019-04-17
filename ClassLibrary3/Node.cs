using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary3
{
    public class Node
    {
        public string Id { get; set; }
        public List<string> Dependencies { get; set; } = new List<string>();
    }

    public static class NodeParser
    {


        private static void CheckExceed(List<Node> nodes)
        {
            var allNodesFromDepndencies = new HashSet<string>(nodes.SelectMany(a => a.Dependencies));
            var allNodesFromRoot = nodes.Select(a => a.Id).ToList();
            if (allNodesFromDepndencies.Except(allNodesFromRoot).Count()!=0)
            {
                throw new ExceedDependencyException(allNodesFromDepndencies.Where(a => !allNodesFromRoot.Contains(a))
                    .ToList());
            }
        }

        public static Queue<Node> Parse(List<Node> nodes)
        {
            CheckExceed(nodes);
            HashSet<string> map = new HashSet<string>();
            var source = nodes.ToList();
            Queue<Node> result = new Queue<Node>();
            int count = -1;
            while (count!= map.Count)
            {
                count = map.Count;
                var independent = ExtractAllIndependent(source, map);
                foreach (var node in independent)
                {
                    result.Enqueue(node);
                    map.Add(node.Id);
                }

                source.RemoveAll(a => map.Contains(a.Id));
            }

            if (result.Count != nodes.Count)
            {
                throw new CycleDependencyException(nodes.Select(a => a.Id).Except(result.Select(a => a.Id)).ToList());
            }
            return result;
        }

        private static List<Node> ExtractAllIndependent(List<Node> nodes, HashSet<string> map)
        {
            return nodes.Where(a => a.Dependencies.All(map.Contains)).ToList();
        }
    }

    public class CycleDependencyException : Exception
    {
        public CycleDependencyException(List<string> unresolvedNodes)
        {
            UnresolvedNodes = unresolvedNodes;
        }

        public List<string> UnresolvedNodes { get; set; }

    }

    public class ExceedDependencyException : Exception
    {
        public List<string> ExceedNodes { get; set; }

        public ExceedDependencyException(List<string> exceedNodes)
        {
            ExceedNodes = exceedNodes;
        }
    }
}
