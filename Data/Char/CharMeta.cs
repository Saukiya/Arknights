using System;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Char {
    // 角色元数据
    [Serializable]
    [CreateAssetMenu(fileName = "CharID", menuName = "ArkNight/CharMeta")]
    public class CharMeta : ScriptableObject {
        // 角色类型枚举
        private string id;

        [Header("中文名"),SerializeField]
        private string chineseName;

        [Header("英文名"),SerializeField]
        private string englishName;

        [Header("稀有度"),Range(1,6),SerializeField]
        private int rarity;

        [Header("职业"),SerializeField]
        private CharProfession profession;

        [Header("位置"),SerializeField]
        private CharPosition charPosition;
        
        [Header("阵营"),SerializeField]
        private CharCamp camp;

        [Header("标签"),SerializeField]
        private CharTag[] tags;

        [Header("精英属性[0/1/2]"),SerializeField]
        private CharAttribute[] eliteAttributes;

        [Header("等级属性[0/1/2]"),SerializeField]
        private CharAttribute[] levelAttributes;

        [Header("攻击范围"),SerializeField]
        private string[] atkRange;
        
        private List<Vector3> attackRange;

        [Header("天赋名"),SerializeField]
        private string[] talent;

        [Header("特性"),SerializeField]
        private string feature;

        [Header("立绘"),SerializeField]
        private Sprite image1;
        
        [Header("卡贴"),SerializeField]
        private Sprite image2;
        
        [Header("头像"),SerializeField]
        private Sprite image3;

        [Header("骨骼"), SerializeField]
        private SkeletonDataAsset skeleton;

        
        public string GetId() => id;
        public string GetChineseName() => chineseName;
        public string GetEnglishName() => englishName;
        public int GetRarity() => rarity;
        public CharProfession GetProfession() => profession;
        public CharPosition GetPosition() => charPosition;
        public CharCamp getCamp() => camp;
        public CharTag[] GetTags() => tags;
        public CharAttribute[] GetEliteAttributes() => eliteAttributes;
        public CharAttribute[] GetLevelAttributes() => levelAttributes;
        public List<Vector3> GetAttackRange() => attackRange;
        public string[] GetTalent() => talent;
        public string GetFeature() => feature;
        public Sprite GetImage() => image1;
        public Sprite GetCharImage() => image2;
        public Sprite GetAvatar() => image3;
        public SkeletonDataAsset GetSkeleton() => skeleton;
        
        internal string SetId(string value) => id = value;
        

        public void OnEnable() {
            //计算攻击范围
            //缺点：没有中心点 比如 oooo /n oxoo /n oooo x为中心点
            //优点：够用
            attackRange = new List<Vector3>();
            for (int x = 0; x < atkRange.Length; x++) {
                string str = atkRange[x];
                for (int z = 0; z < str.Length; z++) {
                    if (str[z] != 'o') continue;
                    attackRange.Add(new Vector3(x, 0, z));
                    if (x != 0) attackRange.Add(new Vector3(-x, 0, z));
                }
            }
        }

        /**
         * 获得最大精英值
         * rarity - 稀有度
         */
        public static int GetMaxElite(int rarity) {
            switch (rarity) {
                case 1:
                case 2:
                    return 0;
                case 3:
                    return 1;
                case 4:
                case 5:
                case 6:
                    return 2;
                default:
                    return -1;
            }
        }

        /**
         * 获得最大等级
         * rarity - 稀有度
         * elite - 当前精英阶级
         */
        public static int GetMaxLevel(int rarity,int elite) {
            switch (rarity) {
                case 1:
                case 2:
                    if (elite == 0) return 30;
                    break;
                case 3:
                    if (elite == 0) return 40;
                    if (elite == 1) return 55;
                    break;
                case 4:
                    return elite == 0 ? 45 : elite == 1 ? 60 : 70;
                case 5:
                    return elite == 0 ? 50 : elite == 1 ? 70 : 80;
                case 6:
                    return elite == 0 ? 50 : elite == 1 ? 80 : 90;
            }
            return -1;
        }

        /**
         * 获得最大经验值
         * elite - 当前精英阶级
         * level - 当前等级
         */
        public static int GetMaxExp(int elite,int level) {
            switch (elite) {
                case 0:
                    return 100 + level * 20;
                case 1:
                    return 120 + level * 60;
                case 2:
                    return 120 + level * 120;
            }
            return -1;
        }
    }
}