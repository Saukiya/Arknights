using System;
using UnityEngine;

namespace Data.Item {
    // 物品堆
    [Serializable]
    public class ItemStack {
        // 物品ID
        [SerializeField]
        private int id;
        // 物品数量
        [SerializeField]
        private int amount;

        private ItemMeta meta;
        
        public ItemStack(int id,int amount) {
            this.id = id;
            this.amount = amount;
        }

        public int GetId() => id;
        public int GetAmount() => amount;
        public ItemMeta GetItemMeta() => meta ? meta : meta = ItemManager.Inst().GetMeta(id);

        public void SetAmount(int amount) => this.amount = amount;

        public ItemStack Clone() {
            return new ItemStack(id, amount);
        }
    }
}