using System.Collections.Generic;
using Data.Item;
using Data.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class HouseUI : UIBase {

        public Toggle ALL;
        public Toggle JI_CHU;
        public Toggle YANG_CHENG;
        public ItemIconComponent prefab;

        private List<ItemIconComponent> list = new List<ItemIconComponent>();
        private PlayerData data;
        
        public override void Init() {
            Text allText = ALL.transform.GetComponentInChildren<Text>();
            ALL.onValueChanged.AddListener(value => {
                if (value) ShowItem(ItemType.JI_CHU,ItemType.YANG_CHENG);
                allText.DOColor(value ? Color.white : Color.black, 0.2f);
            });
            Text ji_chuText = JI_CHU.transform.GetComponentInChildren<Text>();
            JI_CHU.onValueChanged.AddListener(value => {
                if (value) ShowItem(ItemType.JI_CHU);
                ji_chuText.DOColor(value ? Color.white : Color.black, 0.2f);
            });
            Text yang_chengText = YANG_CHENG.transform.GetComponentInChildren<Text>();
            YANG_CHENG.onValueChanged.AddListener(value => {
                if (value) ShowItem(ItemType.YANG_CHENG);
                yang_chengText.DOColor(value ? Color.white : Color.black, 0.2f);
            });
        }

        // 筛选物品
        private void ShowItem(params ItemType[] type) {
            List<ItemType> types = new List<ItemType>(type);
            foreach (ItemIconComponent itemIconComponent in list) {
                itemIconComponent.gameObject.SetActive(types.Contains(itemIconComponent.GetMeta().GetItemType()));
            }
        }

        public override void UpdateView() {
            data = PlayerManager.Inst().Get();
            List<ItemStack> dataList = data.GetItems();
            for (int i = 0; i < dataList.Count || i < list.Count; i++) {
                if (i < dataList.Count) {
                    ItemStack itemStack = dataList[i];
                    if (list.Count <= i) {
                        list.Add(Instantiate(prefab, prefab.transform.parent));
                    }
                    ItemIconComponent iic = list[i];
                    iic.SetMeta(itemStack.GetItemMeta());
                    iic.SetAmount(itemStack.GetAmount());
                    iic.gameObject.name = itemStack.GetId().ToString();
                    iic.RemoveAllListeners();
                    iic.AddListener(() => ItemInfoUI.Show(itemStack));
                }
                list[i].gameObject.SetActive(i < dataList.Count);
            }
            if (ALL.isOn) {
                ShowItem(ItemType.JI_CHU,ItemType.YANG_CHENG);
            } else {
                ShowItem(JI_CHU.isOn ? ItemType.JI_CHU : ItemType.YANG_CHENG);
            }
        }

        public override void Show() {
            base.Show();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1,0.3f);
        }

        public override void Hide(bool destroy = false) {
            canvasGroup.DOFade(0,0.2f).OnComplete(() => {
                base.Hide(destroy);
            });
        }
    }
}