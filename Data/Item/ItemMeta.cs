using System;
using UnityEngine;

namespace Data.Item {
    // 物品元数据
    [Serializable]
    [CreateAssetMenu(fileName = "ItemID", menuName = "ArkNight/ItemMeta")]
    public class ItemMeta : ScriptableObject {
        // ID
        [Header("数字ID"),SerializeField]
        private int id;
        // 名字
        [Header("名字"),SerializeField]
        private new string name;
        // 图标
        [Header("图标"),SerializeField]
        private Sprite icon;    
        // 稀有度
        [Header("稀有度"),Range(1,6),SerializeField]
        private int rarity;
        // 类型
        [Header("类型"),SerializeField]
        private ItemType type;
        // 用途
        [Header("用途"),SerializeField]
        private string useInfo;
        // 描述
        [Header("描述"),SerializeField]
        private string description;
        // 获得方式
        [Header("获得方式"),SerializeField]
        private string waysObtain;

        public int GetID() => id;

        public string GetName() => name;

        public Sprite GetIcon() => icon;

        public int GetRarity() => rarity;

        public ItemType GetItemType() => type;

        public string GetUseInfo() => useInfo;

        public string GetDescription() => description;

        public string GetWaysObtain() => waysObtain;
    }
}