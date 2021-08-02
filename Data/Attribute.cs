using System;
using UnityEngine;

namespace Data {

    [Serializable]
    public class Attribute {
        [SerializeField]
        protected float maxHealth;
        [SerializeField]
        protected float atk;
        // 防御力
        [SerializeField]
        protected float def;
        // 魔法抗性
        [SerializeField]
        protected float magicResistance;
        // 攻击速度
        [SerializeField]
        protected float atkSpeed;
        
        public float GetMaxHealth() => maxHealth;
        public float GetAtk() => atk;
        public float GetDef() => def;
        public float GetMagicResistance() => magicResistance;
        public float GetAtkSpeed() => atkSpeed;
        public void SetMaxHealth(float value) => maxHealth = value;
        public void SetAtk(float value) => atk = value;
        public void SetDef(float value) => def = value;
        public void SetMagicResistance(float value) => magicResistance = value;
        public void SetAtkSpeed(float value) => atkSpeed = value;
    }

    [Serializable]
    public class MonsterAttribute : Attribute {
        [SerializeField]
        private float moveSpeed;
        
        public float GetMoveSpeed() => moveSpeed;
        public void SetMoveSpeed(float value) => moveSpeed = value;
    }
    
    // 与怪物存在共用问题
    [Serializable]
    public class CharAttribute : Attribute {

        // 再部署时间
        [SerializeField]
        private int respawnTime;

        // 部署费用
        [SerializeField]
        private int cost;

        // 阻挡数
        [SerializeField]
        private int block;

        // 攻击间隔测试代码
        public static double AttackInterval(float atkSpeed) {
            return 1 / (Mathf.Clamp(atkSpeed,10,600) / 100);
        }

        public int GetRespawnTime() => respawnTime;
        public int GetCost() => cost;
        public int GetBlock() => block;
     
        public void SetRespawnTime(int value) => respawnTime = value;
        public void SetCost(int value) => cost = value;
        public void SetBlock(int value) => block = value;
        
        
        public static CharAttribute operator +(CharAttribute a, CharAttribute b) {
            return new CharAttribute {
                maxHealth = a.maxHealth + b.maxHealth,
                atk = a.atk + b.atk,
                def = a.def + b.def,
                magicResistance = a.magicResistance + b.magicResistance,
                respawnTime = a.respawnTime + b.respawnTime,
                cost = a.cost + b.cost,
                block = a.block + b.block,
                atkSpeed = a.atkSpeed + b.atkSpeed
            };
        }
        
        public static CharAttribute operator -(CharAttribute a, CharAttribute b) {
            return new CharAttribute {
                maxHealth = a.maxHealth - b.maxHealth,
                atk = a.atk - b.atk,
                def = a.def - b.def,
                magicResistance = a.magicResistance - b.magicResistance,
                respawnTime = a.respawnTime - b.respawnTime,
                cost = a.cost - b.cost,
                block = a.block - b.block,
                atkSpeed = a.atkSpeed - b.atkSpeed
            };
        }
        
        public static CharAttribute operator *(CharAttribute a, int amount) {
            return new CharAttribute {
                maxHealth = a.maxHealth * amount,
                atk = a.atk * amount,
                def = a.def * amount,
                magicResistance = a.magicResistance * amount,
                respawnTime = a.respawnTime * amount,
                cost = a.cost * amount,
                block = a.block * amount,
                atkSpeed = a.atkSpeed * amount
            };
        }
    }
}