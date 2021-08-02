using System.Collections.Generic;
using Data;
using Data.Char;
using Manager;
using Tools;
using UnityEngine;

namespace Scripts.Game {
    public class Char : Entity {
        internal CharData data;
        internal new CharAttribute attribute;
        internal TargetSelector<Monster> ts;
        
        //方向
        private Transform dir;
        //当前阻挡数
        internal int block;
        
        private CharController controller;
        //临时代码
        internal Monster target;

        private float timeAtk;


        public Char(CharData data, Transform transform, Vector3 direction) : base(transform, data.GetAttribute()) {
            this.data = data;
            transform.forward = Vector3.forward;
            //设置骨骼
            sa.skeletonDataAsset = data.GetCharMeta().GetSkeleton();
            sa.Initialize(true);
            attribute = data.GetAttribute();
            //攻击间隔
            timeAtk = attribute.GetAtkSpeed();
            
            // 设置阻挡数
            block = data.GetAttribute().GetBlock();
            // 设置方向
            dir = transform.Find("Dir");
            dir.forward = direction;
            model.localScale = new Vector3(1, 1, direction.z > 0 ? -1 : 1);
            SetFace(direction);

            ts = new TargetSelector<Monster>(dir, data.GetCharMeta().GetAttackRange());
            //设置排序方法 - 暂时没有依据距离来判定进远，仅通过剩余移动节点判断优先级
            ts.SetSort((a, b) => (a.spawnData.paths.Length - a.index).CompareTo(b.spawnData.paths.Length - b.index));

            
            state.AddStatus("Idle", new State()
                .OnEnter(() => {
                    sa.loop = true;
                    sa.AnimationName = "Idle";
                })
                .AddCondition("Attack", () => {
                    List<Monster> targets = ts.GetTargets();
                    if (targets.Count <= 0) return false;
                    target = targets[0];
                    return true;
                }));
            
            state.AddStatus("Attack", new State()
                .OnEnter(() => {
                    sa.loop = true;
                    sa.AnimationName = "Attack";
                })
                .OnStay(() => {
                    // 攻速占用
                    // if (!(timeAtk >= attribute.GetAtkSpeed())) return;
                    // sa.loop = false;
                    // sa.AnimationName = "Attack";
                    // Debug.Log("Attack: " + target);
                    // timeAtk = 0;
                })
                .AddCondition("Idle", () => target == null));

            state.AddStatus("Die", new State()
                .OnEnter(() => {
                    sa.loop = false;
                    sa.AnimationName = "Die";
                }));
            state.AddCondition("Die", isDead);

            sa.state.Start += entry => {
                if (entry.Animation.Name == "Attack") {
                    SetFace(target.transform.position - transform.position);
                }
            };
            sa.state.Complete += entry => {
                switch (entry.Animation.Name) {
                    case "Die":
                        Delay.add(() => Dungeon.inst.Remove(this), 1);
                        break;
                    case "Attack":
                        if (sa.AnimationName == "Die") return;
                        List<Monster> targets = ts.GetTargets();
                        if (targets.Count > 0) {
                            target = targets[0];
                            SetFace(target.transform.position - transform.position);
                        } else {
                            target = null;
                        }
                        break;
                    case "Start":
                        state.Name = "Idle";
                        break;
                }
            };
            DamageType type = data.GetCharMeta().GetProfession() == CharProfession.SHU_SHI ? DamageType.Magic : DamageType.Physical;
            sa.state.Event += (entry, e) => {
                if (e.Data.Name == "OnAttack" && target != null && !target.isDead()) {
                    target.damage(attribute.GetAtk(), type);
                }
            };
        }
        
        public override void Update() {
            timeAtk += Time.deltaTime;
            base.Update();
        }
    }
}