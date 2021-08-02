using System;
using Manager;
using Tools;
using XLua;

namespace UI {

    [CSharpCallLua]
    public class RuaUI : UIBase { // LUA脚本启动器
        public Injection[] injections;

        private static LuaEnv luaEnv;
        private static LuaEnv LuaEnv => luaEnv ?? (luaEnv = LuaEnvManager.Inst().LuaEnv());
        
        private LuaTable scriptEnv;
        private Action luaUpdate;
        private Action luaUpdateView;
        private Action luaShow;
        private Action<bool> luaHide;


        public override void Init() { // 初始化
            scriptEnv = LuaEnv.NewTable();
            LuaTable meta = LuaEnv.NewTable();
            meta.Set("__index", LuaEnv.Global); //设置元表
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            
            scriptEnv.Set("self", this);
            foreach (Injection injection in injections) {//带入脚本变量
                scriptEnv.Set(injection.name, injection.value);
            }
            LuaEnv.DoString(LuaEnvManager.LoadLuaText("UI." + Name), "chunk", scriptEnv);
            
            scriptEnv.Get("update", out luaUpdate);
            scriptEnv.Get("updateView", out luaUpdateView);
            scriptEnv.Get("show", out luaShow);
            scriptEnv.Get("hide", out luaHide);
            scriptEnv.Get<Action>("init")?.Invoke();
        }


        private void Update() { //更新
            luaUpdate?.Invoke();
        }


        public override void UpdateView() { //更新
            luaUpdateView?.Invoke();
        }

        public override void Show() {
            luaShow?.Invoke();
        }

        public override void Hide(bool destroy = false) {
            if (luaHide != null)
                luaHide(destroy);
            else {
                base.Hide(destroy);
            }
        }


        public LuaTable GetScriptEnv() => scriptEnv;
        public void BaseShow() => base.Show(); // lua层无法调用被覆盖的父类方法
        public void BaseHide(bool destroy = false) => base.Hide(destroy); // lua层无法调用被覆盖的父类方法
    }
}