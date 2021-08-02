using System;
using Spine.Unity;
using UnityEngine;

namespace Data.Monster {
    [Serializable]
    [CreateAssetMenu(fileName = "MonsterName", menuName = "ArkNight/MonsterMeta")]
    public class MonsterData : ScriptableObject {
        private string id;

        [Header("属性"), SerializeField]
        internal MonsterAttribute attribute;

        [Header("骨骼"), SerializeField]
        internal SkeletonDataAsset skeleton;

        [Header("动画名翻译"), SerializeField]
        internal AnimationsName[] animationsNames;

        internal string this[string value] {
            get {
                if (animationsNames != null) {
                    foreach (AnimationsName animationsName in animationsNames) {
                        if (animationsName.name == value) {
                            return animationsName.target;
                        }
                    }
                }
                return value;
            }
        }
        
        public string GetId() => id;
        internal string SetId(string value) => id = value;
        
        [Serializable]
        public class AnimationsName {
            [SerializeField]
            internal string name;
            [SerializeField]
            internal string target;
        }
    }
}