using System;
using System.Collections.Generic;
using System.Linq;

namespace Scripts.Game {
    // 状态管理
    public class StateManager {
        //状态机列表
        private Dictionary<string, State> dic = new Dictionary<string, State>();

        //总状态更改条件
        private Dictionary<string, Func<bool>> conditions = new Dictionary<string, Func<bool>>();

        //当前状态
        private State current;

        internal string Name {
            get => current?.name;
            set {
                if (value == Name || !dic.TryGetValue(value, out State next)) return;
                current?.exit();
                next.enter();
                current = next;
            }
        }

        // 添加一个状态机
        public void AddStatus(string name, State state) {
            dic.Add(name, state);
            state.name = name;
        }

        // 更新
        public void Tick() {
            if (current == null) return;
            current.stay();
            //总条件跳转
            foreach (KeyValuePair<string, Func<bool>> entry in conditions.Where(entry => entry.Key != Name && entry.Value())) {
                Name = entry.Key;
                return;
            }
            //子条件跳转
            foreach (KeyValuePair<string, Func<bool>> entry in current.conditions.Where(entry => entry.Value())) {
                Name = entry.Key;
                return;
            }
        }

        // 添加总状态机条件
        public void AddCondition(string status, Func<bool> condition) {
            conditions.Add(status, condition);
        }

        // 清理
        public void Clear() {
            dic.Clear();
            conditions.Clear();
        }
    }

    // 状态委托
    public class State {
        private static Action EmptyAction = () => { };

        internal string name;
        internal Action enter = EmptyAction;
        internal Action stay = EmptyAction;
        internal Action exit = EmptyAction;

        internal Dictionary<string, Func<bool>> conditions = new Dictionary<string, Func<bool>>();

        // 设置进入方法
        public State OnEnter(Action action) {
            enter = action;
            return this;
        }

        // 设置持续方法(Update)
        public State OnStay(Action action) {
            stay = action;
            return this;
        }

        // 设置退出方法
        public State OnExit(Action action) {
            exit = action;
            return this;
        }

        // 添加状态跳转条件
        public State AddCondition(string status, Func<bool> condition) {
            conditions.Add(status, condition);
            return this;
        }
    }
}