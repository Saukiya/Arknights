using System.IO;
using System.Text;
using Tools;
using UnityEngine;
using XLua;

namespace Manager {
    public class LuaEnvManager : Single<LuaEnvManager> {
        private const string luaScriptsFolder = "LuaScripts/";

        //管理Lua的虚拟机
        private LuaEnv luaEnv;

        public LuaEnv LuaEnv() => luaEnv;

        /// <summary>
        /// 只要管理器存在了，虚拟机就存在
        /// </summary>
        protected override void Initialization() {
            luaEnv = new LuaEnv();
            luaEnv.AddLoader(LuaFolderLoader);
            luaEnv.AddLoader(AssetLoader);
            luaEnv.DoString("require('Global.Global')", "chunk", luaEnv.Global);
        }

        public void DoFile(string luaFileName, string chunkName = "chunk", LuaTable env = null) {
            luaEnv.DoString($"require('{luaFileName}')", "chunk", env);
        }

        private static byte[] LuaFolderLoader(ref string filepath) {
            filepath = luaScriptsFolder + filepath.Replace(".", "/") + ".lua.txt";
            string scriptPath = Path.Combine(Application.dataPath, filepath);
            return !File.Exists(scriptPath) ? null : Encoding.UTF8.GetBytes(File.ReadAllText(scriptPath));
        }

        public static string LoadLuaText(string filepath) {
            filepath = luaScriptsFolder + filepath.Replace(".", "/") + ".lua";
            string[] args = filepath.Split('/');
            string name = args[args.Length - 1];
            TextAsset textAsset = Asset.Load<TextAsset>(filepath.Substring(0, filepath.Length - name.Length), name);
            return textAsset == null ? null : textAsset.text;
        }

        // 从AB包中加载lua文件
        private static byte[] AssetLoader(ref string filepath) {
            //从AB包中获取文件
            filepath = luaScriptsFolder + filepath.Replace(".", "/") + ".lua";
            string[] args = filepath.Split('/');
            string name = args[args.Length - 1];
            TextAsset textAsset = Asset.Load<TextAsset>(filepath.Substring(0, filepath.Length - name.Length), name);
            return textAsset == null ? null : textAsset.bytes;
        }
    }
}