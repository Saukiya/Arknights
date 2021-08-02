using System;
using System.Collections.Generic;
using Data.Item;
using Tools;
using UnityEngine;

namespace Manager {
    public class ShopManager : Single<ShopManager> {
        private List<ShopItemData> list = Asset.Load<ShopItemDataList>("Data/Shop", "ShopItemDataList").list;

        public List<ShopItemData> GetShopItems(int priceID) {
            return list.FindAll(i => i.GetPrice().GetId() == priceID);
        }
    }

    [Serializable]
    public class ShopItemData {
        [SerializeField]
        private ItemStack sellItem;
        [SerializeField]
        private ItemStack priceItem;

        public ItemStack GetSell() => sellItem;
        public ItemStack GetPrice() => priceItem;
    }
}