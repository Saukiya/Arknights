using System;
using Data.Char;
using Data.Dungeon;
using Data.Player;
using DG.Tweening;
using Manager;
using Scripts.Game;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace UI.Sub {
    public class GameWinUI : UIBase {
        private RectTransform do1;
        private RectTransform do2;
        private Image black;
        private Image blur;

        private Button panel;
        private Image charImage;

        public override void Init() {
            do1 = transform.Find("Do1") as RectTransform;
            do2 = transform.Find("Do2") as RectTransform;
            black = transform.GetComponent<Image>("Black");

            blur = transform.GetComponent<Image>();
            Material blurryShader = blur.material;
            blurryShader.SetFloat("_Size", 160);

            panel = transform.GetComponent<Button>("Panel");
            charImage = panel.transform.GetComponent<Image>("CharImage");
            panel.onClick.AddListener(() => {
                UIManager.Inst().Hide("GameWinUI", true);
                UIManager.Inst().Hide("GameUI", true);
                Delay.add(() => UIManager.Inst().Show("HomeUI"), 2);
            });
        }

        public override void Show() {
            base.Show();
            do1.gameObject.SetActive(true);
            do2.gameObject.SetActive(true);
            Vector2 do2AnchoredPosition = do2.anchoredPosition;
            do2AnchoredPosition.x = -2200;
            do2.anchoredPosition = do2AnchoredPosition;
            black.gameObject.SetActive(true);
            panel.gameObject.SetActive(false);
            blur.enabled = false;
            black.color = new Color(0, 0, 0, 0.0001f);
            do2.DOAnchorPosX(0, 0.6f).SetDelay(0.3f).OnComplete(() => {
                do2.DOAnchorPosX(2200, 0.6f).SetDelay(0.5f);
                black.DOFade(1, 0.4f).SetDelay(0.8f).OnComplete(() => {
                    do1.gameObject.SetActive(false);
                    do2.gameObject.SetActive(false);
                    blur.enabled = true;
                    //激活面板 获取Dungeon数据、刷新。
                    PlayerData playerData = PlayerManager.Inst().Get();
                    DungeonMeta dungeonMeta = Dungeon.inst.dungeonMeta;
                    CharData data;
                    do {
                        data = playerData.GetCharData(playerData.GetSquad()[new Random().Next(playerData.GetSquad().Length)]);
                    } while (data == null);
                    charImage.sprite = data.GetCharMeta().GetImage();
                    panel.transform.GetComponent<Text>("DownPanel/ID").text = dungeonMeta.GetId();
                    panel.transform.GetComponent<Text>("DownPanel/Name").text = dungeonMeta.GetName();
                    panel.gameObject.SetActive(true);
                    charImage.color = new Color(1, 1, 1, 0);
                    black.DOFade(0, 0.4f).OnComplete(() => {
                        black.gameObject.SetActive(false);
                        charImage.DOFade(1, 0.7f);
                    });
                });
            });
        }

        public override void Hide(bool destroy = false) {
            black.gameObject.SetActive(true);
            black.DOFade(1, 1.2f).OnComplete(() => { base.Hide(destroy); });
        }
    }
}