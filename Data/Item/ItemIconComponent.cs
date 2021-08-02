using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Data.Item {
    public class ItemIconComponent : MonoBehaviour {

        public Image ground;
        public Image icon;
        public Text amount;
        
        private ItemMeta meta;

        public ItemMeta GetMeta() => meta;
        
        public void SetMeta(ItemMeta value) {
            meta = value;
            ground.sprite = ItemManager.Inst().GetItemGround(meta.GetRarity());
            icon.sprite = meta.GetIcon();
        }

        public void SetAmount(int value) {
            if (amount) {
                if (value >= 10000) {
                    amount.text = $"{value / 10000f:0.#}万";
                } else {
                    amount.text = $"{value}";
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(amount.transform.parent as RectTransform);
            }
        }

        public void AddListener(UnityAction method) {
            GetComponent<Button>().onClick.AddListener(method);
        }

        public void RemoveAllListeners() {
            GetComponent<Button>().onClick.RemoveAllListeners();
        }
    }
}