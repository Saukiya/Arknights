using System.IO;
using Manager;
using UnityEngine;

namespace Tools {
    public class Asset {
        public static T Load<T>(string path, string name) where T : Object {
            return ABManager.Inst().LoadAsset<T>(name, path) ?? Resources.Load<T>(Path.Combine(path, name));
        }
        
        public static Object Load(string path, string name) {
            return ABManager.Inst().LoadAsset<Object>(name, path) ?? Resources.Load(Path.Combine(path, name));
        }

        public static T[] LoadAll<T>(string path, string name) where T : Object {
            return Resources.LoadAll<T>(Path.Combine(path, name));
        }
        
        public static Object[] LoadAll(string path, string name) {
            return Resources.LoadAll(Path.Combine(path, name));
        }
    }
}