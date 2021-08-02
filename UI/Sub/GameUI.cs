using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Data.Char;
using Data.Player;
using DG.Tweening;
using Manager;
using Script.Event;
using Scripts.Game;
using Scripts.Game.Event;
using Spine.Unity;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Char = Scripts.Game.Char;
using EventHandler = Script.Event.EventHandler;
using EventSystem = Script.Event.EventSystem;

namespace UI.Sub {
    public class GameUI : UIBase, Listener {
        private static Vector2 screenScale;
        
        public Sprite[] professions;

        internal PlayerData data;
        internal Dungeon dungeon;

        private Transform healthBarPrefab;
        private Transform cardPrefab;

        private Toggle tog_speed;
        private Text txt_killMonsterAndMax;
        private Text txt_health;
        private Text txt_cost;
        private Transform tra_cost_float;
        internal PlacePanel placePanel;
        internal List<CharCard> charList;
        internal List<HealthBar> barList;

        private Image blackGround;

        public override void Init() {
            charList = new List<CharCard>();
            barList = new List<HealthBar>();
            healthBarPrefab = transform.Find("HealthBarList/HealthBarPrefab");
            cardPrefab = transform.Find("CharList/CharPrefab");
            txt_killMonsterAndMax = transform.GetComponent<Text>("Title/KillMonsterValue");
            txt_health = transform.GetComponent<Text>("Title/HealthValue");
            txt_cost = transform.GetComponent<Text>("CostPanel/Value");
            tra_cost_float = transform.Find("CostPanel/ValueFloat");

            healthBarPrefab.gameObject.SetActive(false);
            cardPrefab.gameObject.SetActive(false);
            cardPrefab.gameObject.AddComponent<CharCard>();

            placePanel = transform.Find("PlacePanel/Plate/Handle").gameObject.AddComponent<PlacePanel>();
            blackGround = transform.GetComponent<Image>("BlackGround");
            //变速
            tog_speed = transform.GetComponent<Toggle>("SpeedToggle");
            Image speed1 = tog_speed.transform.GetComponent<Image>("Speed1");
            Image speed2 = tog_speed.transform.GetComponent<Image>("Speed2");
            speed2.gameObject.SetActive(false);
            tog_speed.onValueChanged.AddListener(boo => {
                speed1.gameObject.SetActive(!boo);
                speed2.gameObject.SetActive(boo);
                tog_speed.graphic = boo ? speed2 : speed1;
                Time.timeScale = boo ? 2 : 1;
            });
            
            //返回
            transform.GetComponent<Button>("BackButton").onClick.AddListener(() => {
                CommonDialogUI.Message(CommonDialogUI.GroundType.BLACK, "确定要退出本场战斗?", () => {
                    UIManager.Inst().Hide("GameUI", true);
                    Delay.add(() => UIManager.Inst().Show("HomeUI"), 2);
                });
            });
            screenScale = UIManager.Inst().GetCanvasSize() / new Vector2(Screen.width, Screen.height);
        }

        public void Update() {
            if (dungeon) {
                txt_cost.text = Math.Floor(dungeon.cost).ToString(CultureInfo.InvariantCulture);
                tra_cost_float.localScale = new Vector3((float) (dungeon.cost - Math.Floor(dungeon.cost)), 1, 1);
                txt_killMonsterAndMax.text = $"{dungeon.skillAmount}/{dungeon.monsterSize}";
                txt_health.text = $"{dungeon.health}";
                foreach (HealthBar bar in barList.Where(bar => bar.transform)) {
                    bar.update();
                }
            }
        }
        
        [EventHandler]
        void OnCreateEvent(CreateEntityEvent e) {
            barList.Add(new HealthBar(Instantiate(healthBarPrefab, healthBarPrefab.parent), e.entity));
            if (!(e.entity is Char entity)) return;
            foreach (CharCard charCard in charList.Where(charCard => charCard.data.GetId() == entity.data.GetId())) {
                charCard.transform.gameObject.SetActive(false);
            }
        }

