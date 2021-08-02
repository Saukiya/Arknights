using System;
using Object = UnityEngine.Object;

namespace Tools {
    [Serializable]
    public class Injection {
        public string name;
        public Object value;
    }
}