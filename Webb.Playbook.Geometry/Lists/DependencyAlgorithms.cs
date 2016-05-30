using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Webb.Playbook.Geometry.Lists
{
    public static class DependencyAlgorithms
    {
        public static IEnumerable<T> TopologicalSort<T>(
            this IEnumerable<T> originalSet,
            Func<T, IEnumerable<T>> childrenSelector)
        {
            Dictionary<T, bool> visitedSet = new Dictionary<T, bool>();
            List<T> finalOrder = new List<T>();

            AddAllDependents(
                originalSet,
                childrenSelector,
                visitedSet,
                finalOrder);

            return finalOrder;
        }

        private static void AddAllDependents<T>(
            IEnumerable<T> nodes,
            Func<T, IEnumerable<T>> childrenSelector,
            Dictionary<T, bool> visitedSet,
            List<T> finalOrder)
        {
            foreach (var node in nodes)
            {
                if (!visitedSet.ContainsKey(node))
                {
                    visitedSet.Add(node, true);
                    var children = childrenSelector(node);
                    if (!children.IsEmpty())
                    {
                        AddAllDependents(
                            children,
                            childrenSelector,
                            visitedSet,
                            finalOrder);
                    }
                    finalOrder.Add(node);
                }
            }
        }

        public static IEnumerable<T> FindDescendants<T>(Func<T, IEnumerable<T>> childrenSelector, IEnumerable<T> list)
        {
            return TopologicalSort(list, childrenSelector);
        }

        public static IEnumerable<T> FindRoots<T>(Func<T, IEnumerable<T>> childrenSelector, params T[] list)
        {
            return FindRoots(childrenSelector, (IEnumerable<T>)list);
        }

        public static IEnumerable<T> FindRoots<T>(Func<T, IEnumerable<T>> childrenSelector, IEnumerable<T> list)
        {
            List<T> result = new List<T>();
            foreach (var figure in list)
            {
                FindRoots(childrenSelector, figure, result.Add);
            }

            return result.Distinct();
        }

        public static void FindRoots<T>(Func<T, IEnumerable<T>> childrenSelector, T figure, Action<T> collector)
        {
            var children = childrenSelector(figure);
            if (children.IsEmpty())
            {
                collector(figure);
            }
            else
            {
                foreach (var dependency in children)
                {
                    FindRoots(childrenSelector, dependency, collector);
                }
            }
        }
    }
}
