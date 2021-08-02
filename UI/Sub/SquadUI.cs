using System.Collections.Generic;
using System.Linq;
using Data.Char;
using Data.Player;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class SquadUI : UIBase {
        private Transform card_prefab;
        private RectTransform charList;
        private RectTransform deleteButton;
        
        private PlayerData data;
        private List<CharCard> charCards;

        public override void Init() {
            
            card_prefab = transform.Find("DownPanel/CharList/CardPrefab");
            charList = card_prefab.parent as RectTransform;
            deleteButton = transform.Find("DeleteButton") as RectTransform;
            
            charCards = new List<CharCard>();
            for (int i = 0; i < 4; i++) {
                charCards.Add(new CharCard(Instantiate(card_prefab, card_prefab.parent)));
            }
            deleteButton.GetComponent<Button>().onClick.AddListener(() => {
                if (data.GetSquad().Any(name => data.GetCharData(name) != null)) {
                    CommonDialogUI.Message(CommonDialogUI.GroundType.WHITE, "是否确认移除当前编队中的所有干员?", () => {
                        data.ResetSquad();
                        UpdateView();
                    });
                }
            });
        }

        public override void UpdateView() {
            data = PlayerManager.Inst().Get();
            for (int i = 0; i < 4; i++) {
                CharData charData = data.GetCharData(data.GetSquad()[i]);
                CharCard charCard = charCards[i];
                charCard.transform.DOScale(0.9f, 0.1f).OnComplete(() => {
                    charCard.SetData(charData);
                    charCard.transform.DOScale(1f, 0.15f);
                });
            }
        }

        public override void Show() {
            base.Show();
            charList.localScale = new Vector3(1, 1, 1);
            charList.anchoredPosition = new Vector2(300, 0);
            charList.DOAnchorPosX(0, 0.5f);
            deleteButton.anchoredPosition = new Vector2(0, 0);
            deleteButton.DOAnchorPosY(-150, 0.3f);
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, 0.3f);
        }

        public override void Hide(bool destroy = false) {
            charList.DOScale(0.9f, 0.5f);
            charList.DOAnchorPosX(-300, 0.5f);
            deleteButton.DOAnchorPosY(0, 0.3f);
            canvasGroup.DOFade(0, 0.3f).OnComplete(() => { base.Hide(destroy); });
        }

        public class CharCard {
            internal Transform transform;
            private Transform empty;
            private Transform exist;

            private CharData data;
            
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
            
            public CharCard(Transform transform) {
                this.transform = transform;
                transform.gameObject.SetActive(true);
                
                empty = transform.Find("Empty");
                exist = transform.Find("Exist");
                name = exist.GetComponent<Text>("txt_name");
                level = exist.GetComponent<Text>("txt_level");
                expPercentage = exist.GetComponent<Image>("img_exp_ground/img_exp_percentage");
                charCard = exist.GetComponent<Image>("img_char_card");
                rarity1 = exist.GetComponent<Image>("img_rarity_1");
                rarity2 = exist.GetComponent<Image>("img_rarity_2");
                rarity3 = exist.GetComponent<Image>("img_rarity_3");
                rarity4 = exist.GetComponent<Image>("img_rarity_4");
                rarityStar = exist.GetComponent<Image>("img_rarity_star");
                profession = exist.GetComponent<Image>("img_profession");
                eliteImage = exist.GetComponent<Image>("img_elite");
                empty.GetComponent<Button>().onClick.AddListener(OnClick);
                exist.GetComponent<Button>().onClick.AddListener(OnClick);
            }

            public void SetData(CharData data) {
                empty.gameObject.SetActive(data == null);
                exist.gameObject.SetActive(data != null);
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
                CharSelectUI.Show(data?.GetId());
            }
        }
    }
}