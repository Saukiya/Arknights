using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Game {
    // 目标选择器
    // 虽然很单一 但是够用
    public class TargetSelector<T> where T : Entity {
        // 运行方式？Entity实例化的时候实例化

        private Transform transform;
        private List<Vector3> range;
        private List<Entity> entityList = Dungeon.inst.list;

        // internal T this[int value] => GetTargets()[value];

        private SortPackage<T> sort;

        // 实例化方式：给予一个带有方向的坐标
        public TargetSelector(Transform transform, IEnumerable<Vector3> range) {
            this.transform = transform;
            this.range = new List<Vector3>(range);
        }

        // 获得目标列表
        public List<T> GetTargets() {
            List<T> list = new List<T>();
            foreach (Vector3 position in range.Select(vector => Quaternion.LookRotation(transform.forward) * vector).Select(position => position + transform.position)) {
                // list.AddRange(entityList.Where(entity => entity.GetType() == typeof(T) && entity.transform.position.Round() == position.Round() && !entity.isDead()).Cast<T>());
                list.AddRange(entityList.Where(entity => entity.GetType() == typeof(T) && Vector3.Distance(entity.transform.position, position) < 0.7 && !entity.isDead()).Cast<T>());
            }
            if (sort != null) list.Sort(sort);
            return list;
        }

        //设置排序方法
        public void SetSort(SortPackage<T>.CompareDelegate cd) {
            sort = new SortPackage<T>(cd);
        }
    }

    
    // 用委托封装排序接口（为什么要封装？？C#的接口为什么为什么不能像JJJJJJJJJJJava那样子）
    public class SortPackage<T> : IComparer<T> where T : Entity {
        internal CompareDelegate cd;

        public SortPackage(CompareDelegate cd) {
            this.cd = cd;
        }
        
        public delegate int CompareDelegate(T a, T b);
        public int Compare(T x, T y) {
            return cd(x, y);
        }
    }
}