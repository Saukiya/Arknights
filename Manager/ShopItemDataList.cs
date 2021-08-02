using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manager {
    [Serializable]
    [CreateAssetMenu(fileName = "ShopItemDataList", menuName = "ArkNight/ShopItemDataList")]
    public class ShopItemDataList : ScriptableObject {
        public List<ShopItemData> list = new List<ShopItemData>();
    }
    //
}