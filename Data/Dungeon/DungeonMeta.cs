using System;
using Data.Item;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Dungeon {
    [Serializable]
    [CreateAssetMenu(fileName = "DungeonID", menuName = "ArkNight/DungeonMeta")]
    public class DungeonMeta : ScriptableObject {
        // 关卡ID
        private string id;
        

        [Header("关卡名"), SerializeField]
        private string name;

        [Header("预制体"), SerializeField]
        private GameObject dungeonPrefab;

        [Header("所需理智"), SerializeField]
        private int reason;

        [Header("道具奖励"), SerializeField]
        private ItemStack[] reward;

        [Header("关卡描述"), SerializeField]
        private string description;

        public string GetId() => id;
        public string GetName() => name;
        public GameObject GetDungeonPrefab() => dungeonPrefab;
        public int GetReason() => reason;
        public ItemStack[] GetReward() => reward;
        public string GetDescription() => description;
        
        internal string SetId(string value) => id = value;
    }
}