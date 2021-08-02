using Data;
using Spine.Unity;
using UnityEngine;

namespace Scripts.Game {
    public abstract class Entity {
        internal Transform transform;
        internal Transform model;
        internal SkeletonAnimation sa;
        internal StateManager state;
        internal Attribute attribute;
        internal float health;
        internal int face = 1;

        public Entity(Transform transform, Attribute attribute) {
            this.transform = transform;
            this.attribute = attribute;
            health = attribute.GetMaxHealth();
            model = transform.Find("Model");
            sa = model.GetComponent<SkeletonAnimation>("Skeleton");
            state = new StateManager();
        }

        public virtual void Update() {
            state.Tick();
            model.localScale = new Vector3(1, 1, Mathf.Lerp(model.localScale.z, face, Time.deltaTime * 8f));
        }

        public float GetHealth() {
            return health;
        }

        public void SetHealth(float value) {
            health = value;
        }

        public bool isDead() {
            return health <= 0;
        }

        public void damage(float damage, DamageType type) {
            if (type == DamageType.Physical) {
                health -= Mathf.Max(damage * 0.05f, damage - attribute.GetDef()); //物理：攻击 - 防御
            } else {
                health -= Mathf.Max(damage * 0.05f, damage * 0.01f * (100 - attribute.GetMagicResistance())); //法术：
            }
        }

        public void SetFace(Vector3 vec) {
            if (vec.z > 0.5) {
                face = -1;
            } else if (vec.z < -0.5) {
                face = 1;
            }
        }
    }

    public enum DamageType {
        Physical,
        Magic
    }
}