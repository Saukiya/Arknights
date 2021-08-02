using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools {
    public class Delay : MonoSingle<Delay> {
        
        private List<ActionPackage> actions = new List<ActionPackage>();
        
        public static void add(Action action, float time) {
            Inst().actions.Add(new ActionPackage(action, time));
        }

        private void Update() {
            for (int i = actions.Count - 1; i >= 0; i--) {
                if (actions[i].time > 0) {
                    actions[i].time -= Time.deltaTime;
                } else {
                    actions[i].action();
                    actions.RemoveAt(i);
                }
            }
        }

        private class ActionPackage {
            internal Action action;
            internal float time;
            
            internal ActionPackage(Action action, float time) {
                this.action = action;
                this.time = time;
            }
        }
    }
}