        [EventHandler]
        void OnRemoveEvent(RemoveEntityEvent e) {
            for (int i = 0; i < barList.Count; i++) {
                if (barList[i].entity != e.entity) continue;
                HealthBar bar = barList[i];
                barList.RemoveAt(i);
                bar.group.DOFade(0, 0.5f).OnComplete(() => {
                    Destroy(bar.transform.gameObject);
                });
                return;
            }
        }

        public override void Show() {
            base.Show();
            data = PlayerManager.Inst().Get();
            dungeon = Dungeon.inst;
            SoundManager.Inst().PlayMusic(GameManager.Inst().clip, false, () => { SoundManager.Inst().PlayMusic(GameManager.Inst().loop_clip, true); });
            foreach (string charID in data.GetSquad()) {
                CharData charData = data.GetCharData(charID);
                if (charData == null) continue;
                CharCard charCard = Instantiate(cardPrefab, cardPrefab.parent).GetComponent<CharCard>();
                charCard.gameObject.SetActive(true);
                charCard.Init(this, charData);
                charList.Add(charCard);
            }
            blackGround.gameObject.SetActive(true);
            blackGround.DOFade(0, 1f).SetDelay(1).OnComplete(() => {
                blackGround.gameObject.SetActive(false);
                dungeon.sa.Name = "Run";
            });
            
            EventSystem.Inst().AddListener(this);
        }

        public override void Hide(bool destroy = false) {
            EventSystem.Inst().RemoveListener(this);
            blackGround.gameObject.SetActive(true);
            SoundManager.Inst().StopMusic();
            blackGround.DOFade(1, 1f).OnComplete(() => {
                // 清理过期的骨骼
                foreach (CharCard charCard in charList.Where(charCard => charCard.place)) {
                    Destroy(charCard.place);
                }
                Destroy(Dungeon.inst.transform.gameObject);
                base.Hide(true);
            });
            
        }

        public Sprite GetProfessionSprite(CharProfession type) {
            return professions.FirstOrDefault(profession => profession.name == type.ToString());
        }

        public class HealthBar {
            internal Entity entity;
            internal RectTransform transform;
            internal CanvasGroup group;
            private Transform lerp;
            private Image health;
            
            public HealthBar(Transform transform, Entity entity) {
                transform.gameObject.SetActive(true);
                this.transform = transform as RectTransform;
                this.entity = entity;
                group = transform.GetComponent<CanvasGroup>();
                lerp = transform.Find("Lerp");
                health = transform.GetComponent<Image>("Health");
                health.color = Color.green;
            }

            public void update() {
                transform.anchoredPosition = (Dungeon.inst.camera.WorldToScreenPoint(entity.transform.position) - new Vector3(0, 50, 0)) * screenScale;
                float percentage = Mathf.Clamp(entity.health / entity.attribute.GetMaxHealth(), 0, 1);
                
                group.alpha = Mathf.Lerp(group.alpha, percentage == 1 ? 0 : 1, Time.deltaTime * 3);
                health.color = Color.Lerp(Color.red, Color.green, percentage);
                health.transform.localScale = new Vector3(percentage, 1, 1);
                lerp.localScale = new Vector3(Mathf.Lerp(lerp.localScale.x, percentage, Time.deltaTime * 3), 1, 1);
            }
        }
        
        // 放置面板：用于确认塔的方向
        public class PlacePanel : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {
            //界面
            private RectTransform panel;

            //盘子
            private RectTransform plate;

            //句柄
            private Transform handle;
            
            //Group
            private CanvasGroup group;

            //骨骼对象
            private Transform place;
            
            // 放置方块
            private Transform block;
            
            // 干员数据
            private CharData data;

            //距离
            private const short radius = 220;

            //最终点
            private Vector2 dir = Vector2.zero;

            private List<Transform> atkRangedisplays; 


