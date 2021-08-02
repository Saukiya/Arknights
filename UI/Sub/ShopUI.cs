using System;
using System.Collections.Generic;
using Data.Item;
using Data.Player;
using DG.Tweening;
using Manager;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class ShopUI : UIBase {
        public Sprite zzpz_icon;
        public Sprite gjpz_icon;
        public Sprite cgpz_icon;
        public Transform shopItemPrefab;

        private Text cur_zzpz;
        private Text cur_gjpz;
        private Text cur_cgpz;
        private Text cur_lmb;
        private Text cur_ys;
        
        private AudioClip clip;
        private AudioClip loop_clip;
        
        private PlayerData data;
        private ShopItemPanel shopItemPanel;
        private List<ShopItem> list = new List<ShopItem>();

        public override void Init() {
            shopItemPanel = new ShopItemPanel(this, transform.Find("ShopItemPanel"));
            shopItemPanel.transform.gameObject.SetActive(false);
            cur_zzpz = transform.GetComponent<Text>("CurrencyPanel/ZZPZ/ValueGround/txt_value");
            cur_gjpz = transform.GetComponent<Text>("CurrencyPanel/GJPZ/ValueGround/txt_value");
            cur_cgpz = transform.GetComponent<Text>("CurrencyPanel/CGPZ/ValueGround/txt_value");
            cur_lmb = transform.GetComponent<Text>("CurrencyPanel/LMB/ValueGround/txt_value");
            cur_ys = transform.GetComponent<Text>("CurrencyPanel/YS/ValueGround/txt_value");
            clip = Asset.Load<AudioClip>("Audio/Music/Shop", "m_sys_shop_intro");
            loop_clip = Asset.Load<AudioClip>("Audio/Music/Shop", "m_sys_shop_loop");
        }

        public void GiveYs(int size) {
            data.AddItem(0, size);
            UpdateView();
        }

        public override void Show() {
            base.Show();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.3f);
            SoundManager.Inst().PlayMusic(clip, false, () => {
                SoundManager.Inst().PlayMusic(loop_clip, true);
            });
        }

        public override void Hide(bool destroy = false) {
            canvasGroup.DOFade(0, 0.2f).OnComplete(() => { 
                base.Hide(destroy);
            });
            UIManager.Inst().Show("HomeUI");
        }

        public override void UpdateView() {
            data = PlayerManager.Inst().Get();
            cur_zzpz.text = data.GetItemAmount(6).ToString();
            cur_gjpz.text = data.GetItemAmount(7).ToString();
            cur_cgpz.text = data.GetItemAmount(8).ToString();
            cur_lmb.text = data.GetItemAmount(2).ToString();
            cur_ys.text = data.GetItemAmount(0).ToString();
        }

        public void ShowPriceItem(int id) {
            List<ShopItemData> dataList = ShopManager.Inst().GetShopItems(id);
            for (int i = 0; i < dataList.Count || i < list.Count; i++) {
                if (i < dataList.Count) {
                    if (list.Count <= i) {
                        list.Add(new ShopItem(this,Instantiate(shopItemPrefab, shopItemPrefab.parent)));
                    }
                    list[i].SetShopData(dataList[i]);
                    list[i].transform.gameObject.SetActive(true);
                } else {
                    list[i].transform.gameObject.SetActive(false);
                }
            }
        }
        
        public Sprite GetPriceIcon(int id) {
            switch (id) {
                case 6:
                    return zzpz_icon;
                case 7:
                    return gjpz_icon;
                case 8:
                    return cgpz_icon;
            }
            return null;
        }
        
        public class ShopItem {
            internal Transform transform;
            private ShopUI ui;
            
            private ItemIconComponent iic;
            private Text txt_name;
            private Image img_icon;
            private Text txt_price;

            public ShopItem(ShopUI ui, Transform transform) {
                this.transform = transform;
                this.ui = ui;
                iic = transform.GetComponent<ItemIconComponent>();
                txt_name = transform.GetComponent<Text>("Name_Ground/Text");
                img_icon = transform.GetComponent<Image>("Price_Ground/Layout/Price_Icon");
                txt_price = transform.GetComponent<Text>("Price_Ground/Layout/Price");
            }

            public void SetShopData(ShopItemData data) {
                iic.SetMeta(data.GetSell().GetItemMeta());
                iic.SetAmount(data.GetSell().GetAmount());
                txt_name.text = data.GetSell().GetItemMeta().GetName();
                img_icon.sprite = ui.GetPriceIcon(data.GetPrice().GetId());
                txt_price.text = data.GetPrice().GetAmount().ToString();
                iic.RemoveAllListeners();
                iic.AddListener(() => {
                    ui.shopItemPanel.SetShopData(data);
                    ui.shopItemPanel.Show();
                });
            }
        }
        
        public class ShopItemPanel {
            internal Transform transform;
            private CanvasGroup canvasGroup;
            private Material blurryShader;
            private ShopUI ui;
            private ShopItemData data;
            
            private Text txt_sell_name;
            private Image img_sell_use_image;
            private Text txt_sell_use_info;
            private Text txt_sell_amount;
            private Image img_price_icon;
            private Text txt_price_amount;
            private Text txt_buy_amount;
            private Image img_all_price_icon;
            private Text txt_all_price_amount;

            private int buy_amount;
            private int all_price_amount;

            private void SetBuyAmount(int value) {
                buy_amount = value;
                txt_buy_amount.text = value.ToString();
                all_price_amount = data.GetPrice().GetAmount() * value;
                txt_all_price_amount.text = all_price_amount.ToString();
            }

            public ShopItemPanel(ShopUI ui, Transform transform) {
                this.transform = transform;
                this.ui = ui;
                canvasGroup = transform.GetComponent<CanvasGroup>();
                blurryShader = transform.GetComponent<Image>().material;
                txt_sell_name = transform.GetComponent<Text>("ItemNameGround/Text");
                img_sell_use_image = transform.GetComponent<Image>("ItemIconGround/Image");
                txt_sell_use_info = transform.GetComponent<Text>("ItemUseInfo");
                txt_sell_amount = transform.GetComponent<Text>("ItemAmountGround/ItemAmount");
                img_price_icon = transform.GetComponent<Image>("PriceIcon");
                txt_price_amount = transform.GetComponent<Text>("PriceAmount");
                txt_buy_amount = transform.GetComponent<Text>("BuyAmountGround/Text");
                img_all_price_icon = transform.GetComponent<Image>("AllPriceIcon");
                txt_all_price_amount = transform.GetComponent<Text>("AllPriceAmount");
                transform.GetComponent<Button>("CloseButton").onClick.AddListener(Hide);// 关闭界面事件
                
                transform.GetComponent<Button>("AddButton").onClick.AddListener(() => {
                    // 增加数量事件
                    if (ui.data.HasItem(data.GetPrice().GetId(), data.GetPrice().GetAmount() * (buy_amount + 1))) {
                        SetBuyAmount(Math.Min(buy_amount + 1, 99));
                    }
                });
                transform.GetComponent<Button>("TakeButton").onClick.AddListener(() => {
                    // 减少数量事件
                    SetBuyAmount(Math.Max(buy_amount - 1, 0));
                });
                transform.GetComponent<Button>("MinButton").onClick.AddListener(() => {
                    // 最少数量事件
                    SetBuyAmount(ui.data.HasItem(data.GetPrice()) ? 1 : 0);
                });
                transform.GetComponent<Button>("MaxButton").onClick.AddListener(() => {
                    // 最多数量事件
                    SetBuyAmount(Math.Min(ui.data.GetItemAmount(data.GetPrice().GetId()) / data.GetPrice().GetAmount(), 99));
                });
                transform.GetComponent<Button>("BuyButton").onClick.AddListener(() => {
                    // 购买事件
                    if (buy_amount == 0) return;
                    ui.data.AddItem(data.GetSell().GetId(), data.GetSell().GetAmount() * buy_amount);
                    ui.data.TakeItem(data.GetPrice().GetId(), all_price_amount);
                    CommonDialogUI.Message(CommonDialogUI.GroundType.WHITE, "购买成功: " + data.GetSell().GetItemMeta().GetName() + " x " + (data.GetSell().GetAmount() * buy_amount)).AddBackListener(Hide);
                    ui.UpdateView();
                });
            }

            public void SetShopData(ShopItemData data) {
                this.data = data;
                ItemMeta sellMeta = data.GetSell().GetItemMeta();
                txt_sell_name.text = sellMeta.GetName();
                img_sell_use_image.sprite = sellMeta.GetIcon();
                txt_sell_use_info.text = sellMeta.GetUseInfo();
                txt_sell_amount.text = "x " + data.GetSell().GetAmount();
                img_price_icon.sprite = ui.GetPriceIcon(data.GetPrice().GetId());
                txt_price_amount.text = data.GetPrice().GetAmount().ToString();
                SetBuyAmount(0);
                img_all_price_icon.sprite = ui.GetPriceIcon(data.GetPrice().GetId());
                
            }

            public void Show() {
                blurryShader.SetFloat("_Size",0);
                float value = 0;
                DOTween.To(() => value,v => value = v,160,0.5f).OnUpdate(() => blurryShader.SetFloat("_Size",value));
                transform.gameObject.SetActive(true);
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, 0.3f);
                LayoutRebuilder.ForceRebuildLayoutImmediate(txt_sell_name.transform.parent as RectTransform);
            }

            public void Hide() {
                float value = blurryShader.GetFloat("_Size");
                DOTween.To(() => value,v => value = v,0,0.2f).OnUpdate(() => blurryShader.SetFloat("_Size",value));
                canvasGroup.DOFade(0, 0.2f).OnComplete(() => {
                    transform.gameObject.SetActive(false);
                });
            }
        }
    }
}