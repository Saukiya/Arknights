using Data;
using Data.Dungeon;
using Data.Player;
using DG.Tweening;
using Manager;
using Scripts.Game;
using Tools;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Sub {
    public class SelectDungeonUI : UIBase {

        private InfoPanel infoPanel;
        
        public override void Init() {
            infoPanel = new InfoPanel(transform.Find("InfoPanel"));
            Ground ground = new Ground(transform.Find("Ground"));
            transform.GetComponent<ScrollRect>("Scroll").onValueChanged.AddListener(vec => {
                ground.SetLoc(vec.x);
                if (infoPanel.data) {
                    infoPanel.Hide();
                }
            });
        }

        public override void UpdateView() {
            infoPanel.Update();
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

        // 查看对局关卡数据-UI按钮调用
        public void OpenDungeonInfo(string str) {
            DungeonMeta dungeonMeta = DungeonManager.Inst().GetMeta(str);
            // 显示数据到panel，然后设置panel的Canvas Group;
            infoPanel.Show(dungeonMeta);
        }

        private class InfoPanel {
            private CanvasGroup group;
            
            private Text txt_reason;
            private Text txt_name;
            private Text txt_id;
            private Text txt_description;
            private Text txt_required;

            internal DungeonMeta data;

            internal InfoPanel(Transform transform) {
                group = transform.GetComponent<CanvasGroup>();
                txt_reason = transform.GetComponent<Text>("TitleGround/Reason");
                txt_name = transform.GetComponent<Text>("NameGround/Name");
                txt_id = transform.GetComponent<Text>("NameGround/ID");
                txt_description = transform.GetComponent<Text>("Description");
                txt_required = transform.GetComponent<Text>("EnterButton/Image/Required");
                transform.GetComponent<Button>("EnterButton").onClick.AddListener(() => {
                    if (!data) return;
                    PlayerData playerData = PlayerManager.Inst().Get();
                    if (playerData.GetReason() < data.GetReason()) {
                        CommonDialogUI.Message(CommonDialogUI.GroundType.BLACK, "理智不足，无法进入战斗");
                        return;
                    }
                    GameObject prefab = data.GetDungeonPrefab();
                    playerData.SetReason(playerData.GetReason() - data.GetReason());
                    // 防止点击两次
                    data = null;
                    UIManager.Inst().Hide("HomeUI", true);
                    Delay.add(() => UIManager.Inst().Hide("SelectDungeonUI", true), 1);
                    Delay.add(() => Instantiate(prefab), 2);
                });
                group.gameObject.SetActive(false);
            }

            internal void Show(DungeonMeta data) {
                if (!this.data) {
                    group.alpha = 0;
                    group.gameObject.SetActive(true);
                    group.DOFade(1, 0.5f);
                }
                this.data = data;
                Update();
            }

            internal void Hide() {
                data = null;
                group.DOFade(0, 0.3f).OnComplete(() => {
                    group.gameObject.SetActive(false);
                });
                
            }

            internal void Update() {
                PlayerData playerData = PlayerManager.Inst().Get();
                txt_reason.text = $"{playerData.GetReason()}/{playerData.GetMaxReason()}";
                if (!data) return;
                txt_name.text = data.GetName();
                txt_id.text = data.GetId();
                txt_description.text = data.GetDescription();
                txt_required.text = $"-{data.GetReason()}";
            }

        }
        
        private class Ground {
            private RectTransform transform;
            private StateManager sa = new StateManager();

            internal Ground(Transform transform) {
                this.transform = transform as RectTransform;
                Image ground1 = transform.GetComponent<Image>("Ground1");
                Image ground2 = transform.GetComponent<Image>("Ground2");
                sa.AddStatus("1", new State()
                    .OnEnter(() => {
                        ground1.DOFade(1, 0.4f);
                        ground2.DOFade(0, 0.4f);
                    }));
                sa.AddStatus("2", new State()
                    .OnEnter(() => {
                        ground1.DOFade(0, 0.4f);
                        ground2.DOFade(1, 0.4f);
                    }));
                ground2.color = new Color(1, 1, 1, 0);
                SetLoc(0);
            }

            internal void SetLoc(float value) {
                if (value > 0.6) {
                    sa.Name = "2";
                } else if (value < 0.4) {
                    sa.Name = "1";
                }
                transform.anchorMin = new Vector2(value, 0);
                transform.anchorMax = new Vector2(value, 1);
                transform.pivot = new Vector2(value, 0.5f);
            }
        }
    }
}