using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Char;
using Data.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI.Sub {
    public class CharInfoUI : UIBase, IPointerDownHandler, IPointerUpHandler {
        public RectTransform prefab;
        public Transform professionHelp;

        private bool dragging;
        private float moveLocal = 0.5f;
        private string[] list;
        private PlayerData playerData;
        private ScrollRect scrollRect;
        private Transform[] prefabs = new Transform[3];
        private LinkedList<CharPrefab> charPrefabs = new LinkedList<CharPrefab>();

        public override void Init() {
            scrollRect = GetComponent<ScrollRect>();
            playerData = PlayerManager.Inst().Get();
            prefab.sizeDelta = UIManager.Inst().GetCanvasSize();
            for (int i = 0; i < 3; i++) {
                prefabs[i] = Instantiate(prefab, prefab.parent);
                prefabs[i].gameObject.name = "CharPrefabs_" + i;
                charPrefabs.AddLast(new CharPrefab(prefabs[i]));
            }
            prefab.gameObject.SetActive(false);

            CanvasGroup professionHelp_canvasGroup = professionHelp.GetComponent<CanvasGroup>();
            Button professionHelp_button = professionHelp.GetComponent<Button>();
            professionHelp_button.onClick.AddListener(() => { professionHelp_canvasGroup.DOFade(0, 0.3f).OnComplete(() => { professionHelp.gameObject.SetActive(false); }); });
            foreach (Transform prefab in prefabs) {
                prefab.GetComponent<Button>("CharInfoPanel/LowerLeftPanel/img_profession").onClick.AddListener(() => {
                    professionHelp.gameObject.SetActive(true);
                    professionHelp_canvasGroup.alpha = 0;
                    professionHelp_canvasGroup.DOFade(1, 0.3f).OnComplete(() => { professionHelp_button.enabled = true; });
                });
            }
        }

        public static void ShowSingle(string value) {
            CharInfoUI ui = UIManager.Inst().Show("CharInfoUI") as CharInfoUI;
            ui.list = new[] {value};
            ui.charPrefabs.ElementAt(0).SetData(ui.playerData.GetCharData(value));
            ui.charPrefabs.ElementAt(1).transform.gameObject.SetActive(false);
            ui.charPrefabs.ElementAt(2).transform.gameObject.SetActive(false);
        }
        
        public static void Show(string[] list, int index) {
            CharInfoUI ui = UIManager.Inst().Show("CharInfoUI") as CharInfoUI;
            ui.list = list;
            index = (index == 0 ? list.Length : index) - 1;
            for (int i = 0; i < 3; i++, index = Math.Abs(index + 1) % list.Length) {
                CharPrefab cp = ui.charPrefabs.ElementAt(i);
                cp.index = index;
                cp.SetData(ui.playerData.GetCharData(list[index]));
                cp.transform.gameObject.SetActive(true);
            }
            ui.scrollRect.enabled = list.Length != 1;
        }

        public override void Show() {
            base.Show();
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.3f);
        }

        public override void Hide(bool destroy = false) {
            canvasGroup.DOFade(0, 0.2f).OnComplete(() => { base.Hide(destroy); });
        }

        // 按下
        public void OnPointerDown(PointerEventData eventData) {
            dragging = true;
        }

        // 松开 获得向量 + 滑轮位置，计算需要去到的位置
        public void OnPointerUp(PointerEventData eventData) {
            dragging = false;
            float normalizedPosition = scrollRect.horizontalNormalizedPosition;
            moveLocal = normalizedPosition < 0.3 ? 0 : normalizedPosition > 0.7 ? 1 : 0.5f;
            if (moveLocal == 0.5) {
                if (scrollRect.velocity.x > 200) {
                    moveLocal = 0;
                } else if (scrollRect.velocity.x < -200) {
                    moveLocal = 1;
                }
            }
            scrollRect.velocity = Vector2.zero;
        }

        // 检测是否为松开，松开则向量插值到要去的位置，与位置距离0.2以内时修改CharPrefab层级+数据
        private void Update() {
            foreach (CharPrefab charPrefab in charPrefabs) {
                charPrefab.canvasGroup.alpha = Mathf.Clamp(1 - Mathf.Abs(charPrefab.transform.position.x) / 200, 0.2f, 1);
            }
            if (!dragging) {
                scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, moveLocal, Time.deltaTime * 8f);
                if (moveLocal != 0.5 && Mathf.Abs(moveLocal - scrollRect.horizontalNormalizedPosition) < 0.2) {
                    CharPrefab c;
                    if (moveLocal == 0) {
                        c = charPrefabs.Last.Value;
                        c.transform.SetAsFirstSibling();
                        c.index = (charPrefabs.First.Value.index == 0 ? list.Length : charPrefabs.First.Value.index) - 1;
                        charPrefabs.RemoveLast();
                        charPrefabs.AddFirst(c);
                        moveLocal = 0.5f;
                        scrollRect.horizontalNormalizedPosition = 0.5f + scrollRect.horizontalNormalizedPosition;
                    } else {
                        c = charPrefabs.First.Value;
                        c.transform.SetAsLastSibling();
                        c.index = Math.Abs(charPrefabs.Last.Value.index + 1) % list.Length;
                        charPrefabs.RemoveFirst();
                        charPrefabs.AddLast(c);
                        moveLocal = 0.5f;
                        scrollRect.horizontalNormalizedPosition = 0.5f - (1 - scrollRect.horizontalNormalizedPosition);
                    }
                    c.SetData(playerData.GetCharData(list[c.index]));
                }
            }
        }

        public class CharPrefab {//角色面板
            internal RectTransform transform;
            internal CanvasGroup canvasGroup;
            internal int index;
            internal CharData charData;

            private Text txt_health;
            private Text txt_atk;
            private Text txt_def;
            private Text txt_magic_resistance;
            private Text txt_respawn_time;
            private Text txt_cost;
            private Text txt_block;
            private Text txt_atk_speed;
            private Image img_health_percentage;
            private Image img_atk_percentage;
            private Image img_def_percentage;
            private Image img_magic_resistance_percentage;

            private Button btn_add_exp;
            private Text txt_level;
            private Text txt_max_level;
            private Text txt_exp;
            private Image img_exp_percentage;

            private Image img_elite;
            private Button btn_add_elite;

            private List<Transform> talents = new List<Transform>();

            public CharPrefab(Transform transform) {
                transform.gameObject.SetActive(true);
                this.transform = transform as RectTransform;
                canvasGroup = transform.GetComponent<CanvasGroup>();

                txt_health = transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/Health/txt_value");
                txt_atk = transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/Atk/txt_value");
                txt_def = transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/Def/txt_value");
                txt_magic_resistance = transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/MagicResistance/txt_value");
                txt_respawn_time = transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/RespawnTime/txt_value");
                txt_cost = transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/Cost/txt_value");
                txt_block = transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/Block/txt_value");
                txt_atk_speed = transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/AtkSpeed/txt_value");
                img_health_percentage = transform.GetComponent<Image>("CharInfoPanel/MiddleLeftPanel/Health/img_ground/img_value_percentage");
                img_atk_percentage = transform.GetComponent<Image>("CharInfoPanel/MiddleLeftPanel/Atk/img_ground/img_value_percentage");
                img_def_percentage = transform.GetComponent<Image>("CharInfoPanel/MiddleLeftPanel/Def/img_ground/img_value_percentage");
                img_magic_resistance_percentage = transform.GetComponent<Image>("CharInfoPanel/MiddleLeftPanel/MagicResistance/img_ground/img_value_percentage");

                btn_add_exp = transform.GetComponent<Button>("CharInfoPanel/RightPanel/Level");
                txt_level = transform.GetComponent<Text>("CharInfoPanel/RightPanel/Level/txt_level");
                txt_max_level = transform.GetComponent<Text>("CharInfoPanel/RightPanel/Level/txt_max_level");
                txt_exp = transform.GetComponent<Text>("CharInfoPanel/RightPanel/Level/Image/txt_exp");
                img_exp_percentage = transform.GetComponent<Image>("CharInfoPanel/RightPanel/Level/img_exp_ground/img_exp_percentage");

                img_elite = transform.GetComponent<Image>("CharInfoPanel/RightPanel/Elite/img_elite");
                btn_add_elite = transform.GetComponent<Button>("CharInfoPanel/RightPanel/Elite");
            }

            public void SetData(CharData data) {
                charData = data;
                init();
            }

            public void init() {
                CharMeta meta = charData.GetCharMeta();
                transform.GetComponent<Image>("img_char").sprite = meta.GetImage();
                transform.GetComponent<Image>("img_camp").sprite = CharManager.Inst().GetCampImage(meta.getCamp().ToString());

                transform.GetComponent<Text>("CharInfoPanel/MiddleLeftPanel/Trust/txt_trust_percentage").text = charData.GetTrust() + "%";
                transform.GetComponent<Image>("CharInfoPanel/MiddleLeftPanel/Trust/img_trust_percentage").fillAmount = charData.GetTrust() / 200f;
                transform.GetComponent<Image>("CharInfoPanel/LowerLeftPanel/img_rarity").sprite = CharManager.Inst().GetStarImage("info_" + meta.GetRarity());
                transform.GetComponent<Text>("CharInfoPanel/LowerLeftPanel/txt_english_name").text = meta.GetEnglishName();
                transform.GetComponent<Text>("CharInfoPanel/LowerLeftPanel/txt_chinese_name").text = meta.GetChineseName();
                transform.GetComponent<Image>("CharInfoPanel/LowerLeftPanel/img_profession").sprite = CharManager.Inst().GetProfessionImage(meta.GetProfession());
                transform.GetComponent<Text>("CharInfoPanel/LowerLeftPanel/img_tag_ground/txt_tag").text = meta.GetTags().Aggregate(" ", (current, charTag) => current + (charTag.GetName() + " "));
                transform.GetComponent<Text>("CharInfoPanel/LowerLeftPanel/img_position_ground/txt_position").text = meta.GetPosition().GetName();

                btn_add_exp.onClick.RemoveAllListeners();
                btn_add_exp.onClick.AddListener(() => { Debug.Log("click [add_exp] to: " + meta.GetChineseName()); });
                btn_add_elite.onClick.RemoveAllListeners();
                btn_add_elite.onClick.AddListener(() => { Debug.Log("click [add_elite] to: " + meta.GetChineseName()); });

                transform.GetComponent<Text>("CharInfoPanel/RightPanel/Feature/txt_feature").text = meta.GetFeature();
                Transform talentTran = transform.Find("CharInfoPanel/RightPanel/Talent/List/img_ground");
                foreach (Transform talent in talents) {
                    Destroy(talent.gameObject);
                }
                talents.Clear();
                foreach (string s in meta.GetTalent()) {
                    Transform talent = Object.Instantiate(talentTran, talentTran.parent);
                    talent.gameObject.SetActive(true);
                    talents.Add(talent);
                    talent.GetComponent<Text>("txt_talent").text = s;
                }
                update();
            }

            public void update() {
                CharAttribute charAttribute = charData.GetAttribute();
                txt_health.text = $"{charAttribute.GetMaxHealth():0}";
                txt_atk.text = $"{charAttribute.GetAtk():0}";
                txt_def.text = $"{charAttribute.GetDef():0}";
                txt_magic_resistance.text = $"{charAttribute.GetMagicResistance():0}";
                txt_respawn_time.text = charAttribute.GetRespawnTime() > 60 ? "慢" : charAttribute.GetRespawnTime() > 40 ? "中等" : "较快";
                txt_cost.text = $"{charAttribute.GetCost():0}";
                txt_block.text = $"{charAttribute.GetBlock():0}";
                txt_atk_speed.text = charAttribute.GetAtkSpeed() >= 1.6 ? "慢" : charAttribute.GetAtkSpeed() >= 1.2 ? "较慢" : charAttribute.GetAtkSpeed() >= 1 ? "中等" : charAttribute.GetAtkSpeed() >= 0.8 ? "快" : "非常快";

                img_health_percentage.fillAmount = charAttribute.GetMaxHealth() / 5000;
                img_atk_percentage.fillAmount = charAttribute.GetAtk() / 1000;
                img_def_percentage.fillAmount = charAttribute.GetDef() / 1000;
                img_magic_resistance_percentage.fillAmount = charAttribute.GetMagicResistance() / 100;

                txt_level.text = $"{charData.GetLevel()}";
                txt_max_level.text = $"{charData.GetMaxLevel()}";
                txt_exp.text = $"<color=#EECB04>{charData.GetExp()}</color>/{charData.GetMaxExp()}";
                img_exp_percentage.fillAmount = (float) charData.GetExp() / charData.GetMaxExp();

                img_elite.sprite = CharManager.Inst().GetCharEliteImage("big_" + charData.GetElite());
            }
        }
    }
}