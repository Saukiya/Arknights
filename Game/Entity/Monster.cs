using System.Collections.Generic;
using Data;
using Data.Monster;
using Manager;
using Tools;
using UnityEngine;
using Random = UnityEngine.Random;
using SpawnData = Scripts.Game.Dungeon.SpawnData;

namespace Scripts.Game {
    public class Monster : Entity {
        internal MonsterData data;
        internal new MonsterAttribute attribute;
        internal TargetSelector<Char> ts;
        
        internal SpawnData spawnData;
        internal Char target;
        public int index;
        private float timeAtk;

        public Monster(SpawnData spawnData, Transform transform) : base(transform, spawnData.GetMeta().attribute) {
            data = spawnData.GetMeta();
            attribute = data.attribute;
            sa.skeletonDataAsset = data.skeleton;
            sa.Initialize(true);
            transform.position = spawnData.spawnPoint.position;

            //攻击间隔
            timeAtk = attribute.GetAtkSpeed();
            
            this.spawnData = spawnData;
            //停止间隔
            float timeIdle = index <= spawnData.timeIdle.Length - 1 ? spawnData.timeIdle[index] : 0;
            
            // 目标选择器
            // 这里放的transform是不会改变朝向的 需要注意下（但是现在的Monster只有近战攻击呢 ^ ^
            ts = new TargetSelector<Char>(transform, new[] {Vector3.zero});
            
            model.localScale = new Vector3(1, 1, (spawnData.paths[index].position - transform.position).z > 0 ? -1 : 1);
            //创建一个名为 Moving 的状态
            state.AddStatus("Moving", new State()
                //启动方法
                .OnEnter(() => {
                    sa.loop = true;
                    sa.AnimationName = data["Move_Loop"];
                })
                //持续方法
                .OnStay(() => {
                    if (index <= spawnData.paths.Length - 1) {
                        Vector3 vec = (spawnData.paths[index].position - transform.position).normalized;
                        SetFace(vec);
                        transform.Translate(Time.deltaTime * attribute.GetMoveSpeed() * vec);
                        if (Vector3.Distance(spawnData.paths[index].position, transform.position) <= 0.1f) {
                            index++;
                            timeIdle = index <= spawnData.timeIdle.Length - 1 ? spawnData.timeIdle[index] : 0;
                        }
                    } else {
                        Dungeon.inst.health -= 1;
                        Dungeon.inst.skillAmount += 1;
                        Dungeon.inst.Remove(this);
                    }
                })
                //跳出判断
                .AddCondition("Attack", () => {
                    List<Char> targets = ts.GetTargets();
                    if (targets.Count > 0 && targets[0].block > 0) {
                        target = targets[0];
                        target.block -= 1;
                        //被阻挡时小幅度偏移
                        transform.Translate((target.transform.position - transform.position) * 0.2f);
                        transform.Translate(Quaternion.Euler(0, 90, 0) * (transform.position - target.transform.position) * Random.Range(0.5f, -0.5f));
                        return true;
                    }
                    return false;
                })
                .AddCondition("Idle", () => timeIdle > 0)
            );

            // 停止状态
            state.AddStatus("Idle", new State()
                .OnEnter(() => {
                    sa.loop = true;
                    sa.AnimationName = "Idle";
                })
                .OnStay(() => {
                    timeIdle -= Time.deltaTime;
                })
                .AddCondition("Moving", () => timeIdle <= 0)
                .AddCondition("Attack", () => {
                    List<Char> targets = ts.GetTargets();
                    if (targets.Count > 0 && targets[0].block > 0) {
                        target = targets[0];
                        target.block -= 1;
                        //被阻挡时小幅度偏移
                        transform.Translate((target.transform.position - transform.position) * 0.2f);
                        transform.Translate(Quaternion.Euler(0, 90, 0) * (transform.position - target.transform.position) * Random.Range(0.5f, -0.5f));
                        return true;
                    }
                    return false;
                })
            );
            
            state.AddStatus("Attack", new State()
                .OnStay(() => {
                    if (!(timeAtk >= attribute.GetAtkSpeed())) return;
                    sa.loop = false;
                    sa.AnimationName = "Attack";
                    timeAtk = 0;
                })
                .AddCondition("Moving", () => target == null));

            state.AddStatus("Die", new State()
                .OnEnter(() => {
                    sa.loop = false;
                    sa.AnimationName = "Die";
                    Dungeon.inst.skillAmount += 1;
                    if (target != null) {
                        //死亡返还阻挡值
                        target.block += 1;
                    }
                }));
            // 总状态条件 - 死亡
            state.AddCondition("Die", isDead);

            // 动画机事件
            sa.state.Complete += entry => {
                switch (entry.Animation.Name) {
                    case "Die":
                        Delay.add(() => Dungeon.inst.Remove(this), 1);
                        break;
                    case "Attack":
                        if (sa.AnimationName == "Die") return;
                        sa.loop = true;
                        sa.AnimationName = "Idle";
                        if (target != null) {
                            if (target.isDead() || !Dungeon.inst.list.Contains(target)) {
                                target = null;
                            }
                        }
                        break;
                }
            };

            // 自定义动画机事件
            sa.state.Event += (entry, e) => {
                if (e.Data.Name == "OnAttack" && target != null && !target.isDead()) {
                    target.damage(attribute.GetAtk(), DamageType.Physical);
                }
            };

            //默认状态
            state.Name = "Moving";
        }

        public override void Update() {
            // if (target != null) {
            //     if (target.isDead() || !Dungeon.inst.list.Contains(target)) {
            //         target = null;
            //     }
            // }
            timeAtk += Time.deltaTime;
            base.Update();
        }
    }
}