            public void Awake() {
                plate = transform.parent as RectTransform;
                panel = plate.parent as RectTransform;
                handle = plate.Find("Handle");
                group = panel.GetComponent<CanvasGroup>();
                atkRangedisplays = new List<Transform>();
                
                panel.GetComponent<Button>("Cancel").onClick.AddListener(Cancel);
                panel.gameObject.SetActive(false);
            }

            public void Place(Transform place, Transform block, CharData data) {
                this.place = place;
                this.block = block;
                this.data = data;
                // 求关卡摄像机与block的相对rotation、屏幕坐标，赋值给transform；
                panel.gameObject.SetActive(true);
                group.alpha = 0;
                group.DOFade(1, 0.2f);
                Camera camera = Dungeon.inst.camera;
                Vector3 position = place.position;
                camera.transform.DOMove(Vector3.ClampMagnitude(position + new Vector3(-4.5f, 7, 1f) - camera.transform.position, 1), 0.3f).SetRelative(true).OnUpdate(() =>
                    panel.anchoredPosition = camera.WorldToScreenPoint(position) * screenScale);
            }

            public void Cancel() {
                dir = Vector2.zero;
                Vector3 position = place.position;
                clearAtkRange();
                Destroy(place.gameObject);
                handle.localPosition = Vector2.zero;
                group.DOFade(0, 0.2f).SetDelay(0.1f).OnComplete(() => { panel.gameObject.SetActive(false); });
                Camera camera = Dungeon.inst.camera;
                camera.transform.DOMove(Dungeon.inst.cameraPosition, 0.3f).OnUpdate(() => {
                    panel.anchoredPosition = camera.WorldToScreenPoint(position) * screenScale;
                });
            }

            public void OnDrag(PointerEventData eventData) {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(plate, eventData.position, eventData.pressEventCamera, out Vector2 localPoint)) {
                    localPoint *= Math.Min(1, radius / (Math.Abs(localPoint.x) + Math.Abs(localPoint.y)));
                    if (dir != Vector2.zero) {
                        if (Vector2.Distance(dir, localPoint) > radius * 0.45f) {
                            dir = Vector2.zero;
                            // 清理预制体
                            clearAtkRange();
                        }
                    } else {
                        if (Math.Abs(localPoint.x) > radius * 0.7f) {
                            dir = new Vector2(localPoint.x > 0 ? radius : -radius, 0);
                        } else if (Math.Abs(localPoint.y) > radius * 0.7f) {
                            dir = new Vector2(0, localPoint.y > 0 ? radius : -radius);
                        }
                        if (dir != Vector2.zero) {
                            // 生成预制体
                            Vector3 position = block.position + new Vector3(0, 1, 0);
                            Quaternion rotation = Quaternion.Euler(90, 0, 0);
                            Quaternion qua = Quaternion.LookRotation(new Vector3(dir.y, 0, -dir.x));
                            foreach (Vector3 vector3 in data.GetCharMeta().GetAttackRange()) {
                                if (Physics.Raycast(position + qua * vector3, Vector3.down, out RaycastHit hitInfo, 4, 1 << LayerMask.NameToLayer("Ground"))) {
                                    atkRangedisplays.Add(Instantiate(GameManager.Inst().atkRangeDisplay, hitInfo.point + new Vector3(0, 0.1f, 0), rotation));
                                }
                            }
                        }
                    }
                    handle.localPosition = dir != Vector2.zero ? dir : localPoint;
                }
            }

            public void clearAtkRange() {
                foreach (Transform atkRangedisplay in atkRangedisplays) {
                    Destroy(atkRangedisplay.gameObject);
                }
                atkRangedisplays.Clear();
            }

            public void OnPointerUp(PointerEventData eventData) {
                if (dir == Vector2.zero) {
                    handle.localPosition = Vector2.zero;
                } else {
                    // 提示处理完毕
                    dir = dir.normalized;
                    Dungeon.inst.SpawnChar(block, data, new Vector3(dir.y, 0, -dir.x));
                    Cancel();
                }
            }

