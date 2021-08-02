using System.Collections.Generic;
using System.Linq;
using Data.Char;
using Data.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class CharSelectUI : UIBase {

        private PlayerData data;
        private string initialSelect;
        private string currentSelect;
        private List<CharCard> list;

        private Transform prefab;
        private RectTransform leftPanel;
        private RectTransform upPanel;
        private RectTransform downPanel;
        private CanvasGroup infoGroup;
        private Text char_name;
        private Text char_level;
        private Text char_health;
        private Text char_atk;
        private Text char_def;
        private Text char_magic_resistance;
        private Text char_respawn_time;
        private Text char_cost;
        private Text char_block;
        private Text char_atk_speed;
        private Toggle sort_rarity;
        private Toggle sort_level;

        public override void Init() {
            list = new List<CharCard>();
            prefab = transform.Find("RightPanel/ItemList/CharPrefab");
            leftPanel = transform.Find("LeftPanel") as RectTransform;
            upPanel = transform.Find("UpPanel") as RectTransform;
            downPanel = transform.Find("DownPanel") as RectTransform;
            infoGroup = leftPanel.GetComponent<CanvasGroup>("INFO");
            char_name = leftPanel.GetComponent<Text>("INFO/CharName");
            char_level = leftPanel.GetComponent<Text>("INFO/CharLevel");
            char_health = leftPanel.GetComponent<Text>("INFO/Attribute/health/Value");
            char_atk = leftPanel.GetComponent<Text>("INFO/Attribute/atk/Value");
            char_def = leftPanel.GetComponent<Text>("INFO/Attribute/def/Value");
            char_magic_resistance = leftPanel.GetComponent<Text>("INFO/Attribute/magic_resistance/Value");
            char_respawn_time = leftPanel.GetComponent<Text>("INFO/Attribute/respawn_time/Value");
            char_cost = leftPanel.GetComponent<Text>("INFO/Attribute/cost/Value");
            char_block = leftPanel.GetComponent<Text>("INFO/Attribute/block/Value");
            char_atk_speed = leftPanel.GetComponent<Text>("INFO/Attribute/atk_speed/Value");
            sort_rarity = upPanel.GetComponent<Toggle>("Sort/tog_rarity");
            sort_level = upPanel.GetComponent<Toggle>("Sort/tog_level");

            // 提升等级(打开干员详情UI)
            leftPanel.GetComponent<Button>("CharInfoButton").onClick.AddListener(() => {
                if (data.GetCharData(currentSelect) != null) {
                    CharInfoUI.ShowSingle(currentSelect);
                }
            });

            // 确认替换(保存并关闭CharSelectUI、刷新SquadUI)
            downPanel.GetComponent<Button>("OKButton").onClick.AddListener(() => {
                // 防止连击
                if (state != UIState.Show) return;
                CharData charData = data.GetCharData(initialSelect);
                for (int i = 0; i < data.GetSquad().Length; i++) {
                    if (charData == null && !string.IsNullOrEmpty(data.GetSquad()[i])) continue;
                    if (charData != null && data.GetSquad()[i] != charData.GetId()) continue;
                    data.GetSquad()[i] = currentSelect;
                    Hide("CharSelectUI");
                    UIManager.Inst().Update("SquadUI");
                    break;
                }
            });
            // 排序
            sort_level.onValueChanged.AddListener(boo => {
                if (!boo) return;
                list.Sort(SortLevel);
                UpdateView();
            });
            sort_rarity.onValueChanged.AddListener(boo => {
                if (!boo) return;
                list.Sort(SortRarity);
                UpdateView();
            });
        }

        public int SortLevel(CharCard a, CharCard b) {
            if (a.transform.gameObject.activeSelf && b.transform.gameObject.activeSelf) {
                if (a.data.GetId() == currentSelect) return 1;
                if (b.data.GetId() == currentSelect) return -1;
                if (a.data.GetElite() == b.data.GetElite()) {
                    if (a.data.GetLevel() == b.data.GetLevel()) {
                        return a.data.GetExp().CompareTo(b.data.GetExp());
                    }
                    return a.data.GetLevel().CompareTo(b.data.GetLevel());
                }
                return a.data.GetElite().CompareTo(b.data.GetElite());
            }
            return a.transform.gameObject.activeSelf.CompareTo(!b.transform.gameObject.activeSelf);
        }

        public int SortRarity(CharCard a, CharCard b) {
            if (a.transform.gameObject.activeSelf && b.transform.gameObject.activeSelf) {
                if (a.data.GetId() == currentSelect) return 1;
                if (b.data.GetId() == currentSelect) return -1;
                return a.data.GetCharMeta().GetRarity().CompareTo(b.data.GetCharMeta().GetRarity());
            }
            return a.transform.gameObject.activeSelf.CompareTo(!b.transform.gameObject.activeSelf);
        }

        public override void UpdateView() {
            foreach (CharCard charCard in list) {
                charCard.transform.SetAsFirstSibling();
                charCard.select.gameObject.SetActive(charCard.transform.gameObject.activeSelf && charCard.data.GetId() == currentSelect);
            }
            infoGroup.DOFade(0.3f, 0.1f).OnComplete(() => {
                CharData charData = data.GetCharData(currentSelect);
                if (charData != null) {
                    char_name.text = charData.GetCharMeta().GetChineseName();
                    char_level.text = $"<color=#008BB9>{charData.GetLevel()}</color><size=80>/{charData.GetMaxLevel()}</size>";
                    char_health.text = charData.GetAttribute().GetMaxHealth().ToString();
                    char_atk.text = charData.GetAttribute().GetAtk().ToString();
                    char_def.text = charData.GetAttribute().GetDef().ToString();
                    char_magic_resistance.text = charData.GetAttribute().GetMagicResistance().ToString();
                    char_respawn_time.text = charData.GetAttribute().GetRespawnTime().ToString();
                    char_cost.text = charData.GetAttribute().GetCost().ToString();
                    char_block.text = charData.GetAttribute().GetBlock().ToString();
                    char_atk_speed.text = charData.GetAttribute().GetAtkSpeed().ToString();
                } else {
                    char_name.text = "--";
                    char_level.text = "<color=#008BB9>-</color>/-";
                    char_health.text = "--";
                    char_atk.text = "--";
                    char_def.text = "--";
                    char_magic_resistance.text = "--";
                    char_respawn_time.text = "--";
                    char_cost.text = "--";
                    char_block.text = "--";
                    char_atk_speed.text = "--";
                }
                infoGroup.DOFade(1f, 0.1f);
            });
        }

        public static void Show(string select) {
            CharSelectUI ui = UIManager.Inst().Show("CharSelectUI") as CharSelectUI;
            ui.data = PlayerManager.Inst().Get();
            ui.initialSelect = select;
            ui.currentSelect = select;
            List<CharData> dataList = new List<CharData>(ui.data.GetCharList()).FindAll(d => !ui.data.GetSquad().Contains(d.GetId()) || d.GetId() == select);
            for (int i = 0; i < dataList.Count || i < ui.list.Count; i++) {
                if (i < dataList.Count) {
                    if (ui.list.Count <= i) {
                        ui.list.Add(new CharCard(ui, Instantiate(ui.prefab, ui.prefab.parent)));
                    }
                    CharCard card = ui.list[i];
                    card.SetData(dataList[i]);
                }
                ui.list[i].transform.gameObject.SetActive(i < dataList.Count);
            }
            if (ui.sort_level.isOn) {
                ui.list.Sort(ui.SortLevel);
            } else {
                ui.list.Sort(ui.SortRarity);
            }
            ui.UpdateView();
        }

        public override void Show() {
            base.Show();
            leftPanel.anchoredPosition = new Vector2(-300, 0);
            leftPanel.DOAnchorPosX(0, 0.4f);
            upPanel.anchoredPosition = new Vector2(0, 120);
            upPanel.DOAnchorPosY(0, 0.3f).SetDelay(0.1f);
            downPanel.anchoredPosition = new Vector2(0, -120);
            downPanel.DOAnchorPosY(0, 0.3f).SetDelay(0.1f);
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.3f);
            
            
        }

        public override void Hide(bool destroy = false) {
            leftPanel.DOAnchorPosX(-300, 0.4f);
            upPanel.DOAnchorPosY(120, 0.3f).SetDelay(0.1f);
            downPanel.DOAnchorPosY(-120, 0.3f).SetDelay(0.1f);
            canvasGroup.DOFade(0, 0.3f).SetDelay(0.1f).OnComplete(() => { base.Hide(destroy); });
        }

        public class CharCard {
            private CharSelectUI ui;
            internal Transform transform;
            internal Transform select;
            internal CharData data;

            private Text name;
            private Text level;
            private Image expPercentage;
            private Image charCard;
            private Image rarity1;
            private Image rarity2;
            private Image rarity3;
            private Image rarity4;
            private Image rarityStar;
            private Image profession;
            private Image eliteImage;

            public CharCard(CharSelectUI ui, Transform transform) {
                this.ui = ui;
                this.transform = transform;
                transform.gameObject.SetActive(true);
                select = transform.Find("img_select");
                name = transform.GetComponent<Text>("txt_name");
                level = transform.GetComponent<Text>("txt_level");
                expPercentage = transform.GetComponent<Image>("img_exp_ground/img_exp_percentage");
                charCard = transform.GetComponent<Image>("img_char_card");
                rarity1 = transform.GetComponent<Image>("img_rarity_1");
                rarity2 = transform.GetComponent<Image>("img_rarity_2");
                rarity3 = transform.GetComponent<Image>("img_rarity_3");
                rarity4 = transform.GetComponent<Image>("img_rarity_4");
                rarityStar = transform.GetComponent<Image>("img_rarity_star");
                profession = transform.GetComponent<Image>("img_profession");
                eliteImage = transform.GetComponent<Image>("img_elite");
                transform.GetComponent<Button>().onClick.AddListener(OnClick);
            }

            public void SetData(CharData data) {
                transform.gameObject.SetActive(data != null);
                if (data != null && this.data != data) {
                    CharMeta charMeta = data.GetCharMeta();
                    int rarity = charMeta.GetRarity();
                    name.text = charMeta.GetChineseName();
                    charCard.sprite = charMeta.GetCharImage();
                    rarity1.sprite = CharManager.Inst().GetCardGroundImage(rarity + "_1");
                    rarity2.sprite = CharManager.Inst().GetCardGroundImage(rarity + "_2");
                    rarity3.sprite = CharManager.Inst().GetCardGroundImage(rarity + "_3");
                    rarity4.sprite = CharManager.Inst().GetCardGroundImage(rarity + "_4");
                    rarityStar.sprite = CharManager.Inst().GetCardGroundImage(rarity + "_star");
                    profession.sprite = CharManager.Inst().GetProfessionSmallImage(charMeta.GetProfession());
                }
                this.data = data;
                if (data != null) Update();
            }

            public void Update() {
                level.text = data.GetLevel().ToString();
                expPercentage.fillAmount = data.GetExp() / (float) data.GetMaxExp();
                eliteImage.gameObject.SetActive(data.GetElite() > 0);
                if (data.GetElite() > 0) {
                    eliteImage.sprite = CharManager.Inst().GetCharEliteImage("card_" + data.GetElite());
                }
            }

            private void OnClick() {
                ui.currentSelect = ui.currentSelect == data.GetId() ? null : data.GetId();
                ui.UpdateView();
            }
        }
    }
}