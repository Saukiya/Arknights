using System.Collections.Generic;
using Settings;
using Tools;
using UnityEngine;

namespace Data.Item {
    public class ItemManager : Single<ItemManager> {
        private Dictionary<int, ItemMeta> metaDictionary = new Dictionary<int,ItemMeta>();
        private Dictionary<int, Sprite> itemGround = new Dictionary<int,Sprite>();

        public ItemManager() {
            // ItemGround
            Sprite[] sprites = Asset.LoadAll<Sprite>("Sprite/Item","ItemGround");
            foreach (Sprite sprite in sprites) {
                if (!sprite.name.StartsWith("item_ground_")) continue;
                itemGround.Add(int.Parse(sprite.name.Substring(12)), sprite);
            }
        }

        public Sprite GetItemGround(int i) {
            if (i > 0 && i < 7) {
                return itemGround[i];
            }
            return null;
        }

        // 获取对象
        public ItemMeta GetMeta(int id) {
            return metaDictionary.ContainsKey(id) ? metaDictionary[id] : LoadMeta(id);
        }

        private ItemMeta LoadMeta(int id) {
            ItemMeta meta = Asset.Load<ItemMeta>(GameSettings.ITEM_META_PATH , id.ToString());
            metaDictionary.Add(id, meta);
            return meta;
        }
    }
}