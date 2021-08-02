using System;
using System.Collections.Generic;
using System.Linq;
using Data.Char;
using Data.Item;
using UnityEngine;

namespace Data.Player {
    [Serializable]
    [CreateAssetMenu(fileName = "PlayerData",menuName = "ArkNight/PlayerData")]
    public class PlayerData : ScriptableObject {
        
        // 名字
        [SerializeField]
        private new string name;
        
        // 密码
        [SerializeField]
        private string password;
        
        // 等级
        [SerializeField]
        private int level;

        // 经验
        [SerializeField]
        private int exp;

        // 理智
        [SerializeField]
        private int reason;

        // 角色数据 (预加载所有已知元数据)
        [SerializeField]
        private List<CharData> charList;

        // 队伍数据 (队伍名 + 角色类型)
        [SerializeField]
        private string[] squad;

        // 桌面角色
        [SerializeField]
        private string desktopChar;

        // 物品数据
        [SerializeField]
        private List<ItemStack> items;

        // 权限
        [SerializeField]
        private List<string> permissions;

        public string GetName() => name;
        public string GetPassword() => password;
        public int GetLevel() => level;
        public int GetExp() => exp;
        public int GetReason() => reason;
        public int GetMaxLevel() => 120;
        public int GetMaxExp() => GetMaxExp(level);
        public int GetMaxReason() => GetMaxReason(level);
        public List<CharData> GetCharList() => charList;

        public string[] GetSquad() {
            if (squad == null || squad.Length != 4) {
                squad = new string[4];
            }
            return squad;
        }
        public string GetDesktopChar() => desktopChar;
        public List<ItemStack> GetItems() => items;
        public List<string> GetPermissions() => permissions;
        
        public void SetLevel(int value) => level = value;
        public void SetExp(int value) => exp = value;
        public void SetReason(int value) => reason = value;
        public void ResetSquad() => squad = new string[4];

        public void Initialization(string name,string password) {
            this.name = name;
            this.password = password;
            reason = GetMaxReason();
            
            charList = new List<CharData> {
                new CharData("AMIYA")
            };
            ResetSquad();
            items = new List<ItemStack> {
                new ItemStack(0, 5),
                new ItemStack(1, 500),
                new ItemStack(2, 1000)
            };
            permissions = new List<string>();
        }
        
        // 最大经验值
        public static int GetMaxExp(int level) {
            if (level < 1) return 500;
            if (level < 2) return 800;
            if (level < 28) return 80 * level + 1000;
            if (level < 34) return 100 * level + 160;
            if (level < 50) return 300 * level - 6300;
            if (level < 65) return 500 * level - 16500;
            if (level < 101) return 1000 * level - 49000;
            if (level < 110) return 2000 * level - 150000;
            if (level < 120) return 3000 * level - 260000;
            return -1;
        }

        public static int GetMaxReason(int level) {
            if (level < 5) return 80 + 2 * level;
            if (level < 35) return 85 + level;
            if (level < 85) return 113 + level / 5;
            if (level < 100) return 130;
            if (level < 120) return (int) (105.75 + level / 4D);
            return -1;
        }

        public void ItemSort() {
            items.Sort((a,b) => a.GetId() == b.GetId() ? 0 : a.GetId() > b.GetId() ? 1 : -1);
        }
        
        public CharData GetCharData(string id) {
            return charList.FirstOrDefault(charData => charData.GetId() == id);
        }

        public ItemStack GetItemStack(int id) {
            return items.FirstOrDefault(itemStack => itemStack.GetId() == id);
        }

        public int GetItemAmount(int id) {
            ItemStack itemStack = GetItemStack(id);
            return itemStack?.GetAmount() ?? 0;
        }

        public void AddItem(ItemStack item) => AddItem(item.GetId(), item.GetAmount());

        public void AddItem(int id, int amount) {
            ItemStack itemStack = GetItemStack(id);
            if (itemStack != null) {
                itemStack.SetAmount(itemStack.GetAmount() + amount);
            } else {
                items.Add(new ItemStack(id, amount));
                ItemSort();
            }
        }

        public void TakeItem(ItemStack item) => TakeItem(item.GetId(), item.GetAmount());
        
        public void TakeItem(int id, int amount) {
            ItemStack itemStack = GetItemStack(id);
            if (itemStack != null && itemStack.GetAmount() >= amount) {
                itemStack.SetAmount(itemStack.GetAmount() - amount);
                if (itemStack.GetAmount() == 0) {
                    items.Remove(itemStack);
                }
            } else {
                throw new Exception("玩家没有该物品，扣款失败");
            }
        }

        public bool HasItem(ItemStack item) => HasItem(item.GetId(), item.GetAmount());

        public bool HasItem(int id, int amount) => GetItemAmount(id) >= amount;
    }
}