using UnityEngine;

namespace Tools {

    public abstract class Single<T> where T : Single<T>,new() {
        private static T inst;
        public static T Inst() {
            if (inst != null) return inst;
            inst = new T();
            inst.Initialization();
            return inst;
        }

        protected virtual void Initialization() { }
    }

    public abstract class MonoSingle<T> : MonoBehaviour where T : MonoSingle<T> {
        private static T inst;
        
        public static T Inst() {
            if (inst) return inst;
            GameObject gameObject = new GameObject(typeof(T).Name);
            inst = gameObject.AddComponent<T>();
            DontDestroyOnLoad(gameObject);
            return inst;
        }
        
        protected virtual void Initialization() { }

        private void Awake() {
            Initialization();
        }
    }
}