            public void OnPointerDown(PointerEventData eventData) { }
        }

        // 卡牌预制体：用来拖拽放置到场景中
        public class CharCard : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {
            private Image img_charImage;
            private Text txt_cost;
            private Image img_profession;
            internal CharData data;
            private GameObject black;

            private int cost;
            
            // 骨骼体
            internal Transform place;
            // 吸附方块
            private Transform block;
            private GameUI ui;

            private void Awake() {
                img_charImage = transform.GetComponent<Image>("Panel/CharImage");
                txt_cost = transform.GetComponent<Text>("Panel/Cost/Value");
                img_profession = transform.GetComponent<Image>("Panel/Profession");
                black = transform.Find("Panel/Black").gameObject;
            }

            public void Init(GameUI ui, CharData data) {
                //
                this.ui = ui;
                this.data = data;
                cost = data.GetAttribute().GetCost();
                img_charImage.sprite = data.GetCharMeta().GetAvatar();
                txt_cost.text = cost.ToString();
                img_profession.sprite = ui.GetProfessionSprite(data.GetCharMeta().GetProfession());
            }

            public void Update() {
                black.gameObject.SetActive(ui.dungeon.cost < cost);
            }

            // 操作行为：drag触发->检查是否在拖拽骨骼中->是#继续调整骨骼位置+选中当前目标/否#创建拖拽骨骼
            public void OnDrag(PointerEventData eventData) {
                // 检查是否存在骨骼
                if (!place) {
                    if (ui.dungeon.cost < data.GetAttribute().GetCost() || ui.placePanel.gameObject.activeInHierarchy) {
                        eventData.dragging = false;
                        return;
                    }
                    //TODO SELECT THIS
                    place = Instantiate(GameManager.Inst().charPlacePrefab);
                    SkeletonAnimation sa = place.GetComponent<SkeletonAnimation>("Model/Skeleton");
                    sa.skeletonDataAsset = data.GetCharMeta().GetSkeleton();
                    sa.Initialize(true);
                }
                Ray ray = ui.dungeon.camera.ScreenPointToRay(eventData.position);
                // 无block坐标时
                // 判断hitInfo.point 与 hit.collider 的距离是否 <0.5 && hitInfo.collider是否存在子物体， 是的话赋值block坐标， 不是的话不处理
                // 有block坐标时
                // 判断block坐标与hit.point 是否 > 0.75, 是的话block坐标置空
                // 判断
                if (Physics.Raycast(ray, out RaycastHit hitInfo, 15, 1 << LayerMask.NameToLayer("Ground"))) {
                    Collider collider = hitInfo.collider;
                    if (block) {
                        if (Vector3.Distance(block.position, hitInfo.point) > 0.6) {
                            block = null;
                        }
                    }
                    if (!block) {
                        if (hitInfo.collider.CompareTag(data.GetCharMeta().GetPosition().ToString())) {
                            if (Vector3.Distance(collider.transform.position, hitInfo.point) < 0.3 && collider.transform.childCount == 0) {
                                block = collider.transform;
                            }
                        }
                    }
                } else {
                    block = null;
                }

                if (block) {
                    place.position = block.position;
                } else {
                    place.position = ray.origin - (ray.origin.y - 0.3f) / ray.direction.y * ray.direction;
                }
            }

            public void OnPointerDown(PointerEventData eventData) { }

            // 卡池#抬起事件
            public void OnPointerUp(PointerEventData eventData) {
                // 检查是否存在拖拽体（骨骼）
                if (place) {
                    // 检查是否依附于方块
                    if (block) {
                        // 打开放置面板
                        ui.placePanel.Place(place, block, data);
                        place = null;
                    } else {
                        // 吹毁拖拽体
                        Destroy(place.gameObject);
                    }
                }
            }
        }
    }
}