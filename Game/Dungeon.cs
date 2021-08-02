using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data.Char;
using Data.Dungeon;
using Data.Monster;
using Script.Event;
using Scripts.Game.Event;
using UI;
using UnityEngine;

namespace Scripts.Game {
    public class Dungeon : MonoBehaviour {
        internal static Dungeon inst { get; private set; }

        // 对局数据
        public DungeonMeta dungeonMeta;

        // 金币
        public float cost;

        // 生命值
        public int health;

        // 波次(时间-出生点-目标点-怪物ID)
        public List<SpawnData> spawnList;

        // 击杀+走向终点的数量
        internal int skillAmount;

        // 关卡总数量
        internal int monsterSize;

        // 运行中的实体
        internal List<Entity> list;

        // 副本涩像机
        internal new Camera camera;
        internal Vector3 cameraPosition;

        // 关卡状态机
        internal StateManager sa;

        public void Awake() {
            inst = this;
            monsterSize = spawnList.Sum(spawnData => spawnData.amount);
            list = new List<Entity>();
            camera = transform.GetComponent<Camera>("Camera");
            cameraPosition = camera.transform.position;
            // 设置状态机，静止、运行、胜利、失败
            sa = new StateManager();
            sa.AddStatus("Idle", new State());
            sa.AddStatus("Run", new State()
                .OnEnter(() => { StartCoroutine(SpawnMonster()); })
                .OnStay(() => { cost = Mathf.Min(cost + Time.deltaTime, 99); })
                .OnExit(() => { StopCoroutine(SpawnMonster()); })
                .AddCondition("Win", () => health > 0 && skillAmount == monsterSize)
                .AddCondition("Lose", () => health == 0));
            sa.AddStatus("Win", new State()
                .OnEnter(() => {
                    Time.timeScale = 1;
                    UIManager.Inst().Show("GameWinUI");
                }));
            sa.AddStatus("Lose", new State()
                .OnEnter(() => {
                    Time.timeScale = 1;
                    UIManager.Inst().Show("GameLoseUI");
                }));
            sa.Name = "Idle";
        }

        public void Start() {
            UIManager.Inst().Show("GameUI");
        }

        IEnumerator SpawnMonster() {
            foreach (SpawnData spawnData in spawnList) {
                yield return new WaitForSeconds(spawnData.time);
                for (int i = 0; i < spawnData.amount; i++) {
                    Monster monster = new Monster(spawnData, Instantiate(GameManager.Inst().monsterPrefab, transform));
                    list.Add(monster);
                    EventSystem.Inst().Call(new CreateEntityEvent(monster));
                    if (i != spawnData.amount - 1) {
                        yield return new WaitForSeconds(spawnData.interval);
                    }
                }
            }
        }

        public void Update() {
            sa.Tick();
            for (int i = list.Count - 1; i >= 0; i--) {
                list[i].Update();
            }
        }

        public void SpawnChar(Transform block, CharData data, Vector3 direction) {
            Char c = new Char(data, Instantiate(GameManager.Inst().charPrefab, block), direction);
            cost -= data.GetAttribute().GetCost();
            list.Add(c);
            EventSystem.Inst().Call(new CreateEntityEvent(c));
        }

        public void Remove(Entity entity) {
            EventSystem.Inst().Call(new RemoveEntityEvent(entity));
            Destroy(entity.transform.gameObject);
            list.Remove(entity);
        }

        public void OnDestroy() {
            inst = null;
        }

        [Serializable]
        public class SpawnData {
            [SerializeField]
            internal string monsterID;

            [SerializeField]
            internal int time;

            [SerializeField]
            internal int amount;

            [SerializeField]
            internal float interval;

            [SerializeField]
            internal Transform spawnPoint;

            [SerializeField]
            internal Transform[] paths;

            [SerializeField]
            internal float[] timeIdle;

            public MonsterData GetMeta() {
                return MonsterManager.Inst().GetMeta(monsterID);
            }
        }
    }
}