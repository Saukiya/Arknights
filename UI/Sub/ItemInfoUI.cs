using Data.Item;
using Data.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class ItemInfoUI : UIBase {
        public Image blur;

        [Header("图标")]
        public Image icon;

        [Header("名字")]
        public Text itemName;

        [Header("用途")]
        public Text useInfo;

        [Header("描述")]
        public Text description;

        [Header("获得方式")]
        public Text waysObtain;

        [Header("库存")]
        public Text amount;

        private ItemStack item;

        public static void Show(ItemStack item) {
            ItemInfoUI ui = UIManager.Inst().Show("ItemInfoUI") as ItemInfoUI;
            ui.item = item;
            ItemMeta meta = item.GetItemMeta();
            ui.icon.sprite = meta.GetIcon();
            ui.itemName.text = meta.GetName();
            ui.useInfo.text = meta.GetUseInfo();
            ui.description.text = meta.GetDescription();
            ui.waysObtain.text = meta.GetWaysObtain();
            ui.amount.text = item.GetAmount().ToString();

            LayoutRebuilder.ForceRebuildLayoutImmediate(ui.itemName.transform.parent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(ui.useInfo.transform.parent as RectTransform);
        }

        public override void Show() {
            base.Show();
            Material blurryShader = blur.material;
            blurryShader.SetFloat("_Size",0);
            float value = 0;
            DOTween.To(() => value,v => value = v,160,0.5f).OnUpdate(() => blurryShader.SetFloat("_Size",value));
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1,0.3f);
        }

        public override void Hide(bool destroy = false) {
            Material blurryShader = blur.material;
            float value = blurryShader.GetFloat("_Size");
            DOTween.To(() => value,v => value = v,0,0.2f).OnUpdate(() => blurryShader.SetFloat("_Size",value));
            canvasGroup.DOFade(0,0.2f).OnComplete(() => { base.Hide(destroy); });
        }
    }
}