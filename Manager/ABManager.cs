using System.Collections.Generic;
using System.IO;
using Tools;
using UnityEngine;

namespace Manager {
    public class ABManager : Single<ABManager> {
        /// <summary>
        /// 加载过并且未卸载的AB包
        /// </summary>
        private Dictionary<string, AssetBundle> loadedDic = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 单一的总包
        /// </summary>
        private AssetBundle single;

        /// <summary>
        /// 总的构建清单
        /// </summary>
        private AssetBundleManifest manifest;

        /// <summary>
        /// ab包的路径
        /// </summary>
        private string abPath = "";

        public string ABPath {
            get {
                if (abPath == "") {
                    #if UNITY_STANDALONE_WIN
                    abPath = Application.streamingAssetsPath + "/";
                    #elif UNITY_ANDROID || UNITY_IOS
                    abPath = Application.persistentDataPath + "/";
                    #endif
                }
                return abPath;
            }
        }

        /// <summary>
        /// 总的AB包的名字
        /// </summary>
        public string SingleABName {
            get {
                //可能平台不同，总的AB包的名字可能不同
                #if UNITY_STANDALONE_WIN
                return "Win";
                #elif UNITY_ANDROID
            return "Android";
                #elif UNITY_IOS
            return "IOS";
                #else
            Debug.LogError("未指定该平台的名字");
                #endif
                return "";
            }
        }

        /// <summary>
        /// 加载AB包
        /// </summary>
        /// <returns></returns>
        public AssetBundle LoadAssetBundle(string abName) {
            //要加载依赖项，要先获取依赖，要获取依赖，先加载到总的构建清单

            //先判断总包是否加载过
            if (single == null) {
                if (!File.Exists(ABPath + SingleABName)) {
                    Debug.LogWarning($"未找到路径: {ABPath}{SingleABName}");
                    return null;
                }
                single = AssetBundle.LoadFromFile(ABPath + SingleABName);
            }

            //再判断构建清单是否加载过
            if (manifest == null) {
                //从总包里加载构建清单
                manifest = single.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }

            //获取要加载AB包的依赖项
            string[] deps = manifest.GetAllDependencies(abName);
            //遍历数据加载依赖项
            for (int i = 0; i < deps.Length; i++) {
                string depABName = deps[i];
                //方法本身就是加载AB包的， 在调用本身就是加载另一个AB包
                //两个AB包相互依赖的情况下，递归会造成死循环
                //LoadAssetBundle(depABName);

                //判断是否加载过
                if (!loadedDic.ContainsKey(depABName)) {
                    AssetBundle depAB = AssetBundle.LoadFromFile(ABPath + depABName);
                    loadedDic.Add(depABName, depAB);
                }
            }

            //是否加载过
            if (!loadedDic.TryGetValue(abName, out AssetBundle ab)) {
                //没找到， 未加载过，或加载过之后卸载了, 
                //需要重新加载
                //加载该ab包前，需要先加载它的依赖项
                if (File.Exists(ABPath + abName)) {
                    //依赖都加载完毕了
                    //在加载当前的AB包
                    ab = AssetBundle.LoadFromFile(ABPath + abName);
                    //Debug.Log("加载了 " + abName + " 包");
                    //将加载进来的AB包添加到字典中
                    loadedDic.Add(abName, ab);
                }
            }
            return ab;
        }


        /// <summary>
        /// 卸载AB包
        /// </summary>
        public void UnloadAssetBundle(string abName, bool unloadAllObjects = false) {
            //是否加载了
            AssetBundle ab = null;
            if (loadedDic.TryGetValue(abName, out ab)) {
                //卸载
                ab.Unload(unloadAllObjects);
                //将其从字典中移除
                loadedDic.Remove(abName);
                // Debug.Log("卸载了：" + abName);
            }
        }

        /// <summary>
        /// 卸载全部的AB包
        /// </summary>
        /// <param name="unloadAllObjects"></param>
        public void UnloadAll(bool unloadAllObjects = false) {
            //遍历字典的值
            foreach (AssetBundle assetbundle in loadedDic.Values) {
                assetbundle.Unload(unloadAllObjects);
            }
            //清空字典，不清空的的情况下，键值对还存在，只不过是值为null
            loadedDic.Clear();
        }


        /// <summary>
        /// 从指定的AB包中加载指定名字的资源 T为资源类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <param name="abName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetName, string abName)
            where T : Object {
            //先获取AB包, 利用加载AB包的方法获取AB包，即使之前加载过，方法内部判断了不会重复的加载
            AssetBundle ab = LoadAssetBundle(abName);
            if (ab != null && ab.Contains(assetName)) {
                //从AB包中获取资源
                T asset = ab.LoadAsset<T>(assetName);
                return asset;
            }
            //复用非泛型方法
            //Object asset = LoadAsset(assetName, abName, typeof(T));
            //return asset as T;
            return default(T);
        }


        public Object LoadAsset(string assetName, string abName, System.Type type) {
            //先获取AB包
            AssetBundle ab = LoadAssetBundle(abName);
            if (ab != null) {
                //从ab包中获取资源
                Object asset = ab.LoadAsset(assetName, type);
                return asset;
            }
            return null;
        }
    }
}