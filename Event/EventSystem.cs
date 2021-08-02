using System;
using System.Collections.Generic;
using System.Reflection;
using Tools;

namespace Script.Event {
    public class EventSystem : Single<EventSystem> {

        // 事件 - 对象 - 方法  <Event, <Listener, List<MethodInfo>>>
        private Dictionary<Type, Dictionary<Listener, List<MethodInfo>>> dic = new Dictionary<Type, Dictionary<Listener, List<MethodInfo>>>();

        /**
         * 添加监听器
         */
        public void AddListener(Listener listener) {
            MethodInfo[] methodInfos = listener.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreReturn);
            foreach (MethodInfo methodInfo in methodInfos) {
                if (methodInfo.ReturnType == typeof(void) && Attribute.GetCustomAttribute(methodInfo, typeof(EventHandler)) != null) {
                    // 判断是否符合要求
                    if (methodInfo.GetParameters().Length != 1) {
                        throw new Exception("方法: " + methodInfo.Name + " 的参数数量不为1!");
                    }
                    Type eventType = methodInfo.GetParameters()[0].ParameterType;
                    if (eventType.IsAbstract) {
                        throw new Exception("方法: " + methodInfo.Name + " 未使用可实例化参数!");
                    }
                    if (!eventType.IsSubclassOf(typeof(Event))) {
                        throw new Exception("方法: " + methodInfo.Name + " 的参数未继承于Event!");
                    }
                    // 符合条件的添加方式
                    if (!dic.TryGetValue(eventType, out Dictionary<Listener, List<MethodInfo>> listenerList)) {
                        listenerList = new Dictionary<Listener, List<MethodInfo>>();
                        dic.Add(eventType, listenerList);
                    }
                    if (!listenerList.TryGetValue(listener, out List<MethodInfo> methods)) {
                        methods = new List<MethodInfo>();
                        listenerList.Add(listener, methods);
                    }
                    methods.Add(methodInfo);
                }
            }
        }

        /**
         * 删除监听器
         */
        public void RemoveListener(Listener listener) {
            MethodInfo[] methodInfos = listener.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreReturn);
            foreach (MethodInfo methodInfo in methodInfos) {
                if (methodInfo.ReturnType == typeof(void) && Attribute.GetCustomAttribute(methodInfo, typeof(EventHandler)) != null && methodInfo.GetParameters().Length == 1) {
                    Type eventType = methodInfo.GetParameters()[0].ParameterType;
                    if (!eventType.IsAbstract && eventType.IsSubclassOf(typeof(Event)) && dic.TryGetValue(eventType, out Dictionary<Listener, List<MethodInfo>> listenerList) && listenerList.ContainsKey(listener)) {
                        listenerList.Remove(listener);
                    }
                }
            }
        }
        
        
        /**
         * 通知事件
         */
        public void Call(Event callEvent) {
            if (dic.TryGetValue(callEvent.GetType(), out Dictionary<Listener, List<MethodInfo>> listenerList)) {
                foreach (KeyValuePair<Listener,List<MethodInfo>> entry in listenerList) {
                    Listener listener = entry.Key;
                    foreach (MethodInfo methodInfo in entry.Value) {
                        methodInfo.Invoke(listener, new object[] {callEvent});
                    }
                }
            }
        }

        // 使用示范
        // public static void Main() {
        //     Inst().AddListener(new TestListener());
        //     Inst().AddListener(new ZzzZzzListener());
        //     Listener listener = new ZzzZzzListener();
        //     Inst().AddListener(listener);
        //     Console.WriteLine();
        //     Console.WriteLine("等待2秒");
        //     Console.WriteLine();
        //     Thread.Sleep(2000);
        //     Inst().Call(new TestEvent("小明", " 去吃饭了"));
        //     Inst().Call(new QVQEvent("芳芳", 233));
        //     Console.WriteLine();
        //     Inst().RemoveListener(listener);
        //     Thread.Sleep(2000);
        //     Console.WriteLine();
        //     Console.WriteLine("延迟2秒");
        //     Thread.Sleep(1000);
        //     Console.WriteLine("Call Event");
        //     Inst().Call(new TestEvent("小王", " 去上网了"));
        //     Inst().Call(new QVQEvent("吱吱吱吱", 666));
        // }
    }
}