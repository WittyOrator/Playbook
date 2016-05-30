using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Webb.Playbook.Geometry.Shapes;

namespace Webb.Playbook.Geometry.Lists
{
    public class DependencyList : List<Type>
    {
        public DependencyList()
        {

        }

        static class SingletonHelper<T>
            where T : DependencyList, new()
        {
            public static readonly DependencyList Instance = new T();
        }

        private class DependencyList1<T> : DependencyList
        {
            public DependencyList1() : base(typeof(T)) { }
        }

        private class DependencyList2<T, T2> : DependencyList
        {
            public DependencyList2() : base(typeof(T), typeof(T2)) { }
        }

        private class DependencyList3<T, T2, T3> : DependencyList
        {
            public DependencyList3() : base(typeof(T), typeof(T2), typeof(T3)) { }
        }

        public static readonly DependencyList None = Create();
        public static readonly DependencyList Point = Create<IPoint>();
        public static readonly DependencyList PointPoint = Create<IPoint, IPoint>();
        public static readonly DependencyList PointPointPoint = Create<IPoint, IPoint, IPoint>();
        public static readonly DependencyList LinePoint = Create<ILine, IPoint>();

        public DependencyList(params Type[] types)
        {
            AddRange(types);
        }

        public static DependencyList Create()
        {
            return SingletonHelper<DependencyList>.Instance;
        }

        public static DependencyList Create<T>()
        {
            return SingletonHelper<DependencyList1<T>>.Instance;
        }

        public static DependencyList Create<T, T2>()
        {
            return SingletonHelper<DependencyList2<T, T2>>.Instance;
        }

        public static DependencyList Create<T, T2, T3>()
        {
            return SingletonHelper<DependencyList3<T, T2, T3>>.Instance;
        }
    }